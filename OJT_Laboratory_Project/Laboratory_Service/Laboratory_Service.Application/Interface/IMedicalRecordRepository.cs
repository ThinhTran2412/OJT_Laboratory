using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Repository interface for Medical Record operations
    /// </summary>
    public interface IMedicalRecordRepository
    {
        /// <summary>
        /// Get medical record by ID
        /// </summary>
        /// <param name="medicalRecordId">The medical record identifier.</param>
        /// <returns></returns>
        Task<MedicalRecord?> GetByIdAsync(int medicalRecordId);

        /// <summary>
        /// Get medical records by patient ID
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId);

        /// <summary>
        /// Add new medical record
        /// </summary>
        /// <param name="medicalRecord">The medical record.</param>
        /// <returns></returns>
        Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord);

        /// <summary>
        /// Update existing medical record
        /// </summary>
        /// <param name="medicalRecord">The medical record.</param>
        /// <returns></returns>
        Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord);

        /// <summary>
        /// Soft delete medical record
        /// </summary>
        /// <param name="medicalRecordId">The medical record identifier.</param>
        /// <param name="deletedBy">The deleted by.</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int medicalRecordId, string deletedBy);

        /// <summary>
        /// Get medical records by patient ID with pagination (simple version for gRPC)
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize);

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<MedicalRecord>> GetAllAsync();
        Task<MedicalRecordHistory> AddHistoryAsync(MedicalRecordHistory history);
        Task<MedicalRecord?> GetMedicalRecordById(int patientId);
    }
}
