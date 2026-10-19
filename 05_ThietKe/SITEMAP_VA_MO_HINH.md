# Sơ đồ và cấu trúc bản cuối

USE_CASE.puml dùng ký pháp PlantUML với actor, use case và tổng quát hóa Customer→Guest, Admin→Staff. Đăng nhập là tiền điều kiện của thao tác bảo vệ, không ép include đăng nhập lại ở mọi use case.

## Sitemap theo đường dẫn thực

| Khu | Trang |
| --- | --- |
| Công khai | /Home, /Home/About, /Car, /Car/Details/{id}, /Compare, /Cart, /Contact, /Home/Information |
| Tài khoản | /Account/Register, /Account/Login, POST /Account/Logout |
| Khách đã đăng nhập | /Account/Profile, /Account/ChangePassword, /Deposit/Mine, POST /Deposit/Create |
| Nhân viên và Admin | /Admin/Home, /Admin/Car, /Admin/CarImage, /Admin/Manufacturer, /Admin/Customer, /Admin/Supplier, /Admin/Purchase, /Admin/Order, /Admin/Contact, /Admin/Notification |
| Chỉ Admin | /Admin/User, /Admin/Setting, API ghi hãng |

## Mô hình quan hệ

| Quan hệ | Ý nghĩa và ràng buộc |
| --- | --- |
| Manufacturer 1–N CarModel, CarModel 1–N Car | Không lưu lặp ManufacturerId trong Car. |
| Car 1–N CarImage | UNIQUE CarId khi IsPrimary=1, không phải unique toàn bộ CarId. |
| User 1–0..1 Customer | Customer.UserId nullable và unique khi có giá trị. |
| Supplier 1–N PurchaseReceipt 1–N PurchaseDetail | Xe có 0..1 chi tiết nhập; CarId unique trong phạm vi nhập một lần. |
| Customer và Car 1–N DepositRequest | Unique CustomerId+CarId cho yêu cầu New/Processing. |
| Car 1–N SaleTransaction | Chỉ unique CarId khi sale Status IN (1,2,3), vẫn có nhiều nháp/hủy lịch sử. |
| SaleTransaction 1–N PaymentEntry, 1–0..1 Delivery | Thu/hoàn append ledger; SaleId trong Delivery unique. |
| User 1–N AdminNotificationRead | Unique UserId+EventKey; không gộp trạng thái đọc giữa các nhân viên. |
| ShowroomSetting | Khóa Id=1 do ứng dụng chỉ định, không Identity; RowVersion chống ghi đè. |
| ContactInquiry | Yêu cầu tư vấn riêng, không khóa xe hoặc tạo hợp đồng. |

Model và SQL trong source là nguồn chuẩn về trường/ràng buộc. Cập nhật hình ERD của báo cáo theo hai bảng mới và trạng thái Processing=3; không đảo số enum cũ vì DB có thể đã có dữ liệu.

## Wireframe cấu trúc

Mở WIREFRAME.html để xem bố cục minh họa trang danh sách, chi tiết, phiếu nhập và giao dịch. Đây là thiết kế cấu trúc, không phải ảnh chụp hay bằng chứng kiểm thử UI thực tế.
