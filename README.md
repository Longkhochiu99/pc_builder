# PC Builder — WinForms .NET 8 + SQL Server

Phiên bản này chuyển giao diện chính của các Form sang cấu trúc WinForms Designer chuẩn để có thể chỉnh sửa bằng Visual Studio 2022. Database và các lớp nghiệp vụ hiện có được giữ nguyên.

## 1. Yêu cầu

- Windows 10/11.
- Visual Studio 2022 với workload **.NET desktop development**.
- .NET 8 SDK/Windows Desktop Runtime phù hợp.
- SQL Server Express đang chạy, mặc định là `localhost\\SQLEXPRESS`.
- Database `PCBuilderDB` và các stored procedure đã được tạo theo bộ script trong thư mục `Database`.

## 2. Mở và chạy project

1. Giải nén toàn bộ ZIP vào thư mục bình thường, không chạy trực tiếp trong ZIP.
2. Mở `PCBuilderProject.sln` bằng Visual Studio 2022.
3. Trong Solution Explorer, nhấp phải project `PCBuilder.WinForms` → **Set as Startup Project**.
4. Mở `PCBuilder.WinForms/appsettings.json` và kiểm tra connection string. Mặc định:

   ```json
   {
     "ConnectionStrings": {
       "PCBuilderDB": "Server=localhost\\\\SQLEXPRESS;Database=PCBuilderDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;"
     }
   }
   ```

   Trong file JSON thực tế, dấu `\\` được escape như trên. Nếu SQL Server instance hoặc tên database của bạn khác, chỉ sửa giá trị connection string; không cần thay đổi schema.
5. Chọn cấu hình `Debug` và `Any CPU`, sau đó nhấn **F5**.
6. Đăng nhập bằng tài khoản đã có trong database. Đăng ký công khai luôn tạo role `User`; không dùng chức năng đăng ký để tạo Admin.

## 3. Mở WinForms Designer

Trong **Solution Explorer**, mở `PCBuilder.WinForms > Forms`. Các cặp file giao diện/logic gồm:

| Form | File Designer | Mục đích |
|---|---|---|
| Đăng nhập | `LoginForm.Designer.cs` | Đăng nhập và mở màn hình đăng ký |
| Đăng ký | `RegisterForm.Designer.cs` | Tạo tài khoản User |
| Màn hình chính | `MainForm.Designer.cs` | Khung sidebar, tiêu đề trang và vùng nội dung |
| Danh mục linh kiện | `ComponentsForm.Designer.cs` | Tìm kiếm, lọc và bảng linh kiện |
| Thêm/sửa linh kiện | `ComponentEditForm.Designer.cs` | Biểu mẫu chỉnh sửa thông tin linh kiện |
| Build PC | `BuildForm.Designer.cs` | Danh mục, cấu hình đang chọn, ngân sách và kết quả |
| Chẩn đoán | `TroubleshootingForm.Designer.cs` | Câu hỏi, câu trả lời và vùng kết luận |

Để chỉnh sửa trực quan:

1. Mở file `.cs` tương ứng trong Solution Explorer.
2. Nhấp phải file `.cs` (ví dụ `BuildForm.cs`) → **View Designer**. Có thể dùng `Shift+F7` khi đang mở file code.
3. Chọn control trong Designer hoặc Document Outline, thay đổi thuộc tính trong cửa sổ **Properties**.
4. Đặt tên control trong thuộc tính **(Name)**. Các control đã được dùng trong logic có tên bắt đầu bằng `_`, ví dụ `_search`, `_grid`, `_budget`; không đổi tên chúng nếu chưa cập nhật code-behind.
5. Lưu file và build lại.

**Lưu ý về các phần tạo động:** sidebar trong `MainForm` được nạp theo quyền User/Admin; thẻ tổng quan được tạo trong code; các nút lựa chọn trong `TroubleshootingForm` được tạo theo cây câu hỏi. Các phần này thay đổi theo dữ liệu/quyền nên vẫn được dựng bằng code, trong khi khung Form và các control nền có thể chỉnh sửa trong Designer. Hộp thoại nhập tên cấu hình nhỏ trong `BuildForm` cũng được tạo động.

## 4. Cấu trúc source

```text
PCBuilderProject_NET8/
├── PCBuilderProject.sln
├── Database/
│   ├── 01_CreateDatabase.sql
│   ├── 02_StoredProcedures.sql
│   └── 03_DemoData.sql
└── PCBuilder.WinForms/
    ├── Program.cs
    ├── appsettings.json
    ├── Data/
    │   ├── Db.cs
    │   ├── AuthRepository.cs
    │   ├── ComponentRepository.cs
    │   └── BuildRepository.cs
    ├── Models/
    │   ├── AppUser.cs
    │   └── ComponentItem.cs
    ├── Services/
    │   ├── CompatibilityService.cs
    │   ├── RecommendationService.cs
    │   └── TroubleshootingService.cs
    └── Forms/
        ├── *Form.cs              # sự kiện và logic giao diện
        ├── *Form.Designer.cs     # khởi tạo control, layout và thuộc tính
        └── UiTheme.cs            # màu sắc/style dùng chung
```

### Vai trò các lớp

- `Program.cs`: điểm khởi chạy ứng dụng, mở `LoginForm`.
- `Db.cs`: tạo kết nối SQL Server theo cấu hình.
- `AuthRepository.cs`: đăng nhập, xác thực mật khẩu và đăng ký tài khoản User.
- `ComponentRepository.cs`: tìm kiếm, đọc, thêm, sửa và ngừng kinh doanh linh kiện; tải danh mục/hãng.
- `BuildRepository.cs`: lưu cấu hình PC và các linh kiện thuộc cấu hình trong transaction.
- `AppUser.cs`: thông tin người dùng và quyền Admin/User.
- `ComponentItem.cs`: mô hình dữ liệu linh kiện.
- `CompatibilityService.cs`: kiểm tra tương thích phần cứng và ước lượng giới hạn cấu hình dựa trên dữ liệu có sẵn.
- `RecommendationService.cs`: tạo/xếp hạng gợi ý cấu hình theo ngân sách và mục đích.
- `TroubleshootingService.cs`: cây câu hỏi chẩn đoán sự cố.
- `UiTheme.cs`: style dùng chung cho form, nút, bảng và textbox.

## 5. Luồng chức năng

### Đăng nhập/đăng ký

`LoginForm` gọi `AuthRepository.Login`. Nếu hợp lệ, ứng dụng mở `MainForm` với `AppUser`. `RegisterForm` kiểm tra trường bắt buộc, độ dài mật khẩu và xác nhận mật khẩu rồi gọi `RegisterUser`. Role được đặt thành `User` ở tầng repository.

### Tra cứu và quản trị linh kiện

`ComponentsForm` tải danh mục/hãng, lọc theo từ khóa và danh mục. Với người dùng Admin, các nút thêm/sửa/ngừng kinh doanh được hiển thị. `ComponentEditForm` thu thập các trường model, SKU, giá, tồn kho, danh mục, hãng và trạng thái, sau đó gọi `ComponentRepository.Save`.

### Build PC

`BuildForm` cho phép tìm linh kiện, thêm/bỏ linh kiện, xem tổng giá, kiểm tra tương thích, gợi ý cấu hình theo ngân sách/mục đích và lưu cấu hình. `CompatibilityService` kiểm tra các thông tin tương thích có trong database. Điểm hiệu năng/giá trong `RecommendationService` phụ thuộc chất lượng dữ liệu benchmark; dữ liệu demo không phải benchmark độc lập đã xác minh.

### Chẩn đoán sự cố

`TroubleshootingForm` hiển thị từng câu hỏi từ `TroubleshootingService`, giữ lịch sử để quay lại và trình bày kết luận/hướng khắc phục khi tới nút cuối của cây.

## 6. Database — không tạo lại nếu đã có

Nếu database `PCBuilderDB` đã tồn tại và ứng dụng hiện kết nối được, **không chạy lại** script tạo database hoặc demo data chỉ vì chuyển giao diện. Thư mục `Database` được giữ lại để tham khảo/cài đặt trên môi trường mới. Tránh chạy `03_DemoData.sql` nhiều lần nếu chưa kiểm tra các ràng buộc duy nhất và dữ liệu hiện có.

Nếu gặp lỗi kết nối:

1. Mở SQL Server Configuration Manager/Services và kiểm tra instance `SQLEXPRESS` đang chạy.
2. Trong SSMS, xác nhận server name là `localhost\\SQLEXPRESS` (hoặc thay đúng instance của máy bạn).
3. Xác nhận Windows account chạy ứng dụng có quyền truy cập `PCBuilderDB`.
4. Kiểm tra stored procedures mà các repository gọi đã tồn tại.

## 7. Build và xử lý lỗi

- Chọn **Build → Rebuild Solution**.
- Nếu thiếu target framework, cài .NET 8 SDK và workload **.NET desktop development**.
- Nếu Visual Studio báo designer bị lỗi, xem **Error List** và **Output → Windows Forms Designer**; kiểm tra constructor design-time của Form không thực hiện truy vấn DB.
- Không viết truy vấn SQL hoặc logic nghiệp vụ vào file `.Designer.cs`. Hãy đặt logic trong file `.cs` tương ứng.
- Không chỉnh sửa trực tiếp phần `InitializeComponent()` bằng code khi đang mở Designer nếu có thể chỉnh qua cửa sổ Properties; Designer có thể ghi đè các thay đổi thủ công.

## 8. Giới hạn kiểm thử của gói này

Source đã được chuyển sang cấu trúc partial class với các file `.Designer.cs` và có constructor không truy cập database dành cho Designer. Môi trường tạo gói hiện tại không có Windows/.NET SDK để chạy build WinForms, vì vậy vẫn cần mở bằng Visual Studio 2022 trên Windows và thực hiện **Rebuild Solution** trước khi sử dụng. Không coi việc đóng gói ZIP là bằng chứng đã chạy kiểm thử trên Windows.
