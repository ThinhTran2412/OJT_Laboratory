using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.Test_Result
{
    /// <summary>
    /// Handler for SaveRawTestResultCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Test_Result.SaveRawTestResultCommand, System.Boolean&gt;" />
    public class SaveRawTestResultHandler : IRequestHandler<SaveRawTestResultCommand, bool>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawBackupRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveRawTestResultHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public SaveRawTestResultHandler(IRawBackupRepository repository)
        {
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
        public async Task<bool> Handle(SaveRawTestResultCommand request, CancellationToken cancellationToken)
        {
            var rawBackup = new RawBackup
            {
                Id = Guid.NewGuid(),
                RawContent = request.RawJson,
                ReceivedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(rawBackup);
            return true;
        }

    }
}
