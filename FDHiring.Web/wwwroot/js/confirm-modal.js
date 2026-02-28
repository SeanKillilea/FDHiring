/* =============================================
   Confirm Modal
   Usage:
     import { confirmDelete } from '/js/confirm-modal.js';
     const ok = await confirmDelete('Delete this file permanently?');
     if (ok) { ... }
   ============================================= */

const overlay = document.querySelector('[data-modal="confirm-delete"]');
const messageEl = overlay.querySelector('[data-confirm-message]');
const yesBtn = overlay.querySelector('[data-confirm-yes]');
const closeBtns = overlay.querySelectorAll('[data-modal-close]');

let resolveFn = null;

function open() {
    overlay.classList.add('open');
}

function close(result) {
    overlay.classList.remove('open');
    if (resolveFn) {
        resolveFn(result);
        resolveFn = null;
    }
}

// Close buttons (Cancel + X)
closeBtns.forEach(btn => btn.addEventListener('click', () => close(false)));

// Confirm button
yesBtn.addEventListener('click', () => close(true));

// Click overlay backdrop to cancel
overlay.addEventListener('click', (e) => {
    if (e.target === overlay) close(false);
});

// Escape key to cancel
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && overlay.classList.contains('open')) close(false);
});

/**
 * Show confirm modal and return a promise.
 * @param {string} message - The confirmation message to display
 * @returns {Promise<boolean>} - true if confirmed, false if cancelled
 */
export function confirmDelete(message) {
    messageEl.textContent = message || 'Are you sure? This action cannot be undone.';
    open();
    return new Promise((resolve) => {
        resolveFn = resolve;
    });
}