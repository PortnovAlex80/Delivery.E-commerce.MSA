using System.Reflection;
using Api.Filters;
using Api.Formatters;
using Api.OpenApi;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using DeliveryApp.Infrastructure;

using DeliveryApp.Infrastructure.Adapters.Postgres;

using MassTransit;
using MediatR;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;
using Quartz;
using DeliveryApp.Ui.Adapters.BackgroundJobs;
using DeliveryApp.Ui.Adapters.RabbitMq;
using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Application.DomainEventHandlers;
using DeliveryApp.Infrastructure.Adapters.RabbitMq;

namespace DeliveryApp.Ui
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            Configuration = configuration;

        }

        /// <summary>
        /// Конфигурация
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.Configure<Settings>(options => Configuration.Bind(options));


            var connectionString = Configuration["CONNECTION_STRING"];
            var rabbitMqHost = Configuration["RABBIT_MQ_HOST"];
            var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];

            // БД 
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(connectionString,
                        npgsqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);                        
                        });
                    options.EnableSensitiveDataLogging();                
                }
            );

            services.AddTransient(x => new Client(geoServiceGrpcHost));
            // gRPC
            services.AddGrpcClient<Client>(options => 
            { 
                options.Address = new Uri(geoServiceGrpcHost); 
            });

            // CRON Jobs
            services.AddQuartz(configure =>
            {
                var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
                var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
                configure
                    .AddJob<AssignOrdersJob>(assignOrdersJobKey)
                    .AddTrigger(
                        trigger => trigger.ForJob(assignOrdersJobKey)
                            .WithSimpleSchedule(
                                schedule => schedule.WithIntervalInSeconds(20)
                                    .RepeatForever()))
                    .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                    .AddTrigger(
                        trigger => trigger.ForJob(moveCouriersJobKey)
                            .WithSimpleSchedule(
                                schedule => schedule.WithIntervalInSeconds(2)
                                    .RepeatForever()));
                configure.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService();
        
            //Postgres
            services.AddTransient<ICourierRepository, CourierRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IGeoClient>(x => new Client(geoServiceGrpcHost));
            services.AddTransient<IBusProducer,  RabbitMqBusProducer>();

            //MediatR
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());
            services.AddMediatR(typeof(Startup).Assembly);
            
            // Commands
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.CreateOrder.Command,bool>,
                    Core.Application.UseCases.Commands.CreateOrder.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.MoveToOrder.Command,bool>,
                Core.Application.UseCases.Commands.MoveToOrder.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.AssignOrderToCourier.Command,bool>,
                Core.Application.UseCases.Commands.AssignOrderToCourier.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.StartWork.Command,bool>,
                Core.Application.UseCases.Commands.StartWork.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.StopWork.Command,bool>,
                Core.Application.UseCases.Commands.StopWork.Handler>();
            
            // Queries
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetOrder.Query,
                Core.Application.UseCases.Queries.GetOrder.Response>>(x 
                => new Core.Application.UseCases.Queries.GetOrder.Handler(connectionString));
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetActiveOrders.Query,
            Core.Application.UseCases.Queries.GetActiveOrders.Response>>(x 
                => new Core.Application.UseCases.Queries.GetActiveOrders.Handler(connectionString));
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetAllCouriers.Query,
            Core.Application.UseCases.Queries.GetAllCouriers.Response>>(x 
                => new Core.Application.UseCases.Queries.GetAllCouriers.Handler(connectionString));

            // MediatR Domain Event Handlers
            services.AddTransient<INotificationHandler<OrderCreatedDomainEvent>,OrderCreatedDomainEventHandler>();
            services.AddTransient<INotificationHandler<OrderAssignedDomainEvent>,OrderAssignedDomainEventHandler>();
            services.AddTransient<INotificationHandler<OrderCompletedDomainEvent>,OrderCompletedDomainEventHandler>();


            // HTTP Handlers
            services.AddControllers(options =>
                {
                    options.InputFormatters.Insert(0, new InputFormatterStream());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("1.0.0", new OpenApiInfo
                {
                    Title = "Delivery Service",
                    Description = "Отвечает за учет курьеров, деспетчеризацию доставкуов, доставку",
                    Contact = new OpenApiContact
                    {
                        Name = "Kirill Vetchinkin",
                        Url = new Uri("https://microarch.ru"),
                        Email = "info@microarch.ru"
                    }
                });
                options.CustomSchemaIds(type => type.FriendlyId(true));
                options.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
                options.DocumentFilter<BasePathFilter>("");
                options.OperationFilter<GeneratePathParamsValidationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

                        // Message Broker
            services.AddMassTransit(x =>
            {
                //Consumers
                x.AddConsumer<BasketConfirmedConsumer>();
                x.UsingRabbitMq((context,cfg) =>
                {
                    cfg.Host(rabbitMqHost, "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8087")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            
            app.UseCors("AllowSpecificOrigin");
            app.UseHealthChecks("/health");
            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                })
                .UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "openapi";
                    options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Delivery Service");
                    options.RoutePrefix = string.Empty;
                    options.SwaggerEndpoint("/openapi-original.json", "Swagger Delivery Service");
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}