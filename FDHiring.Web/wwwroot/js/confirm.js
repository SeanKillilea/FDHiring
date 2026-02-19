/**
 * Confirm modal system
 * Usage:
 *   const yes = await Confirm.show('Delete this candidate?');
 *   if (yes) { ... }
 *
 *   // With custom options
 *   const yes = await Confirm.show('Remove this file?', {
 *       title: 'Delete File',
 *       confirmText: 'Remove',
 *       cancelText: 'Keep'
 *   });
 */

const Confirm = (() => {
    let modal, titleEl, messageEl, confirmBtn, cancelBtn;
    let resolvePromise = null;

    function init() {
        modal = document.querySelector('[data-confirm-modal]');
        titleEl = modal?.querySelector('[data-confirm-modal-title]');
        messageEl = modal?.querySelector('[data-confirm-modal-message]');
        confirmBtn = modal?.querySelector('[data-confirm-modal-confirm]');
        cancelBtn = modal?.querySelector('[data-confirm-modal-cancel]');

        if (!modal) return;

        confirmBtn.addEventListener('click', () => close(true));
        cancelBtn.addEventListener('click', () => close(false));

        // Close on overlay click
        modal.addEventListener('click', (e) => {
            if (e.target === modal) close(false);
        });

        // Close on Escape
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && !modal.classList.contains('hidden')) {
                close(false);
            }
        });
    }

    function show(message, options = {}) {
        if (!modal) init();

        const {
            title = 'Are you sure?',
            confirmText = 'Delete',
            cancelText = 'Cancel',
            confirmClass = 'btn btn-danger'
        } = options;

        titleEl.textContent = title;
        messageEl.textContent = message;
        confirmBtn.textContent = confirmText;
        confirmBtn.className = confirmClass;
        cancelBtn.textContent = cancelText;

        modal.classList.remove('hidden');

        return new Promise((resolve) => {
            resolvePromise = resolve;
        });
    }

    function close(result) {
        modal.classList.add('hidden');
        if (resolvePromise) {
            resolvePromise(result);
            resolvePromise = null;
        }
    }

    return { show };
})();