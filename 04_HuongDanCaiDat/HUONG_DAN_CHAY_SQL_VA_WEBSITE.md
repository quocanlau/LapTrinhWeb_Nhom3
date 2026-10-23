# Hướng dẫn cài AUTORA: chạy SQL rồi mở website

## 1. Cần cài gì?

Trên Windows, cài một lần:

- Visual Studio hỗ trợ .NET 8, workload ASP.NET and web development.
- .NET 8 SDK, không chỉ Runtime.
- SQL Server Express LocalDB, hoặc SQL Server Express/Developer đang có. Script hướng đến SQL Server 2019 (major 15) trở lên; không dùng các tùy chọn riêng của SQL Server 2025 trong file xuất gốc.

SSMS chỉ là công cụ quản lý, không thay SQL Server engine. Nếu đã có SQL Server Object Explorer trong Visual Studio thì có thể chạy SQL ở đó, không bắt buộc cài SSMS. Máy cần Internet khi NuGet restore package chưa có trong cache.

## 2. Kết nối LocalDB

### Dùng Visual Studio

1. View → SQL Server Object Explorer.
2. Add SQL Server.
3. Server Name: `(localdb)\MSSQLLocalDB`.
4. Windows Authentication → Connect.
5. Mở query trên kết nối này. Mở file SQL trong SQL editor và chọn đúng kết nối nếu được hỏi.

### Dùng SSMS

1. Server type: Database Engine.
2. Server name: `(localdb)\MSSQLLocalDB`.
3. Authentication: Windows Authentication → Connect.
4. File → Open → File, chọn script ở mục 3.

Nếu LocalDB chưa có hoặc chưa chạy, mở Terminal PowerShell và kiểm tra:

```powershell
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

Chỉ tạo instance khi danh sách chưa có:

```powershell
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

Nếu không nhận diện sqllocaldb, kiểm tra đã cài LocalDB trong Visual Studio Installer hoặc bộ cài SQL Server Express. Nếu đang dùng Express/Developer thay cho LocalDB, kết nối đúng Server name và làm mục 6.

## 3. Tạo DB và dữ liệu bằng một file

**Chỉ chạy file `03_Database/Autora_Full.sql`.**

1. Dừng website nếu đang chạy.
2. Mở toàn bộ file trên đúng kết nối SQL Server/LocalDB, không chỉ dán một đoạn INSERT.
3. Không tự tạo AutoraDb rỗng trước. Script tự CREATE DATABASE bằng thư mục dữ liệu mặc định của máy nhận.
4. Bỏ vùng chọn nếu đang bôi đen một phần; nhấn Execute để chạy toàn bộ file. Trong SSMS có thể nhấn F5. Không cần bật SQLCMD mode.
5. Chờ đến khi Messages có dòng **AUTORA: Database [AutoraDb] da tao day du schema va du lieu** và không có lỗi.
6. Refresh Databases, mở AutoraDb → Tables.
7. Chạy `03_Database/KiemTraDB.sql` để đối chiếu dữ liệu.

Script đã bỏ đường dẫn C:\Users\My PC từ máy nguồn, bỏ các tùy chọn database riêng của SQL Server 2025, đặt compatibility level 150 và giữ READ_COMMITTED_SNAPSHOT ON như bản xuất. AUTO_CLOSE được tắt để tránh đóng/mở DB liên tục giữa các kết nối.

DDL và dữ liệu được nạp trong một transaction, có kiểm tra số dòng từng bảng trước commit. Bảng, bản ghi, khóa, chỉ mục và ràng buộc bên trong được giữ nguyên theo file bạn xuất; không tự gen xe, seed admin, reset mật khẩu hoặc sửa nghiệp vụ.

CREATE DATABASE không nằm trong transaction vì SQL Server yêu cầu riêng. Nếu nạp lỗi, các bảng/bản ghi mới được rollback nhưng database vừa tạo có thể còn ở trạng thái rỗng. Chỉ coi cài thành công khi có thông báo cuối và truy vấn kiểm tra thành công. Không xóa DB cũ để xử lý lỗi ngẫu nhiên.

### Nếu AutoraDb đã tồn tại

Script báo lỗi 51001 và dừng để giữ nguyên database. Chạy lại không tự trộn, xóa hoặc chèn trùng dữ liệu.

- Nếu đó chính là DB của nhóm bạn đang dùng, giữ nguyên và chuyển sang mục 4.
- Nếu muốn kiểm tra bộ dữ liệu xuất trong một DB riêng, tạo bản sao của file Full rồi sửa **duy nhất dòng** gần đầu file:

```sql
DECLARE @database sysname = N'AutoraDb';
```

thành:

```sql
DECLARE @database sysname = N'AutoraDb_KiemTra';
```

Chạy toàn bộ bản sao. Tên mới phải chưa tồn tại. Nếu muốn web dùng DB này thì đặt chuỗi kết nối theo mục 6. Khi chạy KiemTraDB.sql, đổi riêng câu USE cho đúng tên DB đang kiểm tra. Không thay tên database bằng cách thay toàn bộ nội dung dữ liệu xuất.

## 4. Mở và chạy website

1. Mở `02_SourceCode/Autora.sln`.
2. Trong Solution Explorer, nhấp phải project Autora → Set as Startup Project.
3. Chờ NuGet restore package. Build → Build Solution nếu cần kiểm tra lỗi.
4. Chọn profile **http** cạnh nút Run.
5. Ctrl+F5 hoặc F5.
6. Mở URL Visual Studio hiển thị, thường là http://localhost:5241.
7. Đăng nhập bằng tài khoản/mật khẩu sẵn có trong DB lúc xuất.

**Không chạy seed hoặc Update-Database cho lần cài đầu đã dùng file Full.** Lịch sử ba migration đã có trong SQL. Các lần chạy sau chỉ mở solution và Run; không Execute lại file Full mỗi lần.

Giữ thư mục wwwroot/images của project khi chuyển máy. SQL lưu đường dẫn ảnh, không chứa nội dung file ảnh. Cả 16 liên kết ảnh trong bản xuất đã được đối chiếu thấy file tương ứng.

Bản này không thêm cơ chế tự tạo hoặc tự restore DB vào Program.cs. Bước tạo dữ liệu là Execute SQL rõ ràng trước khi Run.

## 5. Dữ liệu và đăng nhập

Bản xuất có 23 bảng và 92 lệnh INSERT, mỗi lệnh một dòng dữ liệu. Trong đó có 16 xe, 4 hãng, 15 dòng xe, 16 ảnh, 4 tài khoản, 3 khách hàng, 3 yêu cầu, 2 giao dịch bán, 1 phiếu nhập, 16 chi tiết nhập và 1 nhà cung cấp. Bảng trống trong file xuất vẫn được tạo; không tự điền dữ liệu vào đó.

Identity roles, liên kết tài khoản–vai trò, password hash, security stamp và lịch sử EF được giữ nguyên. Không thể đọc ngược password hash thành mật khẩu rõ. Nhóm cần cung cấp tài khoản demo mà nhóm đã biết cho người nhận; không dùng mật khẩu SQL Server để đăng nhập website.

Không sửa trực tiếp password hash trong bảng. Nếu cần đặt lại mật khẩu, dùng chức năng quản trị đã có trong ứng dụng hoặc thực hiện quy trình đặt lại riêng có kiểm tra.

## 6. Nếu website kết nối sai DB

Source mặc định dùng:

```text
Server=(localdb)\MSSQLLocalDB;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True
```

Không cần sửa code trên máy mới dùng đúng LocalDB. Nếu trước đây đã cấu hình user-secrets, hoặc đang dùng SQL Server Express, cần kiểm tra override trên máy hiện tại.

Mở View → Terminal, chọn PowerShell và đứng tại `02_SourceCode` có Autora.sln. Để quay về cấu hình mặc định của source:

```powershell
dotnet user-secrets remove "ConnectionStrings:DefaultConnection" --project .\Autora\Autora.csproj
```

Để chỉ định một DB khác đang có, ví dụ SQL Express:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=AutoraDb;Trusted_Connection=True;TrustServerCertificate=True" --project .\Autora\Autora.csproj
```

Thay server/tên DB cho đúng. Muốn dùng DB thử AutoraDb_KiemTra trên LocalDB thì thay server thành `(localdb)\MSSQLLocalDB`, Database thành AutoraDb_KiemTra. SQL và website phải dùng **cùng server, instance và tên DB**.

Profile http đặt môi trường Development để đọc appsettings.Development.json và user-secrets. Biến môi trường ConnectionStrings__DefaultConnection có thể ghi đè các thiết lập này; kiểm tra nếu vẫn kết nối sai nơi. Không công khai mật khẩu hoặc toàn bộ nội dung user-secrets.

## 7. Lỗi thường gặp

| Hiện tượng | Cách xử lý |
| --- | --- |
| AutoraDb đã tồn tại | Script dừng có chủ đích. Dùng DB hiện có hoặc cài vào tên DB riêng theo mục 3 |
| CREATE DATABASE permission denied | Dùng LocalDB của tài khoản Windows hiện tại hoặc nhờ cấp quyền trên instance đã chọn |
| SQL Server quá cũ | Dùng SQL Server 2019/LocalDB major 15 trở lên |
| Không kết nối instance | Kiểm tra engine đã cài, đúng tên server và dịch vụ/instance đang chạy |
| Invalid object/column name khi mở web | Kiểm tra Full SQL đã chạy thành công và web đang dùng đúng DB; không seed để chữa lỗi |
| SQL báo sai số dòng | Ghi lại lỗi; transaction nạp sẽ rollback. Không coi là cài thành công |
| Một số bảng trống | Có thể đúng theo bản xuất. Xem kiểm kê; không phải mọi bảng đều có dữ liệu nguồn |
| Không đăng nhập được | Dùng tài khoản/mật khẩu có lúc xuất DB, bản này không tạo admin mới hoặc đổi mật khẩu |
| Website không thấy xe sau khi Execute SQL | Kiểm tra chuỗi kết nối đang trỏ DB/instance khác hoặc dữ liệu công khai theo trạng thái xe |
| Chữ tiếng Việt lỗi khi xem script | Mở file UTF-8 có BOM đúng encoding; không đổi N'...' thành chuỗi không Unicode |

Không chọn chạy riêng phần thân SQL động bên trong file Full. Lớp bao ngoài giúp kiểm tra DB tồn tại, khóa cài đặt và xử lý transaction. File được kiểm tra cả lớp bao lẫn SQL đã giải mã ở bên trong.

## 8. Đưa lên GitHub và bàn giao

Giữ 03_Database/Autora_Full.sql, tài liệu cài, ảnh, migration và source. File .sql không bị loại bởi .gitignore; không cần ép add backup. Không đưa bin, obj hoặc .vs lên Git.

Người nhận tải đủ project rồi làm mục 2–4. Git lưu snapshot SQL, không tự đồng bộ database đang chạy. Nếu sau này muốn phát hành dữ liệu mới, xuất lại Schema and data từ đúng DB rồi kiểm tra trước khi thay file Full.

Script có password hash, thông tin tài khoản và dữ liệu nghiệp vụ. Chia sẻ theo phạm vi nhóm cho phép; nếu định công khai dữ liệu cá nhân thật thì cần dùng bộ demo đã được nhóm kiểm tra.

## 9. Phần giữ nguyên và giới hạn kiểm tra

Toàn bộ 201 tệp source/cấu hình/tài nguyên trong 02_SourceCode được đối chiếu nguyên byte với ZIP Autora_NopBai(1).zip, không tính file IDE/build phát sinh bị loại khỏi gói. Không sửa Program.cs, controller, service, model, view, CSS/JS, csproj, migration hoặc appsettings. EF Core vẫn cần cho code, không bị xóa.

Đã kiểm tra cú pháp SQL bằng parser SQL Server 2019, dữ liệu INSERT, đủ bảng theo schema, khóa ngoại và ảnh. Chưa Execute trên SQL Server thực trong môi trường xử lý này nên chưa có kết luận Pass cho cài đặt/runtime DB. Nhóm cần chạy thử trên máy khác, xác nhận số dòng và thử đăng nhập, xem xe, đăng xuất/đăng nhập, đặt cọc và bàn giao trên dữ liệu thử trước khi nộp.

Chi tiết kiểm thử: 05_KiemTra/KET_QUA_BAN_SQL_DAY_DU.md.
