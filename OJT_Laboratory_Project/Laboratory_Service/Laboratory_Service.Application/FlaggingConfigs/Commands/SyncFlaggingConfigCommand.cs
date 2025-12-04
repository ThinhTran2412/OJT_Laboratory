using System;
using System.Collections.Generic;
using Laboratory_Service.Application.DTOs.FlaggingConfig;
using MediatR;

namespace Laboratory_Service.Application.FlaggingConfigs.Commands;

/// <summary>
/// Command yêu cầu đồng bộ/ cập nhật danh sách FlaggingConfig từ nguồn cấu hình.
/// </summary>
public class SyncFlaggingConfigCommand : IRequest<SyncFlaggingConfigResultDto>
{
    public IReadOnlyCollection<FlaggingConfigUpsertItemDto> Configs { get; set; } = Array.Empty<FlaggingConfigUpsertItemDto>();

}

