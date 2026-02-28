/* =============================================
   Toast helper
   Usage:
     showToast('Candidate saved', 'success');
     showToast('Something went wrong', 'danger');
     showToast('Check your input', 'warning');
     showToast('Interview scheduled', 'info');
   ============================================= */
const ICON_MAP = {
    success: '#icon-success',
    danger: '#icon-error',
    warning: '#icon-warning',
    info: '#icon-info',
};
const DURATION = 4000; // auto-dismiss ms
function getContainer() {
    let c = document.querySelector('.toast-container');
    if (!c) {
        c = document.createElement('div');
        c.className = 'toast-container';
        document.body.appendChild(c);
    }
    return c;
}
function removeToast(el) {
    el.classList.add('toast-out');
    el.addEventListener('animationend', () => el.remove(), { once: true });
    // Fallback in case animationend doesn't fire
    setTimeout(() => { if (el.parentNode) el.remove(); }, 500);
}
export function showToast(message, variant = 'info') {
    const container = getContainer();
    const icon = ICON_MAP[variant] || ICON_MAP.info;
    const toast = document.createElement('div');
    toast.className = `toast toast-${variant}`;
    toast.innerHTML = `
      <svg><use href="${icon}"></use></svg>
      <span class="toast-message">${message}</span>
      <button class="toast-close" title="Dismiss">
         <svg><use href="#icon-close"></use></svg>
      </button>
   `;
    toast.querySelector('.toast-close').addEventListener('click', () => removeToast(toast));
    container.appendChild(toast);
    // Auto-dismiss
    setTimeout(() => {
        if (toast.parentNode) removeToast(toast);
    }, DURATION);
}
// Make available globally for inline onclick handlers
window.showToast = showToast;