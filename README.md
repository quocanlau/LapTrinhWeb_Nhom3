# Autora Bản hoàn thiện kỹ thuật

Website ASP.NET Core MVC .NET 8, EF Core 8.0.11, SQL Server và Identity. Cọc bằng ROUND(giá bán chốt × 0.02, 0) VND; xe đang giữ hoặc đã cọc không nhận yêu cầu mới. Không có cổng thanh toán ngân hàng tự động.

## 1 Chuẩn bị trên Windows

- Cài **.NET 8 SDK**, không chỉ .NET Runtime. Kiểm tra bằng `dotnet --list-sdks` trong PowerShell; cần có phiên bản 8.x.
- Cài SQL Server LocalDB (có thể qua Visual Studio Installer, workload ASP.NET và thành phần SQL Server Express LocalDB) hoặc SQL Server Express/Developer. SSMS là công cụ quản lý, không thay thế SQL Server engine.
- Nếu dùng LocalDB, mở PowerShell và chạy `sqllocaldb info`. Instance mặc định là `MSSQLLocalDB`; có thể khởi động bằng `sqllocaldb start MSSQLLocalDB`.
- Có Internet để restore NuGet. Bootstrap và jQuery đã dùng file local; font Google, Bootstrap Icons, bản đồ/ảnh URL ngoài cần Internet để hiển thị đầy đủ. Ứng dụng vẫn có font hệ thống dự phòng.
- Visual Studio cần hỗ trợ .NET 8 nếu mở solution. Có thể chạy toàn bộ bằng PowerShell, không cần IIS.

## 2 Mở đúng thư mục và build

Giải nén vào thư mục riêng. Mở thư mục `Autora_NopBai` trong File Explorer, bấm thanh địa chỉ, nhập `powershell` và Enter. Thư mục hiện tại phải có `02_SourceCode`, `03_Database`, `04_HuongDanCaiDat`.

```powershell
Set-Location .\02_SourceCode
dotnet --list-sdks
dotnet restore .\Autora.sln
dotnet tool restore
dotnet build .\Autora.sln
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

`dotnet-ef` đã có manifest trong `02_SourceCode/.config`, không cần tự cài một bản major khác.

## 3 Đặt kết nối DB

Với LocalDB của Windows:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True" --project .\Autora\Autora.csproj
```

Với SQL Server Express, thay server thành `localhost\SQLEXPRESS` hoặc đúng tên instance trong SSMS. Với SQL Authentication, dùng tài khoản được cấp quyền và lưu mật khẩu trong user-secrets; không đưa mật khẩu thật lên GitHub.

`appsettings.Development.json` có cấu hình LocalDB mẫu; user-secrets ở Development ghi đè cấu hình này. Thư mục project và DB là hai nơi độc lập. Giải nén code mới không tự chuyển hoặc xóa dữ liệu xe trong SQL Server.

## 4 Dựng DB mới hoặc cập nhật DB cũ

### Đang có DB của nhóm

Dừng ứng dụng, tạo bản sao lưu DB bằng SSMS, giữ nguyên kết nối tới DB cũ rồi chạy:

```powershell
dotnet ef database update --project .\Autora\Autora.csproj
```

Migration `CompleteShowroomWorkflow` thêm bảng cấu hình showroom, yêu cầu tư vấn và mở rộng trạng thái yêu cầu Đang xử lý. Không xóa xe, tài khoản hay giao dịch cũ. Không chạy `database drop`, không xóa migration cũ và không tạo Initial mới.

### Máy mới chưa có DB

Cũng chạy lệnh `database update` bên trên. EF tạo DB và các bảng theo migration. Sau đó thực hiện bước 5 để tạo admin và dữ liệu demo.

### Dùng SSMS thay EF

Mở `03_Database/AutoraDb.sql` từ thư mục gốc gói nộp. File này dựng/cập nhật database **AutoraDb** theo lịch sử migration. Nếu DB tên khác, ưu tiên EF với chuỗi kết nối đúng tên; không chạy nhầm câu USE.

`03_Database/CapNhatBanCu.sql` chỉ dành cho DB đã có migration AddAdminNotificationReads. DB mới nên dùng full script hoặc EF. Chọn một cách áp dụng; không cần dựng DB hai lần. SQL chỉ chứa schema, không chứa mật khẩu hay dữ liệu thực của nhóm.

## 5 Tạo admin và dữ liệu demo trên DB mới

```powershell
dotnet user-secrets set "Bootstrap:AdminEmail" "admin@autora.local" --project .\Autora\Autora.csproj
dotnet user-secrets set "Bootstrap:AdminPassword" "THAY_BANG_MAT_KHAU_RIENG" --project .\Autora\Autora.csproj
dotnet user-secrets set "Seed:IncludeDemoData" "true" --project .\Autora\Autora.csproj
dotnet user-secrets set "Seed:DemoCarCount" "100" --project .\Autora\Autora.csproj
dotnet run --project .\Autora\Autora.csproj --launch-profile http -- --seed
```

Thay `THAY_BANG_MAT_KHAU_RIENG` trước khi chạy: tối thiểu 8 ký tự, có chữ hoa, thường, số, ký tự đặc biệt. Seed tạo role và admin; **không đổi mật khẩu admin đã tồn tại**. Sau khi tạo, có thể xóa Bootstrap:AdminPassword khỏi user-secrets.

Nếu bảng Cars đã có xe, seed không chép đè hoặc thêm 100 xe vào dữ liệu thực. Bộ demo trên DB rỗng là 100 hồ sơ Camry mô phỏng với VIN giả để thử phân trang/hiệu năng; không phải bảng báo giá thực tế hoặc dữ liệu khảo sát. Muốn chuyển nguyên dữ liệu thực của nhóm, cần backup/restore SQL Server riêng rồi cấu hình đúng connection string.

## 6 Chạy website

```powershell
dotnet run --project .\Autora\Autora.csproj --launch-profile http
```

Mở địa chỉ in ở PowerShell, thường là `http://localhost:5241`. Để dừng, nhấn Ctrl+C. Sau khi thay CSS/JS nhấn Ctrl+F5 trong trình duyệt. Muốn chạy HTTPS trên máy học, có thể dùng profile https và `dotnet dev-certs https --trust` khi cần.

## 7 Các chức năng của bản này

- `/Account/Profile`, `/Account/ChangePassword`: hồ sơ và đổi mật khẩu; fullname/phone đồng bộ với hồ sơ khách.
- `/Car`: hãng, dòng, khoảng giá, năm, nhiên liệu, hộp số, kiểu dáng, tình trạng, sắp xếp và AJAX. Footer Sedan/SUV lọc BodyType đúng trường.
- `/Compare`: tối đa 3 xe trong Session; giỏ quan tâm `/Cart` vẫn riêng, tối đa 20 xe. Cả hai không giữ xe.
- `/Admin/Customer`, `/Admin/Supplier`, `/Admin/Manufacturer`: quản lý khách mua trực tiếp, đối tác, hãng và dòng. Không xóa hồ sơ đã có chứng từ hoặc tài khoản liên kết.
- `/Admin/Car`: VIN, năm, giá nguyên VND, dòng hợp lệ và RowVersion. Quy ước xe mới không quá 100 km trong bản học tập; ghi rõ quy ước này trong báo cáo.
- `/Admin/CarImage?id=...`: thêm URL hoặc upload PNG/JPEG/WebP, tối đa 5 MB/ảnh và 12 ảnh/xe; chọn ảnh chính, đổi thứ tự, xóa liên kết. Kiểm tra signature và đuôi file, sinh tên GUID; không nhận SVG/executable. Xóa liên kết không tự xóa file gốc để tránh mất ảnh dùng chung.
- `/Admin/Purchase`: lưu nháp nhiều xe, sửa/bỏ dòng, xóa nháp, xác nhận tất cả trong một transaction. Phiếu đã xác nhận khóa sửa/xóa.
- `/Admin/Order`: nhận yêu cầu, Đang xử lý, giữ xe từ yêu cầu hoặc lập giao dịch nháp cho khách trực tiếp; nháp chưa khóa xe. Thu/hoàn, chặn thu vượt, hủy sau hoàn đủ và bàn giao khi đủ giá bán.
- `/Admin/User`: tạo Staff/Admin với mật khẩu riêng, đổi vai trò, khóa/mở, đặt mật khẩu mới. Tài khoản có chứng từ chỉ vô hiệu hóa, không thay người lập cũ. Kiểm tra stamp mỗi request để khóa/đổi quyền có hiệu lực ở request sau.
- `/Admin/Home`: khoảng ngày UTC+7, tồn hiện tại, doanh thu theo ngày bàn giao, giá trị nhập, thu trừ hoàn, lãi gộp và phải thu hiện tại của giao dịch xác nhận trong kỳ.
- `/Admin/Setting`: cấu hình được lưu DB, chỉ Admin. Cọc vẫn 2%, không có ô cọc 50 triệu giả.
- `/Contact`: yêu cầu tư vấn/báo giá lưu DB; Staff xem `/Admin/Contact`. Gửi tối đa 5 lần/phút/IP, không tự gửi email.
- Chuông quản trị: hoạt động 30 ngày, polling 30 giây, trạng thái đọc theo từng người; liên kết tới đúng giao dịch dù danh sách phân trang.

Hủy nháp không làm thay đổi xe đang bị giao dịch khác giữ. Hoàn một phần xuống dưới mức cọc chuyển về Đang giữ, vẫn không nhận giao dịch khác. Không hỗ trợ mất cọc/phạt hủy hoặc tự hết hạn giữ xe trong bản này.

## 8 API

- `GET /api/cars?manufacturerId=1&carModelId=1&minPrice=...&maxPrice=...&yearFrom=...&yearTo=...&page=1&pageSize=12`; alias `brandId` cũng được nhận.
- `GET /api/cars/{id}`.
- `GET/POST /api/manufacturers`, `GET/PUT/DELETE /api/manufacturers/{id}`; `/api/brands` là alias. Ghi chỉ Admin.
- `POST /api/purchase-requests` dành cho Customer; JSON ví dụ `{"carId":1,"note":"Xin tư vấn"}`. CustomerId lấy từ tài khoản.
- `GET /api/purchase-requests/{id}`: chủ sở hữu hoặc Staff; không có quyền trả 404.

API ghi dùng cookie đăng nhập và header `X-CSRF-TOKEN` lấy từ meta csrf-token của trang đã đăng nhập. POST thiếu token trả 400; chưa đăng nhập API trả 401, thiếu role trả 403. Không gửi mật khẩu/giá nhập/thông tin riêng khách trong API xe công khai.

## 9 Kiểm thử

```powershell
dotnet test .\Autora.Tests\Autora.Tests.csproj
```

Bản đóng gói được kiểm tra: web build 0 lỗi/0 cảnh báo, 50 test thành công và 1 test SQL Server chưa chạy. Bộ test bổ sung dùng SQLite in-memory và ASP.NET TestServer để kiểm tra service và HTTP qua view, xác thực, anti-CSRF, API, nhập–bán–thu tiền–bàn giao. Trong adapter test, tiền nguyên VND được map long, thời gian map UTC ticks và RowVersion được mô phỏng; **production vẫn SQL Server decimal(18,2), datetimeoffset và rowversion**.

Để chạy test SQL Server thực trên máy có instance và quyền tạo DB:

```powershell
$env:AUTORA_TEST_SQLSERVER = "Server=(localdb)\MSSQLLocalDB;Trusted_Connection=True;TrustServerCertificate=True"
dotnet test .\Autora.Tests\Autora.Tests.csproj --filter FullyQualifiedName~SqlServerIntegrationTests
Remove-Item Env:AUTORA_TEST_SQLSERVER
```

Test tạo DB tên `Autora_Integration_<guid>` và chỉ xóa DB test do chính nó tạo sau khi chạy; không dùng bảng của AutoraDb. Test này kiểm tra migration, giữ trùng, thu đồng thời và khóa cấu hình. Chưa chạy trong môi trường đóng gói vì không có SQL Server.

Cần nhóm tự kiểm tra DB hiện có, trình duyệt ở 375/768/1366 px, i18n và đo phản hồi với 100 xe trên cấu hình máy demo. Chưa có số đo hiệu năng hoặc ảnh chụp UI được xác nhận trong lần sửa này; không tự điền Pass cho các mục đó.

## 10 Tài liệu và phạm vi

Đọc `05_KiemTra/DOI_CHIEU_BAN_CUOI.md` và `05_KiemTra/KICH_BAN_NGHIEM_THU.md`. Bản Word nhóm trong 01_BaoCao được giữ làm bản gốc; cần chốt tên/MSSV, ảnh UI thật, số test mới và kết quả SQL Server trước khi nộp. Không sử dụng các câu “100% xuất sắc” hay bảng PASS cũ làm bằng chứng cho source mới.

Các mục nên có/mở rộng chưa nhận triển khai: phiếu chi nhập, audit log đầy đủ trước/sau mỗi trường, Việt/Anh cho toàn bộ quản trị và thông báo lỗi, email, thanh toán online, hết hạn giữ xe, trả góp, bảo hành nghiệp vụ và nhiều showroom. Không có chức năng giả báo thành công để thay cho các mục này.
