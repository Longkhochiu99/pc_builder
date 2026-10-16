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

### Tra cứu và quản trị linh kiện (CRUD Components)

Phần quản lý linh kiện đã có đủ luồng **Create, Read, Update và Delete theo kiểu ngừng kinh doanh (soft delete)**. Chức năng thêm/sửa/ngừng kinh doanh chỉ được mở khi tài khoản hiện tại có role `Admin` và màn hình được mở ở chế độ quản trị.

#### 1. Read — xem và tìm kiếm

- Form: `Forms/ComponentsForm.cs`.
- Repository: `Data/ComponentRepository.cs`, phương thức `Search(keyword, categoryId, includeInactive)`.
- Stored procedure: `dbo.sp_Component_Search`.
- Người dùng có thể nhập từ khóa rồi bấm **Tìm kiếm** và lọc theo danh mục. Từ khóa được tìm trong tên model, SKU, tên hãng và tên danh mục.
- Chế độ tra cứu thông thường chỉ hiển thị linh kiện đang hoạt động (`IsActive = 1`). Chế độ quản trị truyền `includeInactive = true` để xem cả linh kiện đã ngừng kinh doanh.
- Các cột kỹ thuật dùng nội bộ để kiểm tra tương thích được ẩn khỏi bảng; việc ẩn cột không xóa dữ liệu trong database.

#### 2. Create — thêm linh kiện

1. Đăng nhập bằng tài khoản Admin.
2. Mở **Quản lý linh kiện** ở thanh điều hướng.
3. Bấm **Thêm**, điền danh mục, hãng, tên model, SKU (nếu có), giá và số lượng tồn kho.
4. Bấm **Lưu**.

`ComponentsForm.AddClick()` mở `ComponentEditForm` với `ComponentID = 0`. Khi lưu, `ComponentEditForm.Save()` tạo `ComponentItem` và gọi `ComponentRepository.Save()`. Repository nhận diện ID bằng 0 và gọi `dbo.sp_Component_Create`; SQL Server tự sinh `ComponentID`.

#### 3. Update — sửa linh kiện

1. Chọn một dòng trong bảng.
2. Bấm **Sửa**.
3. Thay đổi thông tin rồi bấm **Lưu**.

`ComponentsForm.EditClick()` truyền linh kiện đang chọn vào `ComponentEditForm`. Form điền các giá trị hiện tại vào control. Khi lưu, `ComponentRepository.Save()` thấy `ComponentID != 0` nên gọi `dbo.sp_Component_Update`, cập nhật danh mục, hãng, model, SKU, giá, tồn kho và trạng thái `IsActive`.

#### 4. Delete — ngừng kinh doanh, không xóa vật lý

1. Chọn linh kiện trong bảng quản trị.
2. Bấm **Ngừng kinh doanh** (nút trong giao diện có thể mang nhãn ngắn là **Xóa**).
3. Xác nhận hộp thoại.

`ComponentsForm.DeleteClick()` gọi `ComponentRepository.Delete(id)`, sau đó repository chạy `dbo.sp_Component_Delete`. Stored procedure chỉ đặt `IsActive = 0` và cập nhật `UpdatedAt`; bản ghi không bị xóa vật lý. Cách này giữ lịch sử cấu hình đã lưu và tránh làm hỏng khóa ngoại. Linh kiện ngừng kinh doanh sẽ không xuất hiện trong danh sách thông thường, nhưng vẫn có thể xem trong chế độ quản trị.

#### Các lớp và stored procedure liên quan

| Thành phần | Trách nhiệm |
|---|---|
| `ComponentsForm.cs` | Tải bảng, tìm kiếm, chọn dòng, mở form thêm/sửa, xác nhận ngừng kinh doanh |
| `ComponentsForm.Designer.cs` | Bố cục bảng, bộ lọc và các nút thao tác |
| `ComponentEditForm.cs` | Đổ dữ liệu lên form, kiểm tra các trường bắt buộc và xử lý lưu |
| `ComponentEditForm.Designer.cs` | Các control nhập thông tin linh kiện |
| `ComponentRepository.Search()` | Đọc danh sách và lọc linh kiện |
| `ComponentRepository.Save()` | Chọn stored procedure tạo mới hay cập nhật dựa trên `ComponentID` |
| `ComponentRepository.Delete()` | Gọi stored procedure ngừng kinh doanh |
| `sp_Component_Search` | Truy vấn danh sách, từ khóa, danh mục và trạng thái hoạt động |
| `sp_Component_Create` | Thêm bản ghi linh kiện |
| `sp_Component_Update` | Cập nhật bản ghi linh kiện |
| `sp_Component_Delete` | Đặt `IsActive = 0`, không xóa vật lý |

**Giới hạn của form hiện tại:** form quản lý mới cho phép sửa các trường chung gồm danh mục, hãng, model, SKU, giá, tồn kho và trạng thái. `Description`, `ImageURL` và thông số chuyên biệt trong các bảng như `CPU_Specs`, `GPU_Specs`, `RAM_Specs` chưa được chỉnh sửa từ form này. `ComponentRepository.Save()` hiện truyền `NULL` cho `Description` và `ImageURL`; không dùng form hiện tại để sửa các trường đó nếu cần bảo toàn giá trị đang có. Muốn quản trị đầy đủ các thông số này cần bổ sung control và cập nhật repository riêng.

### Quản lý tài khoản (Users) — phạm vi hiện có

**Lưu ý quan trọng: source hiện tại chưa có màn hình CRUD người dùng dành cho Admin.** README ghi rõ phạm vi thực tế để tránh nhầm với chức năng đã được triển khai.

- **Create — có:** `RegisterForm.cs` kiểm tra thông tin bắt buộc, mật khẩu tối thiểu 8 ký tự và xác nhận mật khẩu, rồi gọi `AuthRepository.RegisterUser()`. Repository tạo hash mật khẩu bằng PBKDF2-SHA256 (210.000 vòng lặp, salt ngẫu nhiên) và gọi `dbo.sp_User_Create`. Role được gán cố định là `User`, vì vậy không thể tự đăng ký thành Admin. Username và email trùng sẽ được báo lỗi.
- **Read — có giới hạn:** `AuthRepository.Login()` gọi `dbo.sp_User_GetForLogin` để tìm một tài khoản theo username và chỉ lấy tài khoản có `IsActive = 1`. Đây là tra cứu phục vụ đăng nhập, **không phải** danh sách quản trị tất cả người dùng.
- **Update — chưa có giao diện/logic:** chưa có form hoặc repository method để Admin sửa họ tên, email, role hay trạng thái tài khoản.
- **Delete/Deactivate — chưa có giao diện/logic:** chưa có chức năng khóa/ngừng hoạt động hoặc xóa tài khoản từ ứng dụng. Bảng `Users` có cột `IsActive`, nhưng điều đó không đồng nghĩa UI đã triển khai thao tác này.

Các file liên quan đến phần tài khoản hiện có:

| File/thành phần | Vai trò |
|---|---|
| `Forms/RegisterForm.cs` | Kiểm tra dữ liệu đăng ký và thông báo kết quả |
| `Forms/RegisterForm.Designer.cs` | Giao diện đăng ký |
| `Forms/LoginForm.cs` | Nhận username/password và gọi xác thực |
| `Data/AuthRepository.cs` | `Login()` và `RegisterUser()` |
| `PasswordHash.Create()` / `Verify()` | Tạo và xác minh hash mật khẩu |
| `dbo.sp_User_Create` | Thêm tài khoản vào `Users` |
| `dbo.sp_User_GetForLogin` | Tìm tài khoản đang hoạt động theo username |
| `Models/AppUser.cs` | Dữ liệu tài khoản hiện đăng nhập và kiểm tra role Admin/User |

Nếu yêu cầu bài tập là **CRUD User đầy đủ**, cần phát triển thêm một màn hình quản trị người dùng và các phương thức repository/stored procedure cho danh sách, cập nhật và khóa tài khoản. Phần mở rộng đó chưa được đưa vào bản source này; database hiện có không bị thay đổi bởi phần hướng dẫn này.

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
