$(document).ready(function () {

    // Hàm loại bỏ dấu tiếng Việt dùng chung cho toàn bộ cổng quản trị
    function removeVietnameseTones(str) {
        if (!str) return '';
        return str.normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd').replace(/Đ/g, 'd')
            .toLowerCase().trim();
    }


    // 1. Quản lý kho xe
    function updateCarKPIs() {
        var total = $('#carInventoryTable tbody tr').length;
        var booked = 0;
        var totalValue = 0;

        $('#carInventoryTable tbody tr').each(function () {
            var st = $(this).attr('data-status') || $(this).data('status');
            var price = parseFloat($(this).attr('data-price') || $(this).data('price')) || 0;
            if (st === 'Đã cọc') booked++;
            totalValue += price;
        });

        $('#kpiTotalCars').text(total);
        $('#kpiBookedCars').text(booked);

        var billions = (totalValue / 1000000000).toLocaleString('vi-VN', { maximumFractionDigits: 2 });
        $('#kpiTotalInventoryValue').text(billions + ' tỷ');
    }

    function executeCarSearch() {
        var search = removeVietnameseTones($('#carSearchInput').val());
        var brand = $('#filterBrandSelect').val();
        var status = $('#filterStatusSelect').val();

        $('#carInventoryTable tbody tr').each(function () {
            var rowBrand = $(this).attr('data-brand') || $(this).data('brand') || '';
            var rowStatus = $(this).attr('data-status') || $(this).data('status') || '';
            var name = removeVietnameseTones($(this).find('.car-title').text());
            var vin = removeVietnameseTones($(this).find('.car-vin').text());

            var matchSearch = (!search || name.includes(search) || vin.includes(search));
            var matchBrand = (!brand || rowBrand === brand);
            var matchStatus = (!status || rowStatus === status);

            if (matchSearch && matchBrand && matchStatus) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    $(document).on('click', '#btnSearchCar', function () {
        executeCarSearch();
    });

    $(document).on('keypress', '#carSearchInput', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            executeCarSearch();
        }
    });

    $('#carSearchInput').on('input', function () {
        executeCarSearch();
    });

    $('#filterBrandSelect, #filterStatusSelect').on('change', function () {
        executeCarSearch();
    });

    $('#btnResetCarFilter').on('click', function () {
        $('#carSearchInput').val('');
        $('#filterBrandSelect').val('');
        $('#filterStatusSelect').val('');
        $('#carInventoryTable tbody tr').show();
    });

    $(document).on('click', '.btn-delete-car', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        if (confirm('Bạn có chắc chắn muốn xóa xe [' + name + '] khỏi kho không?')) {
            $('#car-row-' + id).fadeOut(300, function () {
                $(this).remove();
                updateCarKPIs();
            });
        }
    });

    $(document).on('click', '.btn-edit-car', function () {
        var row = $(this).closest('tr');
        var id = $(this).data('id');
        var name = row.attr('data-name') || row.data('name');
        var vin = row.attr('data-vin') || row.data('vin');
        var price = row.attr('data-price') || row.data('price');
        var status = row.attr('data-status') || row.data('status');

        $('#editCarId').val(id);
        $('#editCarName').val(name);
        $('#editCarVin').val(vin);
        $('#editCarPrice').val(price);
        $('#editCarStatus').val(status);

        var modal = new bootstrap.Modal(document.getElementById('editCarModal'));
        modal.show();
    });

    $('#formEditCar').on('submit', function (e) {
        e.preventDefault();
        var id = $('#editCarId').val();
        var newPrice = Number($('#editCarPrice').val());
        var newStatus = $('#editCarStatus').val();

        var row = $('#car-row-' + id);
        row.attr('data-price', newPrice).data('price', newPrice);
        row.attr('data-status', newStatus).data('status', newStatus);

        row.find('.car-price-display').text(newPrice.toLocaleString('vi-VN') + ' đ');

        var badge = row.find('.car-status-badge');
        if (newStatus === 'Còn hàng') {
            badge.removeClass('bg-warning text-warning').addClass('bg-success text-success').text('● Còn hàng');
        } else {
            badge.removeClass('bg-success text-success').addClass('bg-warning text-warning').text('● Đã cọc');
        }

        bootstrap.Modal.getInstance(document.getElementById('editCarModal')).hide();
        updateCarKPIs();
        alert('🎉 Đã cập nhật thông tin xe thành công!');
    });

    $('#formAddCar').on('submit', function (e) {
        e.preventDefault();

        var newId = Date.now();
        var name = $('#addCarName').val().trim();
        var brand = $('#addCarBrand').val();
        var vin = $('#addCarVin').val().toUpperCase().trim();
        var year = $('#addCarYear').val();
        var price = Number($('#addCarPrice').val());
        var specs = $('#addCarSpecs').val().trim();
        var mileage = $('#addCarMileage').val().trim();
        var img = $('#addCarImg').val().trim() || 'https://images.unsplash.com/photo-1549399542-7e3f8b79c341?q=80&w=120';

        var newRowHtml = `
            <tr id="car-row-${newId}" data-brand="${brand}" data-status="Còn hàng" data-name="${name}" data-vin="${vin}" data-price="${price}">
                <td><img src="${img}" class="rounded-3 shadow-sm" style="width:68px; height:46px; object-fit:cover;" /></td>
                <td>
                    <div class="fw-bold text-dark fs-6 car-title">${name}</div>
                    <code class="text-secondary small car-vin">VIN: ${vin}</code>
                </td>
                <td>
                    <span class="badge bg-light text-dark border me-1">${brand}</span>
                    <span class="text-secondary small">${year}</span>
                </td>
                <td>
                    <div class="text-secondary small"><i class="bi bi-speedometer2 me-1"></i>${mileage}</div>
                    <div class="text-secondary small"><i class="bi bi-gear-wide me-1"></i>${specs}</div>
                </td>
                <td><strong class="text-dark fs-6 car-price-display">${price.toLocaleString('vi-VN')} đ</strong></td>
                <td><span class="badge bg-success bg-opacity-10 text-success rounded-pill px-3 py-1 car-status-badge">● Còn hàng</span></td>
                <td class="text-end">
                    <button type="button" class="btn btn-sm btn-outline-primary rounded-circle me-1 btn-edit-car" data-id="${newId}"><i class="bi bi-pencil-fill"></i></button>
                    <button type="button" class="btn btn-sm btn-outline-danger rounded-circle btn-delete-car" data-id="${newId}" data-name="${name}"><i class="bi bi-trash-fill"></i></button>
                </td>
            </tr>
        `;

        $('#carInventoryTable tbody').prepend(newRowHtml);
        bootstrap.Modal.getInstance(document.getElementById('addCarModal')).hide();
        $('#formAddCar')[0].reset();
        updateCarKPIs();
        alert('🎉 Đã thêm xe [' + name + '] vào kho thành công!');
    });


    // 2. Quản lý tài khoản
    function updateUserKPIs() {
        var active = 0;
        var admins = 0;

        $('#userTable tbody tr').each(function () {
            var st = $(this).attr('data-status') || $(this).data('status');
            var rl = $(this).attr('data-role') || $(this).data('role');

            if (st === 'Hoạt động') active++;
            if (rl === 'Quản trị viên') admins++;
        });

        $('#kpiTotalUsers').text($('#userTable tbody tr').length);
        $('#kpiActiveUsers').text(active);
        $('#kpiAdminUsers').text(admins);
    }

    // Hàm tìm kiếm nhân sự kết hợp cả Tên, Mã ID, Email, SĐT
    function executeUserSearch() {
        var keyword = $('#userSearchInput').val().trim();
        var cleanKeyword = removeVietnameseTones(keyword).replace('#', '').replace('-', '');
        var role = $('#filterRoleSelect').val();
        var status = $('#filterUserStatusSelect').val();

        $('#userTable tbody tr').each(function () {
            var rowRole = $(this).attr('data-role') || $(this).data('role') || '';
            var rowStatus = $(this).attr('data-status') || $(this).data('status') || '';
            
            var rowIdRaw = ($(this).attr('data-id-code') || $(this).find('.user-id').text() || '').toLowerCase();
            var rowIdClean = rowIdRaw.replace('#', '').replace('-', '');

            var name = $(this).find('.user-name').text();
            var email = $(this).find('.user-email').text();
            var phone = $(this).find('.user-phone').text();

            var cleanName = removeVietnameseTones(name);
            var cleanEmail = removeVietnameseTones(email);
            var cleanPhone = removeVietnameseTones(phone);

            var matchSearch = (!keyword || 
                               cleanName.includes(cleanKeyword) || 
                               rowIdClean.includes(cleanKeyword) || 
                               cleanEmail.includes(cleanKeyword) || 
                               cleanPhone.includes(cleanKeyword));
            var matchRole = (!role || rowRole === role);
            var matchStatus = (!status || rowStatus === status);

            if (matchSearch && matchRole && matchStatus) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    $(document).on('click', '#btnSearchUser', function () {
        executeUserSearch();
    });

    $(document).on('keypress', '#userSearchInput', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            executeUserSearch();
        }
    });

    $('#userSearchInput').on('input', function () {
        executeUserSearch();
    });

    $('#filterRoleSelect, #filterUserStatusSelect').on('change', function () {
        executeUserSearch();
    });

    $('#btnResetUserFilter').on('click', function () {
        $('#userSearchInput').val('');
        $('#filterRoleSelect').val('');
        $('#filterUserStatusSelect').val('');
        $('#userTable tbody tr').show();
    });

    $(document).on('click', '.btn-toggle-lock', function () {
        var row = $(this).closest('tr');
        var badge = row.find('.user-status-badge');
        var currentStatus = row.attr('data-status') || row.data('status');

        if (currentStatus === 'Hoạt động') {
            row.attr('data-status', 'Tạm khóa').data('status', 'Tạm khóa');
            badge.removeClass('bg-success text-success').addClass('bg-danger text-danger').text('● Tạm khóa');
            $(this).removeClass('btn-outline-warning').addClass('btn-outline-success').html('<i class="bi bi-unlock-fill"></i>').attr('title', 'Mở khóa tài khoản');
        } else {
            row.attr('data-status', 'Hoạt động').data('status', 'Hoạt động');
            badge.removeClass('bg-danger text-danger').addClass('bg-success text-success').text('● Hoạt động');
            $(this).removeClass('btn-outline-success').addClass('btn-outline-warning').html('<i class="bi bi-slash-circle"></i>').attr('title', 'Tạm khóa tài khoản');
        }
        updateUserKPIs();
    });

    $(document).on('click', '.btn-delete-user', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        if (confirm('Bạn có chắc chắn muốn xóa tài khoản của [' + name + '] không?')) {
            $('#user-row-' + id).fadeOut(300, function () {
                $(this).remove();
                updateUserKPIs();
            });
        }
    });

    $('#formAddUser').on('submit', function (e) {
        e.preventDefault();
        var name = $('#addUserName').val().trim();
        var email = $('#addUserEmail').val().trim();
        var phone = $('#addUserPhone').val().trim();
        var role = $('#addUserRole').val();
        var newId = Date.now();
        var userCode = 'NV-' + Math.floor(100 + Math.random() * 900);

        var words = name.split(' ');
        var initials = words.length > 1 ? (words[0][0] + words[words.length - 1][0]).toUpperCase() : name.substring(0, 2).toUpperCase();

        var roleBadgeClass = role === 'Quản trị viên' ? 'bg-dark text-white' : (role === 'Quản lý kho' ? 'bg-primary bg-opacity-10 text-primary' : 'bg-info bg-opacity-10 text-info');

        var newRow = `
            <tr id="user-row-${newId}" data-id-code="${userCode}" data-name="${name}" data-role="${role}" data-status="Hoạt động">
                <td>
                    <div class="d-flex align-items-center gap-3">
                        <div class="rounded-circle text-white fw-bold d-flex align-items-center justify-content-center bg-dark" style="width:42px; height:42px; font-size:1rem;">${initials}</div>
                        <div>
                            <div class="fw-bold text-dark user-name">${name}</div>
                            <span class="text-secondary small user-id">ID: #${userCode}</span>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="text-dark small fw-semibold user-email">${email}</div>
                    <span class="text-secondary small user-phone">${phone}</span>
                </td>
                <td><span class="badge ${roleBadgeClass} rounded-pill px-3 py-1 user-role-badge">● ${role}</span></td>
                <td><span class="badge bg-success bg-opacity-10 text-success rounded-pill px-3 py-1 user-status-badge">● Hoạt động</span></td>
                <td><span class="text-secondary small">Vừa tạo</span></td>
                <td class="text-end">
                    <button class="btn btn-sm btn-outline-warning rounded-circle me-1 btn-toggle-lock" data-id="${newId}" title="Tạm khóa tài khoản"><i class="bi bi-slash-circle"></i></button>
                    <button class="btn btn-sm btn-outline-danger rounded-circle btn-delete-user" data-id="${newId}" data-name="${name}" title="Xóa"><i class="bi bi-trash-fill"></i></button>
                </td>
            </tr>
        `;

        $('#userTable tbody').prepend(newRow);
        bootstrap.Modal.getInstance(document.getElementById('addUserModal')).hide();
        $('#formAddUser')[0].reset();
        updateUserKPIs();
        alert('🎉 Đã tạo tài khoản nhân viên thành công!');
    });


    // 3. Cài đặt hệ thống
    $('#formShowroomSetting').on('submit', function (e) {
        e.preventDefault();
        alert('🎉 Đã lưu thông tin Showroom AUTORA thành công!');
    });

    $('#formBankingSetting').on('submit', function (e) {
        e.preventDefault();
        alert('🎉 Đã cập nhật tài khoản nhận cọc VietQR thành công!');
    });

    $('#formPolicySetting').on('submit', function (e) {
        e.preventDefault();
        alert('🎉 Đã cập nhật chính sách bảo hành showroom!');
    });


    // 4. Duyệt đơn đặt cọc
    function executeOrderSearch() {
        var keyword = $('#orderSearchInput').val().trim();
        var cleanKeyword = removeVietnameseTones(keyword);

        $('#orderTable tbody tr').each(function () {
            var rawText = $(this).text();
            var cleanText = removeVietnameseTones(rawText);

            if (!keyword || cleanText.includes(cleanKeyword)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    $(document).on('click', '#btnSearchOrder', function () {
        executeOrderSearch();
    });

    $(document).on('keypress', '#orderSearchInput', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            executeOrderSearch();
        }
    });

    $('#orderSearchInput').on('input', function () {
        executeOrderSearch();
    });

    $(document).on('click', '#btnResetOrderFilter', function () {
        $('#orderSearchInput').val('');
        $('#orderTable tbody tr').show();
    });

    $(document).on('click', '.btn-approve-order', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        if (confirm('Xác nhận DUYỆT đơn đặt cọc #' + id + ' của khách hàng [' + name + ']?')) {
            $('#order-row-' + id).fadeOut(300, function () {
                $(this).remove();
                var count = parseInt($('#kpiPendingOrders').text()) || 0;
                $('#kpiPendingOrders').text(Math.max(0, count - 1));
                $('.admin-menu-badge').text(Math.max(0, count - 1));
            });
            alert('🎉 Đã duyệt đơn cọc thành công! Hệ thống đã ghi nhận xe chuyển sang trạng thái ĐÃ CỌC.');
        }
    });

    $(document).on('click', '.btn-reject-order', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        if (confirm('Bạn có chắc chắn muốn HỦY đơn cọc #' + id + ' của khách hàng [' + name + ']?')) {
            $('#order-row-' + id).fadeOut(300, function () {
                $(this).remove();
                var count = parseInt($('#kpiPendingOrders').text()) || 0;
                $('#kpiPendingOrders').text(Math.max(0, count - 1));
                $('.admin-menu-badge').text(Math.max(0, count - 1));
            });
        }
    });


    // 5. Chuông thông báo
    $('#btnMarkRead').on('click', function (e) {
        e.preventDefault();
        $('#notifBadge').fadeOut(200);
        alert('🎉 Đã đánh dấu đọc toàn bộ thông báo!');
    });


    // 6. Quản lý hãng xe
    function updateBrandKPIs() {
        var totalBrands = $('#brandGrid .brand-card-col').length;
        var totalCars = 0;
        var countries = [];

        $('#brandGrid .brand-card-col').each(function () {
            var count = parseInt($(this).attr('data-count') || $(this).data('count')) || 0;
            var country = ($(this).attr('data-country') || $(this).data('country') || '').trim();
            totalCars += count;
            if (country && !countries.includes(country)) {
                countries.push(country);
            }
        });

        $('#kpiTotalBrands').text(totalBrands);
        $('#kpiTotalBrandCars').text(totalCars);
        $('#kpiTotalCountries').text(countries.length);
    }

    function executeBrandSearch() {
        var keyword = $('#brandSearchInput').val().trim();
        var cleanKeyword = removeVietnameseTones(keyword);

        $('#brandGrid .brand-card-col').each(function () {
            var name = $(this).find('.brand-name').text();
            var country = $(this).find('.brand-country').text();

            var cleanName = removeVietnameseTones(name);
            var cleanCountry = removeVietnameseTones(country);

            if (!keyword || cleanName.includes(cleanKeyword) || cleanCountry.includes(cleanKeyword)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    $(document).on('click', '#btnSearchBrand', function () {
        executeBrandSearch();
    });

    $(document).on('keypress', '#brandSearchInput', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            executeBrandSearch();
        }
    });

    $('#brandSearchInput').on('input', function () {
        executeBrandSearch();
    });

    $(document).on('click', '#btnResetBrandFilter', function () {
        $('#brandSearchInput').val('');
        $('#brandGrid .brand-card-col').show();
    });

    $(document).on('click', '.btn-edit-brand', function () {
        var col = $(this).closest('.brand-card-col');
        var id = col.data('id');
        var name = col.attr('data-name') || col.data('name');
        var country = col.attr('data-country') || col.data('country');
        var count = col.attr('data-count') || col.data('count');

        $('#editBrandId').val(id);
        $('#editBrandName').val(name);
        $('#editBrandCountry').val(country);
        $('#editBrandCount').val(count);

        var modal = new bootstrap.Modal(document.getElementById('editBrandModal'));
        modal.show();
    });

    $('#formEditBrand').on('submit', function (e) {
        e.preventDefault();
        var id = $('#editBrandId').val();
        var name = $('#editBrandName').val().trim();
        var country = $('#editBrandCountry').val().trim();
        var count = parseInt($('#editBrandCount').val()) || 0;

        var col = $('#brand-col-' + id);
        col.attr('data-name', name).data('name', name);
        col.attr('data-country', country).data('country', country);
        col.attr('data-count', count).data('count', count);

        col.find('.brand-name').text(name);
        col.find('.brand-country').text(country);
        col.find('.brand-count-badge').text('● ' + count + ' xe phân phối');

        bootstrap.Modal.getInstance(document.getElementById('editBrandModal')).hide();
        updateBrandKPIs();
        alert('🎉 Đã cập nhật thông tin thương hiệu [' + name + '] thành công!');
    });

    $(document).on('click', '.btn-delete-brand', function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        if (confirm('Bạn có chắc chắn muốn xóa hãng xe [' + name + '] khỏi hệ thống không?')) {
            $('#brand-col-' + id).fadeOut(300, function () {
                $(this).remove();
                updateBrandKPIs();
            });
        }
    });

    $('#formAddBrand').on('submit', function (e) {
        e.preventDefault();
        var name = $('#addBrandName').val().trim();
        var country = $('#addBrandCountry').val().trim();
        var count = parseInt($('#addBrandCount').val()) || 0;
        var newId = Date.now();

        var colors = ['text-primary', 'text-info', 'text-danger', 'text-warning', 'text-success', 'text-dark'];
        var randomColor = colors[Math.floor(Math.random() * colors.length)];

        var newCol = `
            <div class="col-md-6 col-lg-3 brand-card-col" id="brand-col-${newId}" data-id="${newId}" data-name="${name}" data-country="${country}" data-count="${count}">
                <div class="card h-100 border rounded-4 shadow-sm position-relative overflow-hidden brand-card p-3 text-center bg-light">
                    <div class="dropdown position-absolute top-0 end-0 m-2">
                        <button class="btn btn-sm btn-light rounded-circle shadow-sm" data-bs-toggle="dropdown">
                            <i class="bi bi-three-dots-vertical"></i>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end border-0 shadow-sm rounded-3">
                            <li><a class="dropdown-item btn-edit-brand" href="javascript:void(0)" data-id="${newId}"><i class="bi bi-pencil me-2 text-primary"></i>Chỉnh sửa</a></li>
                            <li><a class="dropdown-item btn-delete-brand text-danger" href="javascript:void(0)" data-id="${newId}" data-name="${name}"><i class="bi bi-trash me-2"></i>Xóa hãng</a></li>
                        </ul>
                    </div>
                    <div class="d-inline-flex align-items-center justify-content-center rounded-circle mx-auto my-3 bg-white shadow-sm" style="width:70px; height:70px;">
                        <i class="bi bi-award fs-1 ${randomColor}"></i>
                    </div>
                    <h5 class="fw-bold mb-1 text-dark brand-name">${name}</h5>
                    <div class="text-secondary small mb-2"><i class="bi bi-geo-alt me-1"></i>Xuất xứ: <span class="fw-semibold text-dark brand-country">${country}</span></div>
                    <div class="mt-auto pt-2 border-top">
                        <span class="badge bg-dark bg-opacity-10 text-dark rounded-pill px-3 py-1 brand-count-badge">● ${count} xe phân phối</span>
                    </div>
                </div>
            </div>
        `;

        $('#brandGrid').prepend(newCol);
        bootstrap.Modal.getInstance(document.getElementById('addBrandModal')).hide();
        $('#formAddBrand')[0].reset();
        updateBrandKPIs();
        alert('🎉 Đã thêm thương hiệu [' + name + '] thành công!');
    });

});