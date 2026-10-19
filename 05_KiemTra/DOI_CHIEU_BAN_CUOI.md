# Đối chiếu bản hoàn thiện với yêu cầu

Phạm vi: source bản ZIP người dùng gửi ngày 19/10/2026 và tài liệu tổng hợp yêu cầu đi kèm. Phân biệt phần bắt buộc của học phần, nghiệp vụ cốt lõi và mục nên có/mở rộng; không gán điểm hay tự khẳng định toàn bộ sản phẩm đã được nghiệm thu.

## Các mục đã hoàn thiện trong mã nguồn

| Mã | Bản nhận | Bản này |
| --- | --- | --- |
| F01 | Chưa sửa hồ sơ | Có Profile, ChangePassword; đồng bộ tên/điện thoại Customer và User. |
| F02 | Có danh sách/chi tiết, recentCars lỗi | Dùng điều kiện public chung; parse cookie an toàn; lấy recent async ngoài Razor. |
| F03 | Thiếu dòng, MinPrice, khoảng năm | Đã thêm dòng, MinPrice/MaxPrice, năm, BodyType và hộp số; giữ lọc khi paging, SQL Skip/Take ổn định. |
| F04 | Chỉ có giỏ 20 xe | Có Compare tối đa 3 xe, độc lập với giỏ quan tâm; đọc lại giá/trạng thái từ DB. |
| F05 | Có yêu cầu mua | Có API tạo/đọc theo chủ sở hữu, trạng thái Đang xử lý; form tư vấn/báo giá lưu DB riêng. |
| F06 | Dòng chỉ thêm, ảnh chỉ URL chính | Có sửa/ngừng dùng/xóa dòng chưa tham chiếu; bộ ảnh có upload, URL, ảnh chính, thứ tự và xóa liên kết. |
| F07 | Chỉ nhập một xe ngay | Có CRUD khách trực tiếp/đối tác, phiếu nhập nháp nhiều xe, sửa/xóa nháp và xác nhận nguyên tử. |
| F08 | Chỉ bán từ yêu cầu, không nháp | Có bán trực tiếp/nháp, xác nhận giữ, cọc 2%, thu/hoàn/hủy/bàn giao; nháp chưa giữ xe. |
| F09 | Chưa đổi role, xóa đổi tác giả | Có đổi role, mật khẩu riêng, kiểm soát admin cuối và vô hiệu hóa tài khoản có lịch sử. |
| F10 | Chỉ thống kê toàn thời gian | Có ngày UTC+7, doanh thu bàn giao, thu−hoàn, giá nhập, lãi gộp, phải thu nhóm giao dịch trong kỳ; tồn là số hiện tại. |

## Quy tắc dữ liệu

- BR01: VIN được Trim/Upper, regex định dạng và UNIQUE. Không gọi regex là chứng nhận toàn bộ ISO.
- BR02: giá nguyên VND, giá/khối lượng hợp lệ, năm không vượt hiện tại+1. Xe mới quy ước tối đa 100 km cho dữ liệu demo; giá quá nhỏ để cọc làm tròn >0 bị chặn.
- BR03: chỉ nhận CarModelId hợp lệ; hãng suy ra qua dòng; dòng có xe không được đổi sang hãng khác.
- BR04: tập xe public dùng chung cho catalog, API, detail, cart, compare và recent; chỉ Available nhận yêu cầu.
- BR05: RowVersion + filtered unique index chống giữ trùng; không bỏ các constraint cũ.
- BR06: chốt AgreedPrice và RequiredDeposit tại máy chủ, không tính lại từ giá niêm yết.
- BR07: thu/hoàn >0 nguyên VND, không vượt số dư; buộc UPDATE UpdatedAt để luôn kiểm tra RowVersion dù timestamp trùng.
- BR08: đủ giá bán mới bàn giao, Delivery duy nhất, trạng thái và chứng từ lưu trong transaction.
- BR09: khóa chứng từ đã xác nhận; không thay EmployeeId/CreatedById khi vô hiệu hóa nhân viên có chứng từ.
- BR10: role controller, API kiểm tra sở hữu, CSRF trên thao tác ghi, không nhận CustomerId của người khác từ trình duyệt.

## Lỗi và tối ưu đã xử lý

1. Bỏ tên động khỏi inline JavaScript xác nhận tài khoản; dùng data attribute và event listener.
2. Parse cookie một lần với TryParse, giới hạn 5 Id; không để cookie hỏng làm lỗi view.
3. Đưa truy vấn DB ra khỏi Razor chi tiết, dùng async và chung điều kiện public.
4. Badge giỏ hiện lại ngay sau khi thêm xe đầu tiên.
5. Footer Sedan/SUV lọc BodyType thay vì tìm sai Keyword.
6. Không dùng mật khẩu reset mặc định công khai; kiểm tra kết quả Identity và transaction cho user/role.
7. Cài đặt lưu thật vào bảng ShowroomSettings, bỏ giao diện cọc 50 triệu/QR mẫu.
8. Form tư vấn gửi thật; loại bỏ admin.js prototype chỉ sửa DOM/alert.
9. Phân trang kho, đơn bán, phiếu nhập, khách, đối tác và tài khoản; API xe không tải danh sách hãng thừa.
10. Giới hạn read keys thông báo theo khoảng thời gian, link thông báo tìm đúng chứng từ khi có paging. Feed vẫn dựng sự kiện 30 ngày trong RAM; chưa thay bằng outbox/SignalR.
11. Bootstrap/jQuery dùng file local; giữ màu/nền động hiện tại.
12. Tạo migration bổ sung và SQL full/incremental; không dùng EnsureDeleted trong ứng dụng hoặc tự reset DB người dùng.
13. Giữ code thư viện và license upstream; xóa chú thích mã nhóm bằng parser, giữ URL/string/directive. Chuẩn hóa khoảng trắng C#.

## Kết quả kiểm tra và giới hạn

- Web build thành công, 0 lỗi và 0 cảnh báo.
- 50 test đạt, 0 lỗi; 1 test SQL Server được skip khi không có biến AUTORA_TEST_SQLSERVER.
- Có HTTP integration test: đăng nhập, 10 trang quản trị, cấu hình, phiếu nhập hai xe, xác nhận bán nháp, cọc, chặn bàn giao thiếu tiền, thu đủ/bàn giao, so sánh, hồ sơ, API theo chủ sở hữu và vô hiệu hóa người lập cũ mà giữ lịch sử.
- Test SQLite dùng converter phục vụ test tiền nguyên VND/thời gian và mô phỏng version. Không thay kiểu tiền hoặc provider production.
- EF kiểm tra model/snapshot và sinh script SQL. Sinh script không chứng minh migration đã chạy trên SQL Server thật.
- Chưa nghiệm thu UI qua browser ba độ rộng, hiệu năng <2 giây hoặc dữ liệu DB thực của nhóm. Danh sách T01–T16 trong tài liệu không tự coi tất cả là Pass vì các test tự động mới.

Đây là bản hoàn thiện kỹ thuật để nhóm chạy và nghiệm thu, không phải chứng nhận thương mại hay lời khẳng định “100% không còn lỗi”.

## Các mục chưa triển khai vì ngoài phạm vi cốt lõi đã chọn

Phiếu chi nhập (PurchasePayment), audit log chi tiết trước/sau, quốc tế hóa toàn bộ admin/validation, tự hết hạn giữ xe, giữ lại cọc/phạt hủy, email, cổng thanh toán, ký gửi, trả góp và nhiều showroom. Bản Word của nhóm còn cần cập nhật ảnh thực tế, thông tin nhóm, UML/sitemap/wireframe xuất ảnh, số test và các nhận định quá mức.
