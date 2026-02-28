// candidate-add.js — Validation for add candidate form
import { showToast } from '/js/toast.js';

const form = document.querySelector('[data-add-form]');

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
            const firstError = form.querySelector('.error');
            if (firstError) firstError.focus();
        }
    });

    // Clear error highlight on input/change
    form.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });

    form.addEventListener('change', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}