// candidate-interviews.js — Row selection, go toggle, confirm delete, add validation, custom owner
import { showToast } from '/js/toast.js';
import { confirmDelete } from '/js/confirm-modal.js';

const list = document.querySelector('[data-interview-list]');
const addForm = document.querySelector('[data-add-interview-form]');
const editForm = document.querySelector('[data-interview-edit] form');

// ───── CUSTOM OWNER HELPER ─────

function initCustomOwner(selectEl, inputEl) {
    if (!selectEl || !inputEl) return;

    selectEl.addEventListener('change', () => {
        if (selectEl.value === '__custom__') {
            inputEl.style.display = '';
            inputEl.focus();
        } else {
            inputEl.style.display = 'none';
            inputEl.value = '';
        }
    });
}

function injectCustomOwner(selectEl, inputEl) {
    if (!selectEl || selectEl.value !== '__custom__') return true;

    const custom = inputEl.value.trim();
    if (!custom) {
        inputEl.classList.add('error');
        showToast('Interviewer name is required', 'warning');
        inputEl.focus();
        return false;
    }

    const opt = document.createElement('option');
    opt.value = custom;
    opt.selected = true;
    selectEl.appendChild(opt);
    return true;
}

// ───── ADD FORM — CUSTOM OWNER ─────

const addSelect = addForm?.querySelector('[data-owner-select]');
const addCustom = addForm?.querySelector('[data-custom-owner]');
initCustomOwner(addSelect, addCustom);

// ───── EDIT FORM — CUSTOM OWNER ─────

const editSelect = editForm?.querySelector('[data-owner-select-edit]');
const editCustom = editForm?.querySelector('[data-custom-owner-edit]');
initCustomOwner(editSelect, editCustom);

// ───── ROW CLICK → SELECT (navigate to edit) ─────

if (list) {
    list.addEventListener('click', (e) => {
        const row = e.target.closest('.iv-list-row');
        if (!row) return;

        if (e.target.closest('[data-toggle-go]') ||
            e.target.closest('[data-delete-interview]') ||
            e.target.closest('a')) return;

        const href = row.dataset.href;
        if (href) window.location.href = href;
    });
}

// ───── GO CHECKBOX → SUBMIT FORM ─────

if (list) {
    list.addEventListener('change', (e) => {
        const cb = e.target.closest('[data-toggle-go]');
        if (!cb) return;

        const form = cb.closest('form');
        if (form) form.submit();
    });
}

// ───── DELETE WITH CONFIRM ─────

if (list) {
    list.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-delete-interview]');
        if (!btn) return;

        e.stopPropagation();

        const name = btn.dataset.deleteName || 'this interview';
        const ok = await confirmDelete(`Delete "${name}"? This cannot be undone.`);
        if (!ok) return;

        const id = btn.dataset.deleteId;
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Candidate/DeleteInterview';

        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'id';
        input.value = id;

        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
    });
}

// ───── ADD FORM VALIDATION ─────

if (addForm) {
    addForm.addEventListener('submit', (e) => {
        addForm.querySelectorAll('.error').forEach(el => el.classList.remove('error'));

        // Handle custom owner injection
        if (!injectCustomOwner(addSelect, addCustom)) {
            e.preventDefault();
            return;
        }

        const errors = [];

        addForm.querySelectorAll('[data-required]').forEach(field => {
            if (!field.value.trim() || field.value === '__custom__') {
                if (field === addSelect && addCustom?.value.trim()) return;
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

    addForm.addEventListener('change', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });

    addForm.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}

// ───── EDIT FORM — CUSTOM OWNER ON SUBMIT ─────

if (editForm) {
    editForm.addEventListener('submit', (e) => {
        if (!injectCustomOwner(editSelect, editCustom)) {
            e.preventDefault();
        }
    });

    editForm.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}