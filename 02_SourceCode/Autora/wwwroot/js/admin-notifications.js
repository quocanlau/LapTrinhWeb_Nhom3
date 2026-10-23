(function () {
    'use strict';
    const bell = document.getElementById('notificationBell');
    if (!bell) return;
    const badge = document.getElementById('notificationBadge');
    const list = document.getElementById('notificationList');
    const status = document.getElementById('notificationStatus');
    const readAll = document.getElementById('notificationReadAll');
    let feedRequest;
    let unreadCount = 0;
    let changing = false;
    const csrf = () => document.querySelector('meta[name="csrf-token"]')?.content || '';
    async function request(url, data) {
        const response = await fetch(url, {
            method: 'POST', credentials: 'same-origin',
            headers: { 'X-CSRF-TOKEN': csrf(), 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams(data || {})
        });
        if (!response.ok || response.redirected) throw new Error('Không lưu được trạng thái đọc. Hãy tải lại trang và kiểm tra đăng nhập.');
        return response;
    }
    function render(data) {
        unreadCount = data.unreadCount;
        badge.hidden = unreadCount === 0;
        badge.textContent = unreadCount > 99 ? '99+' : String(unreadCount);
        bell.setAttribute('aria-label', `Thông báo quản trị: ${unreadCount} chưa đọc`);
        readAll.disabled = unreadCount === 0 || changing;
        list.replaceChildren();
        status.textContent = data.items.length === 0 ? 'Chưa có hoạt động trong 30 ngày gần nhất.' : `${unreadCount} thông báo chưa đọc`;
        data.items.forEach(item => {
            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'notification-item' + (item.isRead ? '' : ' is-unread');
            const heading = document.createElement('strong');
            heading.textContent = item.title;
            const detail = document.createElement('span');
            detail.textContent = item.message;
            const time = document.createElement('time');
            time.dateTime = item.createdAt;
            time.textContent = new Date(item.createdAt).toLocaleString('vi-VN', { dateStyle: 'short', timeStyle: 'short' });
            button.append(heading, detail, time);
            button.addEventListener('click', async () => {
                if (changing) return;
                changing = true;
                button.disabled = true;
                readAll.disabled = true;
                feedRequest?.abort();
                try {
                    const response = await request('/Admin/Notification/Read', { key: item.key });
                    const result = await response.json();
                    const target = new URL(result.url, window.location.origin);
                    if (target.origin !== window.location.origin || !target.pathname.startsWith('/Admin/')) throw new Error('Đường dẫn thông báo không hợp lệ.');
                    window.location.assign(target.href);
                } catch (error) {
                    status.textContent = error.message;
                    button.disabled = false;
                    changing = false;
                    readAll.disabled = unreadCount === 0;
                }
            });
            list.append(button);
        });
    }
    async function refresh() {
        if (changing) return;
        feedRequest?.abort();
        const controller = new AbortController();
        feedRequest = controller;
        try {
            const response = await fetch('/Admin/Notification/Feed', { credentials: 'same-origin', cache: 'no-store', signal: controller.signal });
            if (!response.ok || response.redirected || !response.headers.get('content-type')?.includes('application/json')) throw new Error('Không tải được thông báo. Kiểm tra đăng nhập, kết nối và cập nhật database.');
            const data = await response.json();
            if (!controller.signal.aborted) render(data);
        } catch (error) {
            if (error.name !== 'AbortError') { status.textContent = error.message; badge.hidden = true; }
        }
    }
    readAll.addEventListener('click', async () => {
        if (changing) return;
        changing = true;
        readAll.disabled = true;
        feedRequest?.abort();
        try {
            await request('/Admin/Notification/ReadAll');
            changing = false;
            await refresh();
        } catch (error) {
            status.textContent = error.message;
            changing = false;
            readAll.disabled = unreadCount === 0;
        }
    });
    bell.addEventListener('show.bs.dropdown', refresh);
    document.addEventListener('visibilitychange', () => { if (!document.hidden) refresh(); });
    window.setInterval(() => { if (!document.hidden) refresh(); }, 30000);
    refresh();
}());
