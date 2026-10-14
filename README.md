# PC Builder — C# WinForms + SQL Server Express (.NET 8)

Ứng dụng desktop quản lý linh kiện, tạo cấu hình PC, kiểm tra một số điều kiện tương thích, lưu cấu hình, đăng nhập/phân quyền và wizard chẩn đoán. Có thuật toán gợi ý cấu hình theo ngân sách dùng beam search và điểm benchmark có nguồn.

## Yêu cầu
- Windows 10/11
- Visual Studio 2022 với workload **.NET desktop development**
- .NET 8 SDK
- SQL Server Express + SQL Server Management Studio (SSMS)

## Cài đặt database
Trong SSMS, chạy lần lượt:
1. `Database/01_CreateDatabase.sql` — tạo `PCBuilderDB` và các bảng.
2. `Database/02_StoredProcedures.sql` — Stored Procedures.
3. `Database/03_DemoData.sql` — dữ liệu minh họa.

Dữ liệu demo và điểm benchmark demo chỉ để thử giao diện/thuật toán; không phải giá thị trường hoặc kết quả benchmark đo thực. Hãy xác minh thông số và nhập benchmark có nguồn trước khi dùng để tư vấn mua linh kiện.

## Kết nối SQL Server Express
Mặc định trong `PCBuilder.WinForms/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PCBuilderDB": "Server=localhost\\SQLEXPRESS;Database=PCBuilderDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;"
  }
}
```
Nếu instance của bạn có tên khác, hãy đổi `Server=...`. Ví dụ instance mặc định có thể là `localhost` hoặc `localhost\SQLEXPRESS`. Tài khoản Windows phải có quyền truy cập database.

## Mở và chạy
1. Mở `PCBuilderProject.sln`.
2. Kiểm tra `PCBuilder.WinForms` Target Framework là `net8.0-windows`.
3. Restore NuGet packages và đặt `PCBuilder.WinForms` làm Startup Project.
4. Chạy (F5). Khi chưa có Admin, dùng chức năng **Tạo Admin đầu tiên** trên màn hình đăng nhập.


## Các tính năng chính trong ứng dụng
1. **Đăng nhập và phân quyền:** tài khoản Admin đã tồn tại đăng nhập như bình thường; User chỉ có quyền sử dụng chức năng dành cho người dùng.
2. **Đăng ký User:** form đăng ký riêng thu thập username, họ tên, email và mật khẩu. Vai trò được gán cố định ở tầng repository là `User`; không cho tự chọn Admin. Mật khẩu lưu dạng PBKDF2 hash.
3. **Dashboard:** trang tổng quan có điều hướng đến các công cụ chính.
4. **Tra cứu linh kiện:** lọc theo danh mục và tìm theo tên/hãng/SKU; Admin có thêm/sửa/ngừng kinh doanh.
5. **Build PC:** thêm/bỏ linh kiện, xem tổng giá, kiểm tra tương thích, lưu cấu hình cá nhân.
6. **Gợi ý theo ngân sách:** chọn mục đích Gaming, Lập trình/Đồ họa hoặc Cân bằng; engine xếp hạng các tổ hợp từ dữ liệu benchmark hiện có.
7. **Troubleshooting Wizard:** trả lời từng câu hỏi để nhận hướng chẩn đoán ban đầu.
8. **Giao diện:** sidebar tối, dashboard sáng, thẻ tính năng, màu sắc thống nhất cho bảng, nút và trường nhập liệu; không cần thư viện UI bên ngoài.

## Gợi ý cấu hình
- Chọn mục đích: Gaming, Lập trình/Đồ họa hoặc Cân bằng.
- Nhập ngân sách bằng VND.
- Engine sinh tổ hợp theo các nhóm CPU, Mainboard, RAM, GPU, PSU, Storage và Case; lọc socket, loại RAM, chiều dài GPU và mức dự phòng PSU; sau đó xếp hạng theo benchmark và hiệu năng/giá.
- Tối đa 18 ứng viên mỗi nhóm và beam width 250 để giới hạn số tổ hợp.
- Engine hiện yêu cầu đủ 7 nhóm linh kiện và dữ liệu kỹ thuật cần thiết để kiểm tra tương thích. Cooling chưa bắt buộc trong tổ hợp nền tảng.

## Thêm benchmark có nguồn
Bảng `dbo.BenchmarkScores` có các trường `ComponentID`, `MetricCode`, `BenchmarkName`, `Score`, `SourceName`, `SourceUrl`, `IsVerified`. Các metric hiện được truy vấn gồm `CPU_MULTI`, `CPU_SINGLE`, `GPU_GAMING`, `GPU_PRODUCTIVITY`. Không trộn điểm từ các bài benchmark khác thang đo. Để có so sánh hợp lệ, hãy thống nhất benchmark/metric giữa các linh kiện cùng loại. Các điểm demo được đánh dấu `IsVerified=0`.

## Kiến trúc
- `Forms`: Presentation Layer (WinForms).
- `Services`: Business Logic, tương thích, gợi ý, troubleshooting.
- `Data`: ADO.NET repositories, parameterized SQL / Stored Procedures.
- `Models`: model dữ liệu.
- `Database`: schema, stored procedures và demo data.

## Giới hạn hiện tại
- Điểm bottleneck không phải benchmark game thực tế; TDP không đại diện cho hiệu năng.
- Kiểm tra tương thích chỉ áp dụng các điều kiện có dữ liệu: socket, DDR, GPU-case, PSU. Cần bổ sung BIOS CPU support, clearance radiator/cooler, đầu cấp nguồn GPU, lane/M.2 và kích thước PSU để đạt độ bao phủ cao hơn.
- Giá và tồn kho demo là dữ liệu minh họa.
- Đây là MVP học tập; cần kiểm thử trên máy Windows + SQL Server Express trước khi nộp/triển khai.


## Giao diện và đăng ký tài khoản
- Giao diện dùng bộ màu tối cho sidebar, nền sáng, thẻ dashboard và bảng dữ liệu được định dạng lại bằng WinForms thuần; không cần thư viện giao diện trả phí.
- Màn hình Login không còn nút tạo Admin. Người dùng mới đăng ký qua form riêng và `AuthRepository.RegisterUser` luôn truyền `UserRole = User`.
- Quyền Admin vẫn lấy từ database; không có trường cho người dùng tự chọn vai trò khi đăng ký.
