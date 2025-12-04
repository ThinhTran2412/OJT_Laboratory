using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface IPatientService
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Synchronizes the patient with user asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <param name="updatedBy">The updated by.</param>
        /// <returns></returns>
        Task<Patient?> SynchronizePatientWithUserAsync(string identifyNumber, string updatedBy);
        /// <summary>
        /// Creates the patient from user asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        Task<Patient?> CreatePatientFromUserAsync(CreatePatientClient patientClient, string createdBy);

        /// <summary>
        /// Gets the patient by identity number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<PatientDto?> GetPatientByIdentityNumberAsync(string identifyNumber);
    }
}
