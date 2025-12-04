# Hướng Dẫn Sử Dụng Script Tự Động - Laboratory_Service

Thư mục này chứa các script tự động để chạy test và cập nhật database migration cho project Laboratory_Service.

## 📁 Cấu Trúc Thư Mục

```
Laboratory_Service/
├── Scripts/
│   ├── test.bat              # Batch script để chạy test
│   ├── clean.bat             # Batch script để clean bin/obj và VS files
│   └── migration_update.bat  # Batch script để cập nhật migration
└── Tutorial Auto use.md      # File hướng dẫn này
```

## 🚀 Cách Sử Dụng

### 1. Chạy Test Tự Động

1. Double-click vào file `Scripts\test.bat`
2. Hoặc chạy từ Command Prompt:
   ```cmd
   Scripts\test.bat
   ```

**Script test sẽ tự động:**
- ✅ Clean solution
- ✅ Restore packages
- ✅ Build solution
- ✅ Chạy unit tests với coverage
- ✅ Tạo HTML coverage report

**Kết quả test sẽ được lưu tại:**
- `Laboratory_Service.Application.UnitTest/TestResults/coverage/` - Coverage data
- `Laboratory_Service.Application.UnitTest/TestResults/CoverageReport/` - HTML report

### 2. Clean Bin, Obj và Visual Studio Files

1. Double-click vào file `Scripts\clean.bat`
2. Hoặc chạy từ Command Prompt:
   ```cmd
   Scripts\clean.bat
   ```

**Script clean sẽ tự động:**
- ✅ Dừng các process dotnet/MSBuild đang chạy
- ✅ Xóa tất cả thư mục `bin`, `obj`, và `TestResults`
- ✅ Xóa thư mục `.vs` (Visual Studio cache)
- ✅ Xóa các file `.user` và `.suo` (Visual Studio user settings)
- ✅ Chạy `dotnet clean` để clean solution

**Lưu ý:**
- Script này sẽ xóa tất cả build artifacts và Visual Studio cache
- Hữu ích khi gặp lỗi build hoặc muốn clean hoàn toàn project

### 3. Cập Nhật Database Migration

1. Double-click vào file `Scripts\migration_update.bat`
2. Hoặc chạy từ Command Prompt:
   ```cmd
   Scripts\migration_update.bat
   ```

**Script migration sẽ tự động:**
- ✅ Kiểm tra và cài đặt EF Core Tools (nếu chưa có)
- ✅ Restore packages
- ✅ Build solution
- ✅ Áp dụng tất cả migrations chưa được apply vào database

**Lưu ý:**
- Đảm bảo connection string trong `appsettings.json` hoặc `appsettings.Development.json` đã được cấu hình đúng
- Script sẽ cập nhật database của project **Laboratory_Service.API**

## ⚙️ Yêu Cầu Hệ Thống

- .NET SDK 8.0 hoặc cao hơn
- Entity Framework Core Tools (sẽ được tự động cài đặt nếu chưa có)
- PostgreSQL database (cho migration)

## 📝 Lưu Ý

1. **Test Script:**
   - Script test cho phép một số test fail và vẫn tiếp tục chạy
   - Coverage report sẽ được tạo tự động sau khi test hoàn thành

2. **Migration Script:**
   - Script sẽ cập nhật database lên migration mới nhất
   - Đảm bảo backup database trước khi chạy migration trong môi trường production


## 🐛 Xử Lý Lỗi

### Lỗi "dotnet command not found"
- Đảm bảo .NET SDK đã được cài đặt và thêm vào PATH

### Lỗi "EF Core Tools not found"
- Script sẽ tự động cài đặt, nhưng nếu vẫn lỗi, chạy thủ công:
  ```cmd
  dotnet tool install --global dotnet-ef
  ```

### Lỗi "Connection string not found"
- Kiểm tra file `appsettings.json` hoặc `appsettings.Development.json`
- Đảm bảo connection string có tên là `DefaultConnection`

## 📞 Hỗ Trợ

Nếu gặp vấn đề, vui lòng kiểm tra:
1. Log output từ script
2. Connection string trong appsettings
3. .NET SDK version
4. Database server đang chạy và có thể kết nối được

