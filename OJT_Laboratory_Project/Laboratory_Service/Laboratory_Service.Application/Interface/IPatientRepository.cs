using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Repository interface for Patient operations
    /// </summary>
    public interface IPatientRepository
    {
        /// <summary>
        /// Get patient by ID
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<Patient?> GetByIdAsync(int patientId);

        /// <summary>
        /// Get patient by identification number
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<Patient?> GetByIdentifyNumberAsync(string identifyNumber);

        /// <summary>
        /// Add new patient
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        Task<Patient> AddAsync(Patient patient);

        /// <summary>
        /// Update existing patient
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        Task<Patient> UpdateAsync(Patient patient);

        /// <summary>
        /// Soft delete patient
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="deletedBy">The deleted by.</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int patientId, string deletedBy);

        /// <summary>
        /// Check if patient exists by identification number
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<bool> ExistsByIdentifyNumberAsync(string identifyNumber);
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<Patient>> GetAllAsync();
    }
}
