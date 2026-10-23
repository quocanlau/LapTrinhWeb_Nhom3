# AUTORA — Cài database bằng một file SQL

Bộ nộp này dùng ASP.NET Core MVC .NET 8, EF Core và SQL Server. Dữ liệu được lấy nguyên từ file autorafull.sql nhóm xuất ngày 20/10/2026, không tạo xe mẫu hoặc chạy seed.

## Chạy trên máy mới

1. Cài .NET 8 SDK, Visual Studio hỗ trợ .NET 8 và SQL Server Express LocalDB một lần nếu máy chưa có.
2. Kết nối `(localdb)\MSSQLLocalDB` trong SSMS hoặc SQL Server Object Explorer của Visual Studio.
3. Mở **03_Database/Autora_Full.sql**, chạy **toàn bộ file** bằng Execute. Script tạo AutoraDb, bảng, dữ liệu và kiểm tra số bản ghi. SQL Server 2019/LocalDB major 15 trở lên là mục tiêu cú pháp.
4. Mở **02_SourceCode/Autora.sln**, chọn **Autora → Set as Startup Project**, chờ NuGet restore, chọn **http → Ctrl+F5**.
5. Đăng nhập bằng tài khoản/mật khẩu đã có lúc xuất DB. Password hash giữ nguyên; bản này không đổi mật khẩu.

**Không cần seed, restore backup hoặc chạy Update-Database trong lần cài đầu bằng file Full.** Không cần SQLCMD mode. SQL Server/LocalDB vẫn phải được cài trên máy; mở Visual Studio không thay thế database engine.

## Nếu máy đã có AutoraDb

Script Full dừng và báo rõ, không xóa, ghi đè hoặc trộn dữ liệu. Bạn có thể tiếp tục dùng DB đang có nếu nó đúng bản cần dùng. Muốn thử dữ liệu chuẩn riêng, xem hướng dẫn chi tiết; không xóa DB đang có để thử.

## Dữ liệu trong file đã xuất

23 bảng; 16 xe, 4 hãng, 15 dòng xe, 16 liên kết ảnh, 4 tài khoản, 3 khách hàng, 3 yêu cầu và 2 giao dịch bán. Các bảng chưa có dữ liệu trong bản xuất vẫn được tạo đầy đủ. Giữ cả ba migration đã áp dụng, khóa, chỉ mục, ràng buộc và password hash. Không tự bổ sung hoặc sửa các bản ghi nghiệp vụ.

Các ảnh đã được đối chiếu: 16 liên kết ảnh đều có tệp tương ứng trong source.

## Giới hạn kiểm chứng

Đã kiểm tra cú pháp SQL Server 2019, đối chiếu toàn bộ INSERT, FK, ảnh và mã nguồn. Chưa Execute script trên SQL Server thực trong môi trường xử lý này; kết quả kiểm tra chi tiết nằm tại 05_KiemTra/KET_QUA_BAN_SQL_DAY_DU.md. Không coi kiểm tra cú pháp là bằng chứng đã chạy thành công trên mọi máy.

## Tài liệu

- **04_HuongDanCaiDat/HUONG_DAN_CHAY_SQL_VA_WEBSITE.md**: từng bước cài và xử lý lỗi.
- **03_Database/KiemTraDB.sql**: truy vấn kiểm tra bảng, số bản ghi, migration và ràng buộc sau cài.
- **03_Database/README.md**: phân biệt file cài chính và file tham khảo.
- **05_KiemTra/KET_QUA_BAN_SQL_DAY_DU.md**: đối chiếu dữ liệu và xác nhận code không đổi.

Backup nhị phân không còn cần cho luồng cài đặt chính. Bản xuất gốc được giữ riêng trong 03_Database/ThamKhao để đối chiếu, không chạy đồng thời với file Full. Git sẽ lưu file SQL và ảnh; không tự đồng bộ DB đang chạy trên các máy. Dữ liệu tài khoản/giao dịch trong script chỉ nên chia sẻ đúng phạm vi nhóm cho phép.
