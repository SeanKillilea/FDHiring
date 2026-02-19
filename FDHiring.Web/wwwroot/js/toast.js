/**
 * Toast notification system
 * Usage:
 *   Toast.success('Saved!');
 *   Toast.error('Something went wrong');
 *   Toast.warning('Are you sure?');
 *   Toast.info('Loading...', 2000);
 *   
 *   // Loading pattern
 *   const t = Toast.loading('Saving...');
 *   Toast.update(t, 'Saved!', 'success');
 */

const Toast = (() => {
    const TYPES = {
        success: '✓',
        error: '✕',
        warning: '⚠',
        info: 'ℹ'
    };

    const DURATION = 3000;
    let container = null;

    function initContainer() {
        if (container) return;
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    function show(message, type = 'info', duration = DURATION) {
        initContainer();

        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;

        const icon = document.createElement('span');
        icon.className = 'toast-icon';
        icon.textContent = TYPES[type] || TYPES.info;

        const text = document.createElement('span');
        text.className = 'toast-message';
        text.textContent = message;

        const closeBtn = document.createElement('button');
        closeBtn.className = 'toast-close';
        closeBtn.innerHTML = '×';
        closeBtn.onclick = () => remove(toast);

        toast.appendChild(icon);
        toast.appendChild(text);
        toast.appendChild(closeBtn);
        container.appendChild(toast);

        requestAnimationFrame(() => {
            toast.classList.add('toast-show');
        });

        if (duration > 0) {
            setTimeout(() => {
                remove(toast);
            }, duration);
        }

        return toast;
    }

    function remove(toast) {
        toast.classList.remove('toast-show');
        toast.classList.add('toast-hide');

        setTimeout(() => {
            toast.remove();
            if (container && container.children.length === 0) {
                container.remove();
                container = null;
            }
        }, 300);
    }

    function update(toast, message, type, duration = DURATION) {
        const icon = toast.querySelector('.toast-icon');
        const text = toast.querySelector('.toast-message');

        icon.textContent = TYPES[type] || TYPES.info;
        text.textContent = message;
        toast.className = `toast toast-${type} toast-show`;

        if (duration > 0) {
            setTimeout(() => {
                remove(toast);
            }, duration);
        }
    }

    return {
        show,
        success: (msg, duration) => show(msg, 'success', duration),
        error: (msg, duration) => show(msg, 'error', duration),
        warning: (msg, duration) => show(msg, 'warning', duration),
        info: (msg, duration) => show(msg, 'info', duration),
        loading: (msg = 'Loading...') => show(msg, 'info', 0),
        update,
        remove
    };
})();