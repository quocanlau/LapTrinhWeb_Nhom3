# Kịch bản nghiệm thu trên máy nhóm

Các mục dưới đây chưa tự điền Pass cho SQL Server/UI thực. Với mỗi dòng ghi ngày, người chạy, đầu vào, kết quả thực tế, Pass/Fail, ảnh/log, cách sửa và chạy lại.

| Mã | Thao tác và dữ liệu | Kết quả cần đạt | Minh chứng hiện có |
| --- | --- | --- | --- |
| T01 | Email sai, mật khẩu xác nhận khác | Form không tạo tài khoản | Annotation có; cần test UI. |
| T02 | Customer vào admin, API ghi hãng | Bị từ chối | HTTP integration có. |
| T03 | A đọc API yêu cầu B | 404, không lộ ghi chú | Test owner trực tiếp có. |
| T04 | VIN trùng/sai, giá âm, năm quá giới hạn | Không lưu | Policy/schema có, cần SQL UI thật. |
| T05 | Phiếu nháp hai xe rồi xác nhận | Nháp chưa lên sàn, xác nhận cả hai | SQLite service + HTTP có. |
| T06 | Một xe không hợp lệ khi xác nhận | Rollback toàn phiếu | SQLite test có; cần SQL thật. |
| T07 | Hãng/dòng/khoảng giá/năm, đổi trang/rỗng | Giữ đúng lọc, SQL paging | Query và HTTP có; cần browser AJAX. |
| T08 | Đổi lọc nhanh, lỗi mạng | Không ghi đè kết quả mới, báo lỗi | Mã abort có; chưa browser. |
| T09 | Hai người đồng thời giữ một xe | Chỉ một giao dịch hiệu lực | Test SQL Server tùy chọn chưa chạy. |
| T10 | Hai khoản thu đồng thời và thu vượt | Tổng không vượt giá chốt | Service test có, SQL concurrency chưa chạy. |
| T11 | Bàn giao thiếu tiền rồi đủ | Chặn lần đầu, đúng một Delivery khi đủ | Service + HTTP có. |
| T12 | Hoàn hết rồi hủy; hủy đã bàn giao | Giữ ledger, không hủy Delivered | Test service có. |
| T13 | Xóa danh mục có FK, sửa phiếu xác nhận | Bị chặn, lịch sử còn | Constraint/service có; cần SQL UI. |
| T14 | POST thiếu CSRF, tên/ghi chú có nháy/script | 400 hoặc hiển thị text an toàn | HTTP CSRF, data attribute/Razor có. |
| T15 | 375/768/1366, vi/en, session hết | UI dùng được, DB không mất | Chưa kiểm tra browser và session timeout thực. |
| T16 | Đối chiếu bảng giao dịch/thu/giá vốn theo ngày | Công thức đúng, không tính cọc là doanh thu | HTTP dashboard có, cần số liệu SQL thật. |

Thêm test upload đúng/sai đuôi/signature/dung lượng; Compare xe thứ tư; bỏ dòng phiếu nhập rồi thêm lại; khóa tài khoản đang đăng nhập; reset mật khẩu; gán role; thử cập nhật cấu hình bằng version cũ.
