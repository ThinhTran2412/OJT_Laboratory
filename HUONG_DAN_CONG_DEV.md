# Hướng Dẫn Các Cổng Ra - Môi Trường Development

## Tổng Quan
Tài liệu này mô tả các cổng (ports) và URL được sử dụng trong môi trường Development.

## Các Service và Cổng

### 1. Nginx Reverse Proxy
- **Port**: 80 (HTTP), 443 (HTTPS)

### 2. IAM Service
- **REST API**: http://localhost/api/iam
- **Swagger UI (Direct)**: http://localhost:5001/swagger
- **Health Check**: http://localhost/api/iam/health
- **Internal Port**: 8080 (REST API), 8081 (gRPC)
- **Direct Access Port**: 5001

### 3. Laboratory Service
- **REST API**: http://localhost/api/laboratory
- **Swagger UI (Direct)**: http://localhost:5002/swagger
- **Health Check**: http://localhost/api/laboratory/health
- **Internal Port**: 8080 (REST API), 8081 (gRPC)
- **Direct Access Port**: 5002

### 4. Monitoring Service
- **REST API**: http://localhost/api/monitoring
- **Swagger UI (Direct)**: http://localhost:5003/swagger
- **Health Check**: http://localhost/api/monitoring/health
- **Internal Port**: 8080 (REST API)
- **Direct Access Port**: 5003

### 5. Simulator Service
- **REST API**: http://localhost/api/simulator
- **Swagger UI (Direct)**: http://localhost:5004/swagger
- **Health Check**: http://localhost/api/simulator/health
- **Internal Port**: 8080 (REST API), 8081 (gRPC)
- **Direct Access Port**: 5004

### 6. RabbitMQ
- **Management UI**: http://localhost:15672
- **AMQP Port**: 5672
- **Management Port**: 15672
- **Default Credentials**:
  - Username: `guest`
  - Password: `guest`

## Cách Sử Dụng

### Khởi động môi trường Development:
```bash
start_dev.bat
```

### Dừng môi trường:
```bash
stop.bat
```

### Xem logs:
```bash
docker-compose logs -f
```

### Xem logs của một service cụ thể:
```bash
docker-compose logs -f iam-service
docker-compose logs -f laboratory-service
docker-compose logs -f monitoring-service
docker-compose logs -f simulator-service
```

## Lưu Ý

1. **Swagger**: Chỉ có sẵn trong môi trường Development. Trong Production, Swagger sẽ bị tắt.

2. **Health Checks**: Tất cả các service đều có endpoint `/health` để kiểm tra trạng thái.

3. **gRPC**: Các service gRPC chỉ có thể truy cập từ bên trong Docker network, không expose ra ngoài qua Nginx.

4. **Database**: Database được cấu hình trong `appsettings.Development.json` của từng service.

5. **CORS**: Các origin được phép truy cập được cấu hình trong `appsettings.json` của từng service.

## Troubleshooting

### Kiểm tra container đang chạy:
```bash
docker-compose ps
```

### Kiểm tra logs lỗi:
```bash
docker-compose logs --tail=100
```

### Restart một service cụ thể:
```bash
docker-compose restart iam-service
```

### Rebuild và restart:
```bash
docker-compose up -d --build
```

