'use strict';
/* =============================================
   site.js — Common site functionality
   ============================================= */
import { showToast } from './toast.js';
import './user-modal.js';
import './candidate-search.js';
import './header-search.js';

// ===== GLOBAL ACCESS =====
window.showToast = showToast;

// ===== DEBOUNCE UTILITY =====
export function debounce(fn, ms = 300) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => fn(...args), ms);
    };
}

// ===== ACTIVE NAV HIGHLIGHTING =====
function setActiveNav() {
    const path = window.location.pathname.toLowerCase();
    // Sidebar — match by href
    document.querySelectorAll('.sidebar-btn').forEach(btn => {
        const href = btn.getAttribute('href')?.toLowerCase();
        if (href && href !== '/' && path.startsWith(href)) {
            btn.classList.add('active');
        }
    });
    // Header nav — driven by data attribute or ViewData, skip for now
}

// ===== CONFIRM DELETE =====
function initConfirmDialogs() {
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', (e) => {
            if (!confirm(el.dataset.confirm)) {
                e.preventDefault();
            }
        });
    });
}

// ===== FORM DIRTY CHECK =====
function initDirtyForms() {
    document.querySelectorAll('form[data-dirty-check]').forEach(form => {
        let dirty = false;
        form.addEventListener('input', () => { dirty = true; });
        form.addEventListener('submit', () => { dirty = false; });
        window.addEventListener('beforeunload', (e) => {
            if (dirty) {
                e.preventDefault();
                e.returnValue = '';
            }
        });
    });
}

// ===== TOOLTIP DISMISS ON SCROLL =====
function initScrollDismiss() {
    let ticking = false;
    window.addEventListener('scroll', () => {
        if (!ticking) {
            requestAnimationFrame(() => {
                document.activeElement?.blur();
                ticking = false;
            });
            ticking = true;
        }
    });
}

// ===== INIT =====
document.addEventListener('DOMContentLoaded', () => {
    setActiveNav();
    initConfirmDialogs();
    initDirtyForms();
    initScrollDismiss();
});