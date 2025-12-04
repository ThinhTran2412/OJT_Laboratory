using MediatR;
using System.Text.Json.Serialization;

/// <summary>
/// create attribute for class CreateTestOrderCommand
/// </summary>
public class CreateTestOrderCommand : IRequest<Guid>
{
    // Patient info
    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    /// <value>
    /// The full name.
    /// </value>
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the date of birth.
    /// </summary>
    /// <value>
    /// The date of birth.
    /// </value>
    public DateOnly DateOfBirth { get; set; }
    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    /// <value>
    /// The gender.
    /// </value>
    public string Gender { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    /// <value>
    /// The phone number.
    /// </value>
    public string? PhoneNumber { get; set; }
    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    public string? Email { get; set; }
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public string? Address { get; set; }

    // TestOrder info
    /// <summary>
    /// Gets or sets the type of the test.
    /// </summary>
    /// <value>
    /// The type of the test.
    /// </value>
    public string TestType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    public string Priority { get; set; } = "Normal";
    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    /// <value>
    /// The note.
    /// </value>
    public string? Note { get; set; }

    // Identity & creator
    /// <summary>
    /// Gets or sets the identify number.
    /// </summary>
    /// <value>
    /// The identify number.
    /// </value>
    public string IdentifyNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    /// <value>
    /// The created by.
    /// </value>
    [JsonIgnore]
    public string CreatedBy { get; set; } = string.Empty;
}
