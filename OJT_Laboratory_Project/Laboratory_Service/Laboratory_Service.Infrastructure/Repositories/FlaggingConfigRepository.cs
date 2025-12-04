using System;
using System.Collections.Generic;
using System.Linq;
using Laboratory_Service.Application.DTOs.FlaggingConfig;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for FlaggingConfig operations.
    /// </summary>
    public class FlaggingConfigRepository : IFlaggingConfigRepository
    {
        private readonly AppDbContext _dbContext;

        public FlaggingConfigRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the active flagging configuration for a specific test code and gender.
        /// Logic: 
        /// 1) Query 1 lần: Lấy tất cả configs của testCode (cả Gender=null và gender-specific)
        /// 2) Ưu tiên: Config chung (Gender=null) → nếu có thì dùng luôn, bỏ qua gender từ message
        /// 3) Nếu không có config chung → mới xét gender từ message để tìm config gender-specific
        /// Tối ưu: Chỉ 1 query thay vì 2 queries, xử lý logic trong memory.
        /// </summary>
        public async Task<FlaggingConfig?> GetActiveConfigAsync(string testCode, string? gender, CancellationToken cancellationToken = default)
        {
            // Bước 1: Query 1 lần - Lấy TẤT CẢ configs của testCode này (cả Gender=null và gender-specific)
            // Ví dụ: Với testCode="Hb" → lấy cả [Hb, Gender=null], [Hb, Gender=Male], [Hb, Gender=Female]
            var configs = await _dbContext.FlaggingConfigs
                .AsNoTracking()
                .Where(fc => fc.TestCode == testCode && fc.IsActive)
                .OrderByDescending(fc => fc.Version)  // Ưu tiên version mới nhất
                .ToListAsync(cancellationToken);

            // Bước 2: Ưu tiên config chung (Gender=null) - Nếu có thì dùng luôn, BỎ QUA gender từ message
            var generalConfig = configs.FirstOrDefault(c => c.Gender == null);
            if (generalConfig != null)
            {
                return generalConfig;  // Trả về ngay, không cần xét gender từ message
            }

            // Bước 3: Không có config chung → mới xét gender từ message
            // Nếu message không có gender → không tìm được config
            if (string.IsNullOrEmpty(gender))
            {
                return null;
            }

            // Tìm config theo gender cụ thể từ message (trong danh sách đã lấy về)
            return configs.FirstOrDefault(c => c.Gender == gender);
        }

        /// <summary>
        /// Gets all active flagging configurations.
        /// </summary>
        public async Task<List<FlaggingConfig>> GetAllActiveConfigsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.FlaggingConfigs
                .AsNoTracking()
                .Where(fc => fc.IsActive)
                .OrderBy(fc => fc.TestCode)
                .ThenBy(fc => fc.Gender ?? "ZZZ") // null ở cuối
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Upsert danh sách FlaggingConfig trong 1 transaction.
        /// </summary>
        public async Task UpsertConfigsAsync(IEnumerable<FlaggingConfigUpsertItemDto> configs, CancellationToken cancellationToken = default)
        {
            if (configs == null)
            {
                return;
            }

            var payload = configs
                .Where(c => !string.IsNullOrWhiteSpace(c.TestCode))
                .Select(NormalizeDto)
                .ToList();

            if (payload.Count == 0)
            {
                return;
            }

            var targetTestCodes = payload.Select(c => c.TestCode).Distinct().ToList();

            var existingConfigs = await _dbContext.FlaggingConfigs
                .Where(fc => targetTestCodes.Contains(fc.TestCode))
                .ToListAsync(cancellationToken);

            var existingDict = existingConfigs
                .ToDictionary(fc => BuildKey(fc.TestCode, fc.Gender));

            var now = DateTime.UtcNow;

            foreach (var item in payload)
            {
                ValidateMinMax(item);
                var key = BuildKey(item.TestCode, item.Gender);

                if (existingDict.TryGetValue(key, out var entity))
                {
                    ApplyUpdates(entity, item, now);
                }
                else
                {
                    var newEntity = CreateNewEntity(item, now);
                    await _dbContext.FlaggingConfigs.AddAsync(newEntity, cancellationToken);
                    existingDict[key] = newEntity;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static string BuildKey(string testCode, string? gender)
        {
            var normalizedGender = string.IsNullOrWhiteSpace(gender) ? "NULL" : gender.Trim().ToUpperInvariant();
            return $"{testCode.Trim().ToUpperInvariant()}|{normalizedGender}";
        }

        private static FlaggingConfigUpsertItemDto NormalizeDto(FlaggingConfigUpsertItemDto dto)
        {
            dto.TestCode = dto.TestCode.Trim();
            if (dto.ParameterName != null)
            {
                dto.ParameterName = dto.ParameterName.Trim();
            }

            if (dto.Description != null)
            {
                dto.Description = dto.Description.Trim();
            }

            if (dto.Unit != null)
            {
                dto.Unit = dto.Unit.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Gender))
            {
                dto.Gender = dto.Gender.Trim();
            }
            else
            {
                dto.Gender = null;
            }

            return dto;
        }

        private static void ApplyUpdates(FlaggingConfig entity, FlaggingConfigUpsertItemDto dto, DateTime now)
        {
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(dto.ParameterName))
            {
                var value = dto.ParameterName.Trim();
                if (!string.Equals(entity.ParameterName, value, StringComparison.Ordinal))
                {
                    entity.ParameterName = value;
                    hasChanges = true;
                }
            }

            if (dto.Description != null)
            {
                var value = dto.Description.Trim();
                if (!string.Equals(entity.Description ?? string.Empty, value, StringComparison.Ordinal))
                {
                    entity.Description = value;
                    hasChanges = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Unit))
            {
                var value = dto.Unit.Trim();
                if (!string.Equals(entity.Unit ?? string.Empty, value, StringComparison.Ordinal))
                {
                    entity.Unit = value;
                    hasChanges = true;
                }
            }

            if (dto.Min.HasValue)
            {
                if (!entity.Min.Equals(dto.Min.Value))
                {
                    entity.Min = dto.Min.Value;
                    hasChanges = true;
                }
            }

            if (dto.Max.HasValue)
            {
                if (!entity.Max.Equals(dto.Max.Value))
                {
                    entity.Max = dto.Max.Value;
                    hasChanges = true;
                }
            }

            if (dto.IsActive.HasValue)
            {
                if (entity.IsActive != dto.IsActive.Value)
                {
                    entity.IsActive = dto.IsActive.Value;
                    hasChanges = true;
                }
            }

            if (dto.EffectiveDate.HasValue)
            {
                var value = dto.EffectiveDate.Value;
                if (entity.EffectiveDate != value)
                {
                    entity.EffectiveDate = value;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                entity.Version += 1;
                entity.UpdatedAt = now;
            }
        }

        private static FlaggingConfig CreateNewEntity(FlaggingConfigUpsertItemDto dto, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(dto.ParameterName))
            {
                throw new InvalidOperationException($"ParameterName is required for new config (TestCode: {dto.TestCode}).");
            }

            if (string.IsNullOrWhiteSpace(dto.Unit))
            {
                throw new InvalidOperationException($"Unit is required for new config (TestCode: {dto.TestCode}).");
            }

            if (!dto.Min.HasValue || !dto.Max.HasValue)
            {
                throw new InvalidOperationException($"Min/Max are required for new config (TestCode: {dto.TestCode}).");
            }

            return new FlaggingConfig
            {
                TestCode = dto.TestCode,
                ParameterName = dto.ParameterName,
                Description = dto.Description ?? string.Empty,
                Unit = dto.Unit,
                Gender = dto.Gender,
                Min = dto.Min.Value,
                Max = dto.Max.Value,
                Version = 1,
                IsActive = dto.IsActive ?? true,
                EffectiveDate = dto.EffectiveDate ?? now,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static void ValidateMinMax(FlaggingConfigUpsertItemDto dto)
        {
            if (dto.Min.HasValue && dto.Max.HasValue && dto.Min.Value >= dto.Max.Value)
            {
                throw new InvalidOperationException($"Min must be less than Max for TestCode {dto.TestCode} (Gender: {dto.Gender ?? "General"}).");
            }
        }

        public async Task<FlaggingConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.FlaggingConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(fc => fc.FlaggingConfigId == id, cancellationToken);
        }

    }
}

