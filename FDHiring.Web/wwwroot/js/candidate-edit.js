// candidate-edit.js — Validation + confirm delete
import { showToast } from '/js/toast.js';
import { confirmDelete } from '/js/confirm-modal.js';

const form = document.querySelector('[data-edit-form]');
const deleteBtn = document.querySelector('[data-delete-candidate]');

// ───── FORM VALIDATION ─────

if (form) {
    form.addEventListener('submit', (e) => {
        // Clear previous error highlights
        form.querySelectorAll('.error').forEach(el => el.classList.remove('error'));

        const errors = [];

        // Check data-required fields
        form.querySelectorAll('[data-required]').forEach(field => {
            const val = field.value.trim();
            if (!val) {
                errors.push(field.dataset.required + ' is required');
                field.classList.add('error');
            }
        });

        // Check email format
        const emailField = form.querySelector('[data-email]');
        if (emailField && emailField.value.trim()) {
            const email = emailField.value.trim();
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                errors.push('Email format is invalid');
                emailField.classList.add('error');
            }
        }

        // Default date found to today if empty
        const dateField = form.querySelector('#dateFound');
        if (dateField && !dateField.value) {
            dateField.value = new Date().toISOString().split('T')[0];
        }

        if (errors.length) {
            e.preventDefault();
            showToast(errors[0], 'warning');
            // Focus first error field
            const firstError = form.querySelector('.error');
            if (firstError) firstError.focus();
        }
    });

    // Clear error highlight on input
    form.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) {
            e.target.classList.remove('error');
        }
    });

    form.addEventListener('change', (e) => {
        if (e.target.classList.contains('error')) {
            e.target.classList.remove('error');
        }
    });
}

// ───── DELETE WITH CONFIRM MODAL ─────

if (deleteBtn) {
    deleteBtn.addEventListener('click', async () => {
        const ok = await confirmDelete('Delete this candidate? This action cannot be undone.');
        if (!ok) return;

        const id = deleteBtn.dataset.deleteId;
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = deleteBtn.dataset.deleteAction;

        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'id';
        input.value = id;

        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
    });
}