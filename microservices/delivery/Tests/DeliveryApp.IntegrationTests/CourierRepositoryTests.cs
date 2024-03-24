using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests;

public class CourierRepositoryShould: IAsyncLifetime
{
    private ApplicationDbContext _context;
    private Location _location;
    private Weight _weight;
    
    /// <summary>
    /// Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("order")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();
    
    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public CourierRepositoryShould()
    {
        var weightCreateResult=Weight.Create(2);
        weightCreateResult.IsSuccess.Should().BeTrue();
        _weight = weightCreateResult.Value;
        
        var locationCreateResult = Location.Create(1,1);
        locationCreateResult.IsSuccess.Should().BeTrue();
        _location = locationCreateResult.Value;
    }
    
    /// <summary>
    /// Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();
        
        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_postgreSqlContainer.GetConnectionString(),
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);                        
                })
            .Options;
        _context = new ApplicationDbContext(contextOptions);
        _context.Database.Migrate();
    }

    /// <summary>
    /// Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async void CanAddCourier()
    {
        //Arrange
        var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;
        
        //Act
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async void CanUpdateCourier()
    {
        //Arrange
        var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;
        
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Act
        var courierStartWorkResult = courier.StartWork();
        courierStartWorkResult.IsSuccess.Should().BeTrue();
        courierRepository.Update(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
        courierFromDb.Status.Should().Be(CourierStatus.Ready);
    }
    
    [Fact]
    public async void CanGetById()
    {
        //Arrange
        var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;
        
        //Act
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async void CanGetAllActive()
    {
        //Arrange
        var courier1CreateResult = Courier.Create("Иван", Transport.Pedestrian);
        courier1CreateResult.IsSuccess.Should().BeTrue();
        var courier1 = courier1CreateResult.Value;
        courier1.StopWork();
        
        var courier2CreateResult = Courier.Create("Борис", Transport.Pedestrian);
        courier2CreateResult.IsSuccess.Should().BeTrue();
        var courier2 = courier2CreateResult.Value;
        courier2.StartWork();
        
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier1);
        courierRepository.Add(courier2);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Act
        var activeCouriersFromDb= courierRepository.GetAllActive();
        
        //Assert
        activeCouriersFromDb.Should().NotBeEmpty();
        activeCouriersFromDb.Count().Should().Be(1);
        activeCouriersFromDb.First().Should().BeEquivalentTo(courier2);
    }
}