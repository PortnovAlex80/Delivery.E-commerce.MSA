using MediatR;
using Quartz;

namespace DeliveryApp.Ui.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob:IJob
{
    private readonly IMediator _mediator;
    public AssignOrdersJob(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var assignOrderToCourierCommand = new Core.Application.UseCases.Commands.AssignOrderToCourier.Command();
        Console.WriteLine("Background job - assign to order");
        await _mediator.Send(assignOrderToCourierCommand);
    }
}