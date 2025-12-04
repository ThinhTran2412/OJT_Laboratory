using MediatR;
using Simulator.Application.Interface;

namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Handles the send raw test result command
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Simulator.Application.SimulateRawData.Command.SendRawTestResultCommand, Simulator.Application.SimulateRawData.Command.SendRawTestResultResult&gt;" />
    public class SendRawTestResultCommandHandler : IRequestHandler<SendRawTestResultCommand, SendRawTestResultResult>
    {
        /// <summary>
        /// The publisher
        /// </summary>
        private readonly IRabbitMQPublisher _publisher;
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawTestResultRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendRawTestResultCommandHandler"/> class.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="repository">The repository.</param>
        public SendRawTestResultCommandHandler(IRabbitMQPublisher publisher, IRawTestResultRepository repository)
        {
            _publisher = publisher;
            _repository = repository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<SendRawTestResultResult> Handle(SendRawTestResultCommand request, CancellationToken cancellationToken)
        {
            var messageId = await _publisher.PublishAsync("raw_test_result_queue", request.RawResult);

            await _repository.MarkAsSentAsync(request.RawResult.TestOrderId);

            return new SendRawTestResultResult(true, messageId);
        }
    }
}
