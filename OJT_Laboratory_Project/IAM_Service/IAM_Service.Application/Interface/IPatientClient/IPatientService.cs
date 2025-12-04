using IAM_Service.Application.DTOs;

namespace IAM_Service.Application.Interface.IPatientClient
{
    /// <summary>
    /// Create method for IPatientService
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Gets the patient by identity number asynchronous.
        /// </summary>
        /// <param name="identityNumber">The identity number.</param>
        /// <returns></returns>
        Task<PatientDetailDto?> GetPatientByIdentityNumberAsync(string identityNumber);
    }
}
