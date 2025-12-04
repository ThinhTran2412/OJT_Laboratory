/// <summary>
/// Data Transfer Object (DTO) for User entity.
/// Used to transfer user data between application layers
/// without exposing the full domain model.
/// </summary>
namespace Laboratory_Service.Application.DTOs.User
{
    /// <summary>
    /// Represents the basic information of a user.
    /// </summary>
    /// <param name="IdentifyNumber">Unique identifier for the user (e.g., ID card or passport number).</param>
    /// <param name="FullName">Full name of the user.</param>
    /// <param name="DateOfBirth">Date of birth of the user.</param>
    /// <param name="Gender">Gender of the user (e.g., Male, Female, Other).</param>
    /// <param name="PhoneNumber">Contact phone number of the user.</param>
    /// <param name="Email">Email address of the user.</param>
    /// <param name="Address">Physical address of the user.</param>
    public record UserDataDTO(
        string IdentifyNumber,    
        string FullName,          
        DateOnly DateOfBirth,     
        string Gender,            
        string PhoneNumber,       
        string Email,             
        string Address,
        int? RoleId,
        int Age
    );
}
