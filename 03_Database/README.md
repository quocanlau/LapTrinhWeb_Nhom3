# Các file database

| File | Mục đích |
| --- | --- |
| **Autora_Full.sql** | File cài CHÍNH: tạo AutoraDb, toàn bộ schema và dữ liệu nhóm đã xuất. Chạy toàn bộ một lần trên DB chưa tồn tại. |
| **KiemTraDB.sql** | Chỉ đọc, kiểm tra dữ liệu sau cài. |
| AutoraDb.sql | Schema theo migration, không chứa dữ liệu xe/tài khoản; tham khảo hoặc dùng trong quy trình EF riêng. Không dùng thay file Full nếu muốn có dữ liệu sẵn. |
| CapNhatBanCu.sql | Cập nhật cấu trúc cho DB cũ đã có AddAdminNotificationReads; không phải file cài máy mới. |
| ThamKhao/autorafull_XuatGoc.sql | Bản xuất Unicode gốc được giữ nguyên byte. Có đường dẫn máy nguồn và tùy chọn SQL Server 2025 nên chỉ dùng đối chiếu. |

Không chạy tất cả file SQL cùng lúc. Ưu tiên Autora_Full.sql theo hướng dẫn ở 04_HuongDanCaiDat.

Script Full không drop database, không thay dữ liệu khi DB đã tồn tại. DDL và dữ liệu chạy trong cùng transaction; nếu nạp lỗi, các bảng/bản ghi của lần nạp được rollback. CREATE DATABASE nằm ngoài transaction nên lỗi có thể để lại DB mới rỗng; không được hiểu là cài thành công. Khi đó giữ lỗi để kiểm tra, không xóa nhầm DB cũ.

Giữ nguyên dữ liệu xuất, khóa ngoại, CHECK, chỉ mục, identity, lịch sử EF và password hash. RowVersion/timestamp được SQL Server sinh lại đúng cơ chế khi INSERT, như cách bản xuất SSMS đã thiết kế, không chèn giá trị timestamp thủ công.
