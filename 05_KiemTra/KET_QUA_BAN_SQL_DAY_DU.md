# Đối chiếu bản SQL đầy đủ — 20/10/2026

## Phạm vi

Lấy source từ Autora_NopBai(1).zip và dữ liệu từ autorafull.sql do người dùng xuất qua SSMS. Chỉ thay phần SQL, tài liệu cài đặt và đóng gói. Toàn bộ 201 file source/cấu hình/tài nguyên trong 02_SourceCode giữ nguyên byte; không sửa code chính, test, csproj, migration, appsettings hoặc assets.

Loại các file IDE/build phát sinh và hai backup nhị phân khỏi gói phát hành. Các đầu vào gốc vẫn là nguồn đối chiếu; file SQL xuất gốc giữ nguyên byte tại 03_Database/ThamKhao/autorafull_XuatGoc.sql.

## Kiểm kê dữ liệu giữ nguyên từ bản xuất

23 bảng khớp toàn bộ tên bảng trong schema theo migration của project; không thiếu bảng nào. Giữ 92 lệnh INSERT/92 dòng dữ liệu, 24 khóa ngoại, 13 CHECK được xuất và 13 câu CREATE UNIQUE index. Toàn bộ thân schema/data trước phần kiểm tra được lấy nguyên từ bản xuất, chỉ bỏ dấu phân tách batch GO ngoài chuỗi/comment để bao trong một lần thực thi.

| Bảng | Số dòng trong bản xuất |
| --- | ---: |
| __EFMigrationsHistory | 3 |
| AdminNotificationReads | 1 |
| AspNetRoleClaims | 0 |
| AspNetRoles | 3 |
| AspNetUserClaims | 0 |
| AspNetUserLogins | 0 |
| AspNetUserRoles | 4 |
| AspNetUsers | 4 |
| AspNetUserTokens | 0 |
| CarImages | 16 |
| CarModels | 15 |
| Cars | 16 |
| ContactInquiries | 0 |
| Customers | 3 |
| Deliveries | 0 |
| DepositRequests | 3 |
| Manufacturers | 4 |
| PaymentEntries | 0 |
| PurchaseDetails | 16 |
| PurchaseReceipts | 1 |
| SaleTransactions | 2 |
| ShowroomSettings | 0 |
| Suppliers | 1 |

Các bảng 0 dòng không bị loại: bản xuất không có INSERT cho chúng. Không thêm dữ liệu giả để lấp bảng trống. Password hash, security stamp, vai trò/liên kết, VIN, giá, trạng thái và chứng từ giữ đúng giá trị đã xuất. RowVersion do SQL Server sinh khi INSERT theo đúng cơ chế của script SSMS; không chèn timestamp cũ.

## Điều chỉnh phục vụ cài trên máy khác

- Không hardcode MDF/LDF tới C:\Users\My PC. CREATE DATABASE dùng đường dẫn mặc định của instance.
- Bỏ các tùy chọn database riêng của SQL Server 2025 như OPTIMIZED_LOCKING, LEDGER và compatibility 170; dùng compatibility 150, giữ RCSI ON và recovery SIMPLE. AUTO_CLOSE OFF để hạn chế đóng/mở DB giữa kết nối. Các đối tượng/data trong thân xuất không bị thay đổi.
- Một file chính Autora_Full.sql; không SQLCMD mode, không restore .bak hoặc seed. Ba migration của DB nguồn được giữ nguyên để không cần Update-Database cho lần cài đầu.
- Toàn bộ lớp bao cài đặt là một batch. DB đích mặc định AutoraDb, chỉnh được bằng một biến ở đầu. Tên định danh được QUOTENAME, lấy khóa session, kiểm tra DB tồn tại trước mọi CREATE/INSERT.
- DB có sẵn thì dừng; không DROP, REPLACE hoặc trộn dữ liệu. Yêu cầu query không có transaction mở trước khi chạy.
- CREATE DATABASE thực thi trước transaction vì quy định SQL Server. Schema và data được nạp trong transaction, kiểm tra số dòng của cả 23 bảng rồi mới commit. Nếu lỗi, rollback phần nạp; DB mới tạo có thể còn rỗng, không tự xóa DB.
- Có KiemTraDB.sql chỉ đọc để người nhận kiểm tra sau cài.

## Kiểm tra đã chạy

| Kiểm tra | Kết quả |
| --- | --- |
| Build solution .NET 8 | 0 lỗi, 0 cảnh báo |
| Test tự động | 50 thành công, 1 SQL Server test bị skip, tổng 51 |
| Parser TSql150 / ScriptDom 161.8901.0 | 0 lỗi cho lớp bao, nội dung SQL giải mã, thân SQL và file kiểm tra |
| Số dòng AST | Khớp toàn bộ 92 dòng INSERT |
| Đối chiếu INSERT với bản xuất | Giữ nguyên tất cả dòng INSERT |
| Khóa ngoại trên các dòng dữ liệu xuất | 24 quan hệ được kiểm tra, không có tham chiếu thiếu |
| Ảnh xe | 16 liên kết nội bộ, 0 file thiếu |
| Mã nguồn với ZIP mới nhất | 201 file nguyên byte, 0 file thay đổi |

Chi tiết log/TRX/kiểm kê/hash tại SqlFull_20261020. Không đưa hash mật khẩu hay nội dung tài khoản ra báo cáo này; chúng vẫn nằm trong dữ liệu SQL theo bản xuất.

## Giới hạn và bước cần nhóm xác nhận

Chưa Execute Autora_Full.sql trên SQL Server thực trong môi trường xử lý này. Parser không kiểm tra được quyền, đường dẫn instance, khóa runtime hoặc hành vi transaction của engine. Test SQL Server bị skip không phải Pass. Chưa xác nhận đăng nhập bằng mật khẩu thực và trình duyệt trên Windows.

Kết luận không thiếu dữ liệu áp dụng đối với **file autorafull.sql được người dùng cung cấp**, không phải khẳng định mọi dữ liệu trên máy nguồn đã được xuất nếu có bảng/đối tượng ngoài file đó. Phần schema của dự án đã đối chiếu đủ 23 bảng. Tùy chọn database đặc thù 2025 vẫn được lưu trong bản xuất gốc để tra cứu.

Nhóm cần Execute toàn bộ file trên DB tên mới chưa tồn tại, kiểm tra số dòng, mở web, đăng nhập tài khoản có trong bản xuất và thử luồng nghiệp vụ. Thử chạy SQL lần hai phải dừng trước khi thay đổi DB. Chỉ ghi Pass cho các mục này sau khi thực hiện thật.
