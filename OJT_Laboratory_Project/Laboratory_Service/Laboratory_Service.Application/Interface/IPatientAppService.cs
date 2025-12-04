using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.DTOs.Patients;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Application-level service (use-cases) for patient operations exposed to Presentation/API layers.
    /// This keeps business logic inside the Application Layer and returns DTOs to the API.
    /// </summary>
    public interface IPatientAppService
    {
        /// <summary>
        /// Gets the by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<PatientDto?> GetByIdentifyNumberAsync(string identifyNumber);
        /// <summary>
        /// Existses the by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<bool> ExistsByIdentifyNumberAsync(string identifyNumber);
        /// <summary>
        /// Creates the patient from user asynchronous.
        /// </summary>
        /// <param name="createPatientDto">The create patient dto.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        Task<PatientDto?> CreatePatientFromUserAsync(CreatePatientClient createPatientDto, string createdBy);
        /// <summary>
        /// Gets the patients by identify numbers asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        Task<List<PatientDto>> GetPatientsByIdentifyNumbersAsync(IEnumerable<string> identifyNumbers);
    }
}
