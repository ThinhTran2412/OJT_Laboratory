using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    // Command phải implement IRequest<TResponse> để dùng với IRequestHandler<TCommand, TResponse>
    public class DeleteMedicalRecordCommand : IRequest<DeleteMedicalRecordResultDto>
    {
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// Who performs the deletion (username / identifyNumber / user id as string)
        /// </summary>
        public string DeletedBy { get; set; } = string.Empty;
    }
}