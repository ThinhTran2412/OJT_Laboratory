using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Patient operations
    /// </summary>
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryptionService;

        public PatientRepository(AppDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            return await _context.Patients
                .Where(p => p.PatientId == patientId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetByIdentifyNumberAsync(string identifyNumber)
        {
            //var encryptedId = _encryptionService.Encrypt(identifyNumber);

            return await _context.Patients
                .Where(p => p.IdentifyNumber == identifyNumber)
                .FirstOrDefaultAsync();
        }

      
        public async Task<Patient> AddAsync(Patient patient)
        {
            if (!string.IsNullOrEmpty(patient.IdentifyNumber))
                patient.IdentifyNumber = _encryptionService.Encrypt(patient.IdentifyNumber);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> DeleteAsync(int patientId, string deletedBy)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByIdentifyNumberAsync(string identifyNumber)
        {
            var encryptedId = _encryptionService.Encrypt(identifyNumber);
            return await _context.Patients
                .AnyAsync(p => p.IdentifyNumber == identifyNumber);
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            var patients = await _context.Patients
                .AsNoTracking()
                .ToListAsync();

            foreach (var p in patients)
            {
                if (!string.IsNullOrEmpty(p.IdentifyNumber))
                {
                    try
                    {
                        p.IdentifyNumber = _encryptionService.Decrypt(p.IdentifyNumber);
                    }
                    catch
                    {
                        p.IdentifyNumber = p.IdentifyNumber;
                    }
                }
            }

            return patients;
        }

    }
}
