using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    /// <summary>
    /// Назначить заказы на курьеров
    /// </summary>
    public class Command : IRequest<bool>
    {
        /// <summary>
        /// Ctr
        /// </summary>
        public Command()
        {
            
        }
    }
}