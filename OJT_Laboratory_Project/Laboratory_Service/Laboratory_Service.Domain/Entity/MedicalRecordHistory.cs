using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Stores previous snapshot / audit information for MedicalRecord changes.
    /// </summary>
    public class MedicalRecordHistory
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// JSON snapshot of the record before modification
        /// </summary>
        /// <value>
        /// The snapshot json.
        /// </value>
        public string SnapshotJson { get; set; } = string.Empty;

        /// <summary>
        /// Short human-readable summary of changed fields, e.g. "TestResults: old -&gt; new; ClinicalNotes: old -&gt; new"
        /// </summary>
        /// <value>
        /// The change summary.
        /// </value>
        public string ChangeSummary { get; set; } = string.Empty;

        /// <summary>
        /// Who performed the change
        /// </summary>
        /// <value>
        /// The changed by.
        /// </value>
        public string ChangedBy { get; set; } = string.Empty;

        /// <summary>
        /// When the change happened
        /// </summary>
        /// <value>
        /// The changed at.
        /// </value>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}