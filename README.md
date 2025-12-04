# OJT Laboratory Project - Hướng dẫn Deploy và Cấu hình

Hướng dẫn chi tiết để chạy dự án **OJT Laboratory Management System** với Docker, Nginx, Ngrok và cấu hình Database.

---

## 📋 Mục lục

1. [Yêu cầu Hệ thống](#yêu-cầu-hệ-thống)
2. [Chạy Docker với Nginx](#chạy-docker-với-nginx)
3. [Cấu hình Ngrok (Nếu cần)](#cấu-hình-ngrok-nếu-cần)
4. [Cấu hình Database trong pgAdmin 4](#cấu-hình-database-trong-pgadmin-4)
5. [Chạy Frontend](#chạy-frontend)
6. [Xử lý Lỗi](#xử-lý-lỗi)

---

## 🔧 Yêu cầu Hệ thống

Trước khi bắt đầu, đảm bảo bạn đã cài đặt:

- ✅ **Docker Desktop** - Để chạy containers
- ✅ **Docker Compose** - Để quản lý multi-container applications
- ✅ **Node.js và npm** - Để chạy Frontend (môi trường Development)
- ✅ **pgAdmin 4** - Để quản lý PostgreSQL database (tùy chọn)

---

## 🐳 Chạy Docker với Nginx

Dự án có 2 file batch để khởi động Docker:

### 1. Môi trường Development (`start_dev.bat`)

Chạy các services ở chế độ development:

```batch
start_dev.bat
```

**Lưu ý:** File này sẽ:
- Khởi động tất cả các services trong Docker
- Cấu hình Nginx cho môi trường development
- Hiển thị logs của các containers

### 2. Môi trường Production (`start_pro.bat`)

Chạy các services ở chế độ production:

```batch
start_pro.bat
```

**Lưu ý:** File này sẽ:
- Khởi động các services với cấu hình production
- Tối ưu hóa Nginx cho production
- Áp dụng các security settings

### 3. Dừng Docker Containers

Để dừng tất cả containers:

```batch
stop.bat
```

---

## 🌐 Cấu hình Ngrok (Nếu cần)

Ngrok được sử dụng để tạo public URL cho local server, hữu ích cho testing hoặc demo.

### Bước 1: Tải Ngrok

1. Truy cập: [https://ngrok.com/download](https://ngrok.com/download)
2. Tải file `ngrok.exe` cho Windows
3. Giải nén vào thư mục **Downloads** của bạn

### Bước 2: Cấu hình Auth Token

1. Mở **Command Prompt** hoặc **PowerShell**
2. Di chuyển đến thư mục Downloads:
   ```batch
   cd %USERPROFILE%\Downloads
   ```
3. Chạy lệnh cấu hình token:
   ```batch
   ngrok config add-authtoken 35q5dB6Ca7vRIDnQWdOIsOhnCGE_4KLovKHXesf4rpdAvjum
   ```

### Bước 3: Chạy Ngrok

Sau khi cấu hình token, chạy Ngrok với cổng 80:

1. Mở **Command Prompt** hoặc **PowerShell** ở thư mục Downloads
2. Chạy lệnh:
   ```batch
   ngrok http 80
   ```

3. Ngrok sẽ hiển thị một URL công khai (ví dụ: `https://xxxx-xx-xx-xx-xx.ngrok.io`)
4. URL này sẽ forward traffic đến localhost:80 của bạn

**Lưu ý:**
- Giữ cửa sổ Command Prompt mở khi đang sử dụng Ngrok
- URL sẽ thay đổi mỗi lần khởi động lại Ngrok (trừ khi dùng plan có trả phí)
- Để dừng Ngrok, nhấn `Ctrl + C` trong cửa sổ Command Prompt

---

## 🗄️ Cấu hình Database trong pgAdmin 4

### Bước 1: Mở pgAdmin 4

1. Khởi động **pgAdmin 4** trên máy của bạn
2. Đăng nhập với credentials của bạn (nếu đã setup)

### Bước 2: Tạo Server Connection mới

1. Click chuột phải vào **Servers** trong panel bên trái
2. Chọn **Register** → **Server...**

### Bước 3: Nhập thông tin Connection

Trong tab **General**:
- **Name:** `Laboratory Service Database` (hoặc tên bạn muốn)

Trong tab **Connection**, nhập thông tin sau:

```
Host: dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com
Port: 5432
Database: laboratory_service
Username: laboratory_service_user
Password: geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2
```

Trong tab **SSL**:
- **SSL mode:** `Require`
- ✅ **Trust server certificate:** Bật (checked)

### Bước 4: Lưu và Kết nối

1. Click **Save** để lưu cấu hình
2. Click vào server vừa tạo để kết nối
3. Nếu kết nối thành công, bạn sẽ thấy database `laboratory_service` trong danh sách

### Connection String (Để tham khảo)

Nếu cần sử dụng connection string trong code:

```
Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;SSL Mode=Require;Trust Server Certificate=true
```

---

## 🎨 Chạy Frontend

Frontend có thể chạy ở 2 môi trường: **Development** và **Production**.

### Môi trường Development

1. Di chuyển đến thư mục Frontend:
   ```batch
   cd OJT_Laboratory_Project\Front_End
   ```

2. Cài đặt dependencies (nếu chưa cài):
   ```batch
   npm install
   ```

3. Chạy development server:
   ```batch
   npm run dev
   ```

4. Frontend sẽ chạy tại `http://localhost:5173` (hoặc port khác nếu 5173 đã được sử dụng)

**Lưu ý:**
- Development mode hỗ trợ hot-reload
- Thay đổi code sẽ tự động refresh browser
- Sử dụng cho mục đích phát triển và testing

### Môi trường Production

Frontend production được deploy tại:

**URL:** [https://front-end-ojt.onrender.com](https://front-end-ojt.onrender.com)

**Lưu ý:**
- Production version đã được build và tối ưu hóa
- Không có hot-reload
- Sử dụng cho demo hoặc production environment

---

## 🔧 Xử lý Lỗi

### Lỗi: Docker không khởi động

**Nguyên nhân có thể:**
- Docker Desktop chưa được khởi động
- Port đã được sử dụng bởi ứng dụng khác

**Giải pháp:**
1. Kiểm tra Docker Desktop đang chạy
2. Kiểm tra port 80, 443 có đang được sử dụng không
3. Dừng các ứng dụng đang sử dụng port đó
4. Chạy lại `start_dev.bat` hoặc `start_pro.bat`

### Lỗi: Ngrok không kết nối được

**Nguyên nhân có thể:**
- Auth token chưa được cấu hình đúng
- Port 80 chưa có service nào đang chạy

**Giải pháp:**
1. Kiểm tra lại token đã được cấu hình: `ngrok config check`
2. Đảm bảo Docker đang chạy và có service lắng nghe trên port 80
3. Thử chạy lại: `ngrok http 80`

### Lỗi: Không kết nối được Database

**Nguyên nhân có thể:**
- Thông tin connection string sai
- Firewall chặn kết nối
- Database server không khả dụng

**Giải pháp:**
1. Kiểm tra lại thông tin connection trong pgAdmin 4
2. Đảm bảo SSL mode được set là `Require`
3. Kiểm tra kết nối internet
4. Thử ping đến host: `dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com`

### Lỗi: Frontend không chạy được

**Nguyên nhân có thể:**
- Node.js chưa được cài đặt
- Dependencies chưa được cài đặt
- Port đã được sử dụng

**Giải pháp:**
1. Kiểm tra Node.js: `node --version`
2. Cài đặt dependencies: `npm install`
3. Kiểm tra port 5173 có đang được sử dụng không
4. Thử chạy lại: `npm run dev`

---

## 📝 Ghi chú Quan trọng

- **Docker:** Luôn đảm bảo Docker Desktop đang chạy trước khi chạy các script start
- **Ngrok:** URL sẽ thay đổi mỗi lần khởi động lại (trừ khi dùng plan trả phí)
- **Database:** Connection string chứa thông tin nhạy cảm, không commit vào Git
- **Frontend Dev:** Sử dụng `npm run dev` cho development
- **Frontend Production:** Sử dụng URL production cho demo/testing production environment

---

## 🔗 Liên kết Hữu ích

- **Frontend Production:** [https://front-end-ojt.onrender.com](https://front-end-ojt.onrender.com)
- **Ngrok Dashboard:** [https://dashboard.ngrok.com](https://dashboard.ngrok.com)
- **Docker Documentation:** [https://docs.docker.com](https://docs.docker.com)

---

**Lưu ý:** Tài liệu này được thiết kế cho môi trường Windows. Để sử dụng trên Linux/Mac, cần điều chỉnh các lệnh tương ứng.
