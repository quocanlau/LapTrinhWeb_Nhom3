document.addEventListener('DOMContentLoaded', () => {
    const box = document.getElementById('purchaseLines');
    const template = document.getElementById('purchaseLineTemplate');
    const addButton = document.getElementById('addPurchaseLine');
    if (!box || !template || !addButton) return;
    function reindex() {
        Array.from(box.children).forEach((row, index) => {
            row.querySelectorAll('input,select').forEach(field => {
                const property = field.dataset.line || field.name.split('.').pop();
                field.dataset.line = property;
                field.name = `Lines[${index}].${property}`;
            });
        });
        addButton.disabled = box.children.length >= 50;
    }
    function decorate(row) {
        const remove = document.createElement('button');
        remove.type = 'button';
        remove.className = 'btn btn-sm btn-outline-danger w-auto';
        remove.textContent = 'Bỏ dòng xe';
        remove.addEventListener('click', () => { row.remove(); reindex(); });
        row.append(remove);
    }
    function add() {
        if (box.children.length >= 50) return;
        const firstSelect = template.content.querySelector('select');
        if (!firstSelect || firstSelect.options.length <= 1) return;
        box.append(template.content.cloneNode(true));
        decorate(box.lastElementChild);
        reindex();
    }
    Array.from(box.children).forEach(decorate);
    addButton.addEventListener('click', add);
    if (box.children.length === 0 && !addButton.disabled) add();
});
