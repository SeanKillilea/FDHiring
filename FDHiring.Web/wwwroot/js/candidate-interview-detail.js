// candidate-interview-detail.js — Add question validation, delete confirm, save all
import { showToast } from '/js/toast.js';
import { confirmDelete } from '/js/confirm-modal.js';

const detailForm = document.querySelector('[data-detail-form]');
const addForm = document.querySelector('[data-add-question-form]');
const qaList = document.querySelector('[data-qa-list]');
const saveAllBtn = document.querySelector('[data-save-all]');

// ───── DRAG & DROP REORDER Q&A ─────

if (qaList) {
    let dragItem = null;
    let placeholder = null;

    const items = () => [...qaList.querySelectorAll('.qa-item')];

    qaList.addEventListener('mousedown', (e) => {
        const handle = e.target.closest('.qa-handle');
        if (!handle) return;

        dragItem = handle.closest('.qa-item');
        if (!dragItem) return;

        e.preventDefault();

        placeholder = document.createElement('div');
        placeholder.className = 'qa-item';
        placeholder.style.border = '2px dashed var(--clr-primary-light)';
        placeholder.style.background = 'var(--clr-primary-bg)';
        placeholder.style.height = dragItem.offsetHeight + 'px';
        placeholder.style.borderRadius = 'var(--radius)';

        dragItem.style.opacity = '0.5';

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });

    function onMouseMove(e) {
        if (!dragItem) return;

        const allItems = items();
        const mouseY = e.clientY;

        let target = null;
        for (const item of allItems) {
            if (item === dragItem || item === placeholder) continue;
            const rect = item.getBoundingClientRect();
            if (mouseY < rect.top + rect.height / 2) {
                target = item;
                break;
            }
        }

        if (placeholder.parentNode) placeholder.remove();

        if (target) {
            qaList.insertBefore(placeholder, target);
        } else {
            qaList.appendChild(placeholder);
        }
    }

    function onMouseUp() {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        if (!dragItem) return;

        if (placeholder.parentNode) {
            qaList.insertBefore(dragItem, placeholder);
            placeholder.remove();
        }

        dragItem.style.opacity = '';
        placeholder = null;

        // Update question numbers
        items().forEach((item, i) => {
            const num = item.querySelector('.qa-number');
            if (num) num.textContent = i + 1;
        });

        // POST new order
        const reorder = items().map((item, i) => ({
            id: parseInt(item.dataset.qaId),
            stepOrder: i + 1
        }));

        fetch('/Candidate/ReorderAnswers', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(reorder)
        }).then(res => {
            if (res.ok) {
                showToast('Order updated', 'success');
            } else {
                showToast('Failed to save order', 'danger');
            }
        }).catch(() => {
            showToast('Failed to save order', 'danger');
        });

        dragItem = null;
    }
}

// ───── SAVE ALL → SUBMIT THE DETAIL FORM ─────

if (saveAllBtn && detailForm) {
    saveAllBtn.addEventListener('click', (e) => {
        e.preventDefault();
        detailForm.submit();
    });
}

// ───── ADD QUESTION VALIDATION ─────

if (addForm) {
    addForm.addEventListener('submit', (e) => {
        addForm.querySelectorAll('.error').forEach(el => el.classList.remove('error'));

        const errors = [];

        addForm.querySelectorAll('[data-required]').forEach(field => {
            if (!field.value.trim()) {
                errors.push(field.dataset.required + ' is required');
                field.classList.add('error');
            }
        });

        if (errors.length) {
            e.preventDefault();
            showToast(errors[0], 'warning');
            const first = addForm.querySelector('.error');
            if (first) first.focus();
        }
    });

    addForm.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}

// ───── DELETE QUESTION WITH CONFIRM ─────

if (qaList) {
    qaList.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-delete-answer]');
        if (!btn) return;

        e.preventDefault();
        e.stopPropagation();

        const name = btn.dataset.deleteName || 'this question';
        const ok = await confirmDelete(`Delete "${name}"? This cannot be undone.`);
        if (!ok) return;

        const id = btn.dataset.deleteId;
        const interviewId = btn.dataset.deleteInterview;

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Candidate/DeleteInterviewAnswer';

        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = id;

        const ivInput = document.createElement('input');
        ivInput.type = 'hidden';
        ivInput.name = 'interviewId';
        ivInput.value = interviewId;

        form.appendChild(idInput);
        form.appendChild(ivInput);
        document.body.appendChild(form);
        form.submit();
    });
}