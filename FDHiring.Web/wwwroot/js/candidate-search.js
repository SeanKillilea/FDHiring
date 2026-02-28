/* =============================================
   Candidate search — as-you-type with debounce
   Templates cloned from <template> elements
   ============================================= */
import { debounce } from './site.js';

// DOM refs
const searchInput = document.querySelector('[data-search-input]');
const activeSelect = document.querySelector('[data-search-active]');
const currentCheck = document.querySelector('[data-search-current]');
const wouldHireCheck = document.querySelector('[data-search-wouldhire]');
const resultCount = document.querySelector('[data-search-results]');
const candidateList = document.querySelector('[data-candidate-list]');

// Template cache
const tplCompact = document.querySelector('[data-tpl="compact-card"]');
const tplActive = document.querySelector('[data-tpl="active-card"]');
const tplEmpty = document.querySelector('[data-tpl="empty-card"]');

if (searchInput) init();

function init() {
    const run = debounce(search, 300);
    searchInput.addEventListener('input', run);
    activeSelect.addEventListener('change', run);
    currentCheck.addEventListener('change', run);
    wouldHireCheck.addEventListener('change', run);
    search();
}

// ===== SEARCH =====

async function search() {
    const params = new URLSearchParams();
    const name = searchInput.value.trim();
    if (name) params.set('name', name);
    if (activeSelect.value !== '') params.set('active', activeSelect.value);
    if (currentCheck.checked) params.set('current', 'true');
    if (wouldHireCheck.checked) params.set('wouldHire', 'true');

    try {
        const res = await fetch(`/Candidate/SearchJson?${params}`);
        const data = await res.json();
        resultCount.textContent = `Results ${data.results.length} of ${data.total}`;
        renderList(data.results);
    } catch (err) {
        console.error('Search failed:', err);
    }
}

// ===== RENDER LIST =====

function renderList(candidates) {
    candidateList.innerHTML = '';
    const activeCard = document.querySelector('[data-active-candidate]');
    const selectedId = parseInt(activeCard?.dataset.candidateId || '0', 10);

    candidates.forEach(c => {
        const card = tplCompact.content.cloneNode(true).firstElementChild;

        card.dataset.candidateId = c.id;
        if (c.id === selectedId) card.classList.add('selected');

        fillCard(card, c);
        fillBadges(card, c);

        const notes = card.querySelector('[data-field="notes"]');
        if (notes) notes.textContent = c.notes || '';

        card.addEventListener('click', () => selectCandidate(c));
        candidateList.appendChild(card);
    });
}

// ===== SELECT CANDIDATE =====

async function selectCandidate(c) {
    try {
        const res = await fetch(`/Candidate/SetCandidate?id=${c.id}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!res.ok) throw new Error('SetCandidate failed');

        renderActiveCard(c);

        candidateList.querySelectorAll('.candidate-card-compact').forEach(el => {
            el.classList.toggle('selected', parseInt(el.dataset.candidateId, 10) === c.id);
        });
    } catch (err) {
        console.error('Failed to select candidate:', err);
    }
}

// ===== RENDER ACTIVE CARD =====

function renderActiveCard(c) {
    const current = document.querySelector('[data-active-candidate]');
    const card = tplActive.content.cloneNode(true).firstElementChild;

    card.dataset.candidateId = c.id;
    fillCard(card, c);
    fillBadges(card, c);

    const editLink = card.querySelector('[data-field="edit-link"]');
    if (editLink) editLink.href = `/Candidate/Edit/${c.id}`;

    current.replaceWith(card);
}

function renderEmptyCard() {
    const current = document.querySelector('[data-active-candidate]');
    const card = tplEmpty.content.cloneNode(true).firstElementChild;
    current.replaceWith(card);
}

// ===== SHARED HELPERS =====

function fillCard(card, c) {
    const name = card.querySelector('[data-field="name"]');
    const position = card.querySelector('[data-field="position"]');
    const email = card.querySelector('[data-field="email"]');
    const phone = card.querySelector('[data-field="phone"]');
    const linkedin = card.querySelector('[data-field="linkedin"]');
    const photo = card.querySelector('[data-field="photo"]');

    if (name) name.textContent = `${c.firstName} ${c.lastName}`;
    if (position) position.textContent = c.positionName || '';
    if (email) email.textContent = c.email || '';
    if (phone) phone.textContent = c.phone || '';
    if (linkedin) linkedin.textContent = c.linkedIn || '';

    if (photo) {
        if (c.photoPath) {
            photo.innerHTML = `<img src="${c.photoPath}" alt="${c.firstName} ${c.lastName}" />`;
        } else {
            photo.innerHTML = `
                <div style="width:100%;height:100%;display:grid;place-items:center;background:var(--clr-surface-3)">
                    <svg class="icon icon-xl" style="color:var(--text-tertiary)"><use href="#icon-user"></use></svg>
                </div>`;
        }
    }
}

function fillBadges(card, c) {
    const badgeActive = card.querySelector('[data-field="badge-active"]');
    const badgeWould = card.querySelector('[data-field="badge-wouldhire"]');
    const badgeCurrent = card.querySelector('[data-field="badge-current"]');

    setBadge(badgeActive, c.active, 'badge-success', 'Active');
    setBadge(badgeWould, c.wouldHire, 'badge-info', 'Would Hire');
    setBadge(badgeCurrent, c.isCurrent, 'badge-accent', 'Current');
}

function setBadge(el, value, cls, label) {
    if (!el) return;
    if (value) {
        el.className = `badge ${cls} badge-dot`;
        el.textContent = label;
    } else {
        el.className = 'badge badge-empty';
        el.innerHTML = '&nbsp;';
    }
}