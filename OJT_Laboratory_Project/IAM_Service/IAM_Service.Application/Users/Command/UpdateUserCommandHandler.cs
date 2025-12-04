using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Users.Command;
using MediatR;

/// <summary>
/// 
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    /// <summary>
    /// The user repository
    /// </summary>
    private readonly IUsersRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    public UpdateUserCommandHandler(
        IUsersRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">User not found.</exception>
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
{
    var user = await _userRepository.GetByUserIdAsync(request.UserId);
    if (user == null)
        throw new KeyNotFoundException("User not found.");

    bool hasChanges = false;

    if (string.IsNullOrEmpty(request.ActionType) || request.ActionType == "update")
    {
        if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != user.FullName)
        {
            user.FullName = request.FullName;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            user.Email = request.Email;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            user.PhoneNumber = request.PhoneNumber;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Gender) && request.Gender != user.Gender)
        {
            user.Gender = request.Gender;
            hasChanges = true;
        }

        if (request.Age.HasValue && request.Age.Value > 0 && request.Age != user.Age)
        {
            user.Age = request.Age.Value;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Address) && request.Address != user.Address)
        {
            user.Address = request.Address;
            hasChanges = true;
        }

        if (request.DateOfBirth.HasValue && request.DateOfBirth != user.DateOfBirth)
        {
            user.DateOfBirth = request.DateOfBirth.Value;
            hasChanges = true;
        }

        if (hasChanges)
        {
            // Set null để tránh encrypt lại
            user.IdentifyNumber = null;
            await _userRepository.UpdateUserAsync(user);
            // ❌ XÓA dòng này vì UpdateUserAsync đã save rồi
        }
    }


    // update privilege if have ActionType
    if (request.ActionType == "add" && request.PrivilegeIds != null)
    {
        var currentPrivileges = await _userRepository.GetUserPrivilegesAsync(user.UserId);
        var newPrivileges = request.PrivilegeIds.Except(currentPrivileges).ToList();

        if (newPrivileges.Any())
            await _userRepository.AddUserPrivilegesAsync(user.UserId, newPrivileges);
    }
    else if (request.ActionType == "reset")
    {
        var originalPrivileges = await _userRepository.GetOriginalPrivilegesAsync(user.UserId);
        var currentPrivileges = await _userRepository.GetUserPrivilegesAsync(user.UserId);

        var privilegesToRemove = currentPrivileges.Except(originalPrivileges).ToList();

        if (privilegesToRemove.Any())
            await _userRepository.RemoveUserPrivilegesAsync(user.UserId, privilegesToRemove);
    }
    else if (string.IsNullOrEmpty(request.ActionType) && request.PrivilegeIds != null)
    {
        var currentPrivileges = await _userRepository.GetUserPrivilegesAsync(user.UserId);
        var newPrivileges = request.PrivilegeIds.Except(currentPrivileges).ToList();
        var removedPrivileges = currentPrivileges.Except(request.PrivilegeIds).ToList();

        if (newPrivileges.Any())
            await _userRepository.AddUserPrivilegesAsync(user.UserId, newPrivileges);

        if (removedPrivileges.Any())
            await _userRepository.RemoveUserPrivilegesAsync(user.UserId, removedPrivileges);
    }

    // ✅ Chỉ save 1 lần cho privilege changes
    await _userRepository.SaveChangesAsync();

    return Unit.Value;
}

}
