namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Configuration for flagging test results based on reference ranges.
    /// Defines thresholds (Min/Max) for each test code and gender to automatically flag results as Low, Normal, or High.
    /// </summary>
    public class FlaggingConfig
    {
        /// <summary>
        /// Unique identifier for the flagging configuration.
        /// </summary>
        public int FlaggingConfigId { get; set; }

        /// <summary>
        /// Test code/Abbreviation (e.g., "WBC", "Hb", "RBC", "HCT", "PLT", "MCV", "MCH", "MCHC").
        /// </summary>
        public string TestCode { get; set; } = string.Empty;

        /// <summary>
        /// Full parameter name (e.g., "White Blood Cell Count", "Hemoglobin").
        /// </summary>
        public string ParameterName { get; set; } = string.Empty;

        /// <summary>
        /// Description of what the parameter measures.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement (e.g., "cells/µL", "g/dL", "%", "fL", "pg").
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Gender-specific configuration: "Male", "Female", or null (applies to both genders).
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Minimum threshold value. Values below this are flagged as "Low".
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Maximum threshold value. Values above this are flagged as "High".
        /// Values between Min and Max (inclusive) are flagged as "Normal".
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Version of the configuration (for tracking changes and sync).
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Indicates if this configuration is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the configuration becomes effective.
        /// </summary>
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the record was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}

