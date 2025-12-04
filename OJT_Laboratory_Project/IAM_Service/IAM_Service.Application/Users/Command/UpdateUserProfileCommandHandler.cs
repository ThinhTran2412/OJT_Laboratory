using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Command
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
    {
        private readonly IUsersRepository _userRepository;

        // Constructor
        public UpdateUserProfileCommandHandler(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Handle method
        public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin User hiện tại
            // Ghi chú: Tôi giả định request.UserId là ID của người dùng đang cập nhật.
            // Nếu bạn lấy ID từ token/context, hãy thay thế logic này.
            var user = await _userRepository.GetByUserIdAsync(request.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // 2. Cập nhật thông tin cá nhân
            bool hasChanges = false;

            // FullName
            if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != user.FullName)
            {
                user.FullName = request.FullName.Trim();
                hasChanges = true;
            }

            // Email (Thường cần kiểm tra trùng lặp và xác minh, nhưng ở đây chỉ gán)
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                // **LƯU Ý:** Bạn nên thêm logic kiểm tra email này đã tồn tại chưa ở đây.
                user.Email = request.Email.Trim();
                hasChanges = true;
            }

            // PhoneNumber
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = request.PhoneNumber.Trim();
                hasChanges = true;
            }

            // Gender
            if (!string.IsNullOrWhiteSpace(request.Gender) && request.Gender != user.Gender)
            {
                user.Gender = request.Gender.Trim();
                hasChanges = true;
            }

            // Age
            if (request.Age.HasValue && request.Age.Value > 0 && request.Age != user.Age)
            {
                user.Age = request.Age.Value;
                hasChanges = true;
            }

            // Address
            if (!string.IsNullOrWhiteSpace(request.Address) && request.Address != user.Address)
            {
                user.Address = request.Address.Trim();
                hasChanges = true;
            }

            // DateOfBirth
            if (request.DateOfBirth.HasValue && request.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = request.DateOfBirth.Value;
                hasChanges = true;
            }

            // 3. Lưu thay đổi
            if (hasChanges)
            {
                // Phương thức UpdateUserAsync trong repository của bạn đã bao gồm SaveChangesAsync
                await _userRepository.UpdateUserAsync(user);
            }
            // Nếu không có thay đổi nào, chỉ cần trả về Unit.Value

            return Unit.Value;
        }
    }
}