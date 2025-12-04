using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Medical Record operations
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IMedicalRecordRepository" />
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRecordRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MedicalRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get medical record by ID
        /// </summary>
        /// <param name="medicalRecordId">The medical record identifier.</param>
        /// <returns></returns>
        public async Task<MedicalRecord?> GetByIdAsync(int medicalRecordId)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.MedicalRecordId == medicalRecordId)
                .Include(mr => mr.Patient)
                .Include(mr => mr.TestOrders)
                    .ThenInclude(to => to.TestResults)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get medical records by patient ID
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.PatientId == patientId)
                .Include(mr => mr.Patient)
                .ToListAsync();
        }

        /// <summary>
        /// Add new medical record
        /// </summary>
        /// <param name="medicalRecord">The medical record.</param>
        /// <returns></returns>
        public async Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }

        /// <summary>
        /// Update existing medical record
        /// </summary>
        /// <param name="medicalRecord">The medical record.</param>
        /// <returns></returns>
        public async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }

        /// <summary>
        /// Soft delete medical record
        /// </summary>
        /// <param name="medicalRecordId">The medical record identifier.</param>
        /// <param name="deletedBy">The deleted by.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int medicalRecordId, string deletedBy)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(medicalRecordId);
            if (medicalRecord == null) return false;

            medicalRecord.IsDeleted = true;
            medicalRecord.UpdatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedBy = deletedBy;

            _context.MedicalRecords.Update(medicalRecord);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get medical records by patient ID with pagination (simple version for gRPC)
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public async Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.PatientId == patientId)
                .Include(mr => mr.Patient)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<TestOrder>> GetAllByPatientIdAsync(int patientId, CancellationToken cancellationToken)
        {
            return await _context.TestOrders
                .Where(to => to.MedicalRecord.PatientId == patientId && !to.IsDeleted)
                .Include(to => to.TestResults) 
                .ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)         
                .Include(mr => mr.TestOrders)       
                .Where(mr => !mr.IsDeleted)        
                .ToListAsync();
        }
        /// <summary>
        /// Adds the history asynchronous.
        /// </summary>
        /// <param name="history">The history.</param>
        /// <returns></returns>
        public async Task<MedicalRecordHistory> AddHistoryAsync(MedicalRecordHistory history)
        {
            _context.MedicalRecordHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        /// <summary>
        /// Gets the medical record by identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<MedicalRecord?> GetMedicalRecordById(int patientId)
        {
            return await _context.MedicalRecords
                                 .FirstOrDefaultAsync(mr => mr.PatientId == patientId);
        }
    }
}
