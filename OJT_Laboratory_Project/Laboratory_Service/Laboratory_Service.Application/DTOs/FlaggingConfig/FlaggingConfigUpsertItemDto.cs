namespace Laboratory_Service.Application.DTOs.FlaggingConfig;

/// <summary>
/// DTO chứa thông tin cấu hình flagging cần đồng bộ/ cập nhật xuống hệ thống.
/// </summary>
public class FlaggingConfigUpsertItemDto
{
    public string TestCode { get; set; } = string.Empty;

    public string? ParameterName { get; set; }

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public string? Gender { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? EffectiveDate { get; set; }
}

