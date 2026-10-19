# Autora với SQL Server và EF Core

Bản cập nhật ngày 09/10/2026 bổ sung CSDL, Identity và luồng nhập mua–yêu cầu cọc–giữ xe–thu tiền–bàn giao. Tiền cọc bằng **2% giá bán chốt**, làm tròn đến đồng VND. Xe Đã cọc không nhận yêu cầu cọc mới; giao dịch đang giữ cũng chặn giao dịch khác giữ cùng xe.

## 1. Điều kiện chạy trên máy Windows

- .NET 8 SDK và Visual Studio có workload ASP.NET nếu dùng Visual Studio.
- SQL Server LocalDB hoặc một SQL Server instance truy cập được.
- Internet để restore NuGet; giao diện hiện vẫn tải Bootstrap/jQuery/font từ CDN.
- Giải nén vào một thư mục mới. Mở `Autora.sln` để tương thích SDK .NET 8; cũng có `Autora.slnx` dành cho phiên bản Visual Studio hỗ trợ.

Chạy tất cả lệnh sau từ thư mục có README và Autora.sln. Không chạy migration vào database chứa bảng cùng tên do hệ thống khác tạo; sử dụng AutoraDb mới để thử.

## 2. Restore và build

```powershell
dotnet restore .\Autora.sln
dotnet tool restore
dotnet build .\Autora.sln
```

Các package EF Core/Identity và dotnet-ef được ghim cùng phiên bản 8.0.11. Migration đầu tiên và snapshot đã được tạo; **không chạy migrations add InitialAutora lần nữa**.

## 3. Kết nối SQL Server

Nếu có LocalDB, ứng dụng mặc định dùng:

```text
Server=(localdb)\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True
```

Có thể đặt cấu hình rõ ràng bằng user-secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True" --project .\Autora\Autora.csproj
```

Nếu dùng SQL Server Express với Windows Authentication, thay Server bằng tên instance của bạn, ví dụ `localhost\SQLEXPRESS`. Nếu dùng SQL Authentication, lưu connection string trong user-secrets hoặc biến môi trường, không ghi mật khẩu thật vào file để nộp bài. TrustServerCertificate ở chuỗi trên dành cho máy học/demo; cấu hình chứng chỉ theo môi trường khi triển khai.

## 4. Tạo database bằng migration

User-secrets được nạp ở Development, nên đặt môi trường trước:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project .\Autora\Autora.csproj
```

Migration tạo các bảng nghiệp vụ, các bảng Identity, khóa, chỉ mục và ràng buộc. Ứng dụng không tự tạo/xóa database khi khởi động.

**Cách thay thế:** mở `scripts/AutoraDb.sql` trong SSMS và chạy trên SQL Server đã chọn. Script tạo AutoraDb nếu chưa có, chuyển sang AutoraDb và áp dụng migration theo lịch sử. Chọn một cách để dựng CSDL; không cần dựng lại bằng cả hai. Script không có mật khẩu hoặc tài khoản admin.

## 5. Tạo vai trò, admin và dữ liệu mẫu

```powershell
dotnet user-secrets set "Bootstrap:AdminEmail" "admin@autora.local" --project .\Autora\Autora.csproj
dotnet user-secrets set "Bootstrap:AdminPassword" "<mat-khau-do-ban-tu-dat>" --project .\Autora\Autora.csproj
dotnet user-secrets set "Seed:IncludeDemoData" "true" --project .\Autora\Autora.csproj
dotnet run --project .\Autora\Autora.csproj --launch-profile http -- --seed
```

Thay `<mat-khau-do-ban-tu-dat>` bằng mật khẩu riêng, tối thiểu 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt. Không dùng chuỗi placeholder nguyên dạng. Mật khẩu không nằm trong source hoặc SQL script.

Lệnh seed tạo ba vai trò Admin/Employee/Customer và tài khoản admin. Nếu bật IncludeDemoData và bảng xe còn trống, thêm 16 xe mẫu có VIN giả dùng cho demo, hãng/dòng Toyota Camry, đối tác và một phiếu nhập đã xác nhận. Ảnh xe mẫu là hình minh họa local, không phải ảnh nhận diện từng xe. Khi database đã có xe, seed không thêm bộ xe mẫu lần nữa.

Seed dùng email được cấu hình để tạo hoặc gán vai trò admin; chỉ chạy bằng người triển khai tin cậy. Seed không đổi mật khẩu tài khoản đã tồn tại. Có thể xóa Bootstrap:AdminPassword khỏi user-secrets sau khi tạo tài khoản và không cần chạy seed nữa.

## 6. Chạy và kiểm tra luồng chính

```powershell
dotnet run --project .\Autora\Autora.csproj --launch-profile http
```

Mặc định HTTP: `http://localhost:5241`. HTTPS: `https://localhost:7010` khi chọn profile https. Ưu tiên URL thực tế in ở terminal.

1. Mở `/Account/Login`, đăng nhập admin đã seed.
2. Mở `/Admin/Manufacturer` để quản lý hãng và thêm dòng xe.
3. Mở `/Admin/Car`, tạo hồ sơ xe chờ nhập; VIN phải có 17 ký tự, không gồm I/O/Q. Có thể đánh dấu công khai, nhưng xe chỉ xuất hiện sau xác nhận nhập.
4. Mở `/Admin/Purchase`, tạo đối tác nếu cần, chọn xe chờ nhập và xác nhận giá nhập. Bản đầu nhập một xe mỗi lần; cấu trúc bảng hỗ trợ nhiều chi tiết.
5. Mở trình duyệt khác hoặc đăng xuất rồi đăng ký khách tại `/Account/Register`. Tài khoản công khai chỉ nhận vai trò Customer.
6. Mở `/Car`, chọn xe Sẵn sàng, gửi yêu cầu. Theo dõi tại `/Deposit/Mine`.
7. Admin vào `/Admin/Order`, chốt giá và xác nhận giữ xe. Cọc = ROUND(giá chốt × 0.02, 0). Ví dụ 2.799.000.000 đ → 55.980.000 đ.
8. Admin ghi nhận khoản thu. Khi thực thu đạt cọc yêu cầu, xe thành Đã cọc; nút cọc mới bị khóa và máy chủ từ chối yêu cầu mới.
9. Khách cũ thanh toán phần còn lại bằng khoản thu tiếp theo. Chỉ khi đủ giá bán mới bàn giao; xe thành Đã bán.
10. Nếu hủy trước bàn giao, hoàn đủ khoản đã thu rồi hủy. Bản đầu chưa hỗ trợ giữ lại cọc/phạt hủy hoặc tự hết hạn giữ chỗ.

Gửi yêu cầu không đồng nghĩa đã giữ xe hoặc đã trả tiền. Chọn phương thức Chuyển khoản/POS chỉ lưu cách nhân viên nhận tiền, không gọi cổng thanh toán.

## 7. Quy tắc chống trùng và chống thu vượt

- Car.RowVersion phát hiện sửa trạng thái xe đồng thời.
- SaleTransaction.RowVersion và UpdatedAt bảo vệ các khoản thu/hoàn cùng giao dịch.
- Chỉ mục UNIQUE có điều kiện trên SaleTransactions.CarId cho trạng thái 1/2/3 chặn nhiều giao dịch giữ/đã cọc/đã bàn giao cho cùng xe.
- Một lần SaveChanges hoặc transaction bao toàn bộ chứng từ và cập nhật xe; thất bại không để lại một nửa giao dịch.
- Xe Chờ nhập/Đang giữ/Đã cọc/Đã bán không nhận yêu cầu cọc mới.
- Thu tiền không vượt giá bán; hoàn không vượt thực thu; hoàn một phần xuống dưới mức cọc đưa về Đang giữ, vẫn không nhận giao dịch mới.
- Bản đầu một xe chỉ nhập một lần; thu mua lại chiếc xe đã bán là mở rộng.

## 8. API, AJAX và session

- `GET /api/cars`, `GET /api/cars/{id}`: dữ liệu công khai, không trả giá nhập hoặc thông tin khách.
- `GET /api/manufacturers`, `GET /api/manufacturers/{id}`.
- `POST /api/manufacturers`, `PUT/DELETE /api/manufacturers/{id}`: chỉ Admin.
- API ghi dùng cookie authentication nên cần cookie đăng nhập và token CSRF trong header `X-CSRF-TOKEN`. Token được sinh trong meta `csrf-token` của layout khách. API chưa đăng nhập trả 401, thiếu quyền trả 403.
- `/Car/List` trả partial HTML cho AJAX lọc/phân trang. API JSON ở trên là phần RESTful riêng.
- `/Cart` là danh sách quan tâm lưu session, không phải giữ chỗ. Session mất sau thời gian không hoạt động hoặc khi ứng dụng khởi động lại; chứng từ nằm trong SQL Server.
- Cookie ghi nhớ đăng nhập dùng Identity. Chưa bổ sung đổi ngôn ngữ/culture cookie.

## 9. Kiểm thử và giới hạn xác minh

```powershell
dotnet test .\Autora.Tests\Autora.Tests.csproj
```

Kết quả trên môi trường sửa source: build project web thành công với 0 lỗi/0 cảnh báo; 19 test đạt; EF xác nhận model không có thay đổi chưa đưa vào migration. Smoke HTTP kiểm tra Login/Register/Privacy/Error trả 200, Admin chưa đăng nhập chuyển đến Login, API ghi chưa đăng nhập trả 401, đăng ký sai dữ liệu trả form lỗi, POST thiếu CSRF trả 400.

**Chưa có SQL Server chạy trong môi trường kiểm tra**, nên chưa chạy migration/seed/CRUD/giao dịch và test đồng thời trên SQL Server thật. GenerateCreateScript/migrations script kiểm tra được mapping và sinh SQL nhưng không thay thế chạy SQL thật. Cần thực hiện kịch bản mục 6 trên máy có SQL Server trước khi kết luận nghiệm thu. Chưa kiểm tra giao diện trực quan trên trình duyệt ở mọi kích thước.

Cần kiểm tra thêm: hai admin cùng giữ một xe, hai khoản thu đồng thời, VIN trùng, xóa hãng đã có dòng, hủy có cọc, bàn giao khi chưa đủ tiền, khách đổi id để truy cập dữ liệu người khác.

## 10. Phần chưa thuộc bản cập nhật này

- Giao diện quản lý/gán vai trò nhân viên và khóa tài khoản; ba vai trò đã có trong Identity nhưng admin được bootstrap, khách đăng ký công khai.
- CRUD đầy đủ khách/đối tác/dòng xe; hiện có tạo đối tác và dòng xe, khách được tạo khi đăng ký.
- Quốc tế hóa, báo cáo theo kỳ, upload tệp ảnh, phiếu chi nhập, nhiều showroom, tự hết hạn giữ xe, giữ lại cọc khi hủy.
- Biểu mẫu liên hệ trong trang giới thiệu và một số link thông tin mẫu chưa có backend.
- Báo cáo học phần, sơ đồ thiết kế trình bày và test case có kết quả thực tế trên SQL Server vẫn cần hoàn thiện.

Không coi bản này là hoàn thành toàn bộ yêu cầu nộp bài. Nội dung thay đổi và những phần còn lại được ghi trong PROJECT_GUIDE.md.


## Cập nhật nút chuông quản trị

Bản này bổ sung migration AddAdminNotificationReads. Trước khi chạy hãy áp dụng `dotnet ef database update --project .\Autora\Autora.csproj` vào DB hiện tại. Xem CAP_NHAT_THONG_BAO.md để biết chức năng và cách cập nhật. Không cần seed lại.
