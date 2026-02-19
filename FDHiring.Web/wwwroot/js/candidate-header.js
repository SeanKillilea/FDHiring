if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initHeader);
} else {
    initHeader();
}

function initHeader() {
    const header = document.querySelector('[data-candidate-header]');
    if (!header) return;

    const candidateId = header.dataset.candidateId;
    if (!candidateId || candidateId === '0') return;

    header.classList.remove('hidden');
    loadCandidateHeader(candidateId, header);
}

async function loadCandidateHeader(id, header) {
    try {
        const res = await fetch(`/Candidate/GetCandidate?id=${id}`);
        if (!res.ok) return;

        const c = await res.json();

        const img = header.querySelector('[data-candidate-image]');
        const name = header.querySelector('[data-candidate-name]');
        const title = header.querySelector('[data-candidate-title]');
        const wouldHire = header.querySelector('[data-pill-would-hire]');
        const current = header.querySelector('[data-pill-current]');
        const active = header.querySelector('[data-pill-active]');

        if (img) img.src = c.imagePath || '/images/default-avatar.png';
        if (name) name.textContent = `${c.firstName} ${c.lastName}`;
        if (title) title.textContent = c.positionName || '';

        if (wouldHire) wouldHire.style.display = c.wouldHire ? '' : 'none';
        if (current) current.style.display = c.isCurrent ? '' : 'none';
        if (active) active.style.display = c.active ? '' : 'none';
    } catch (err) {
        console.error('Failed to load candidate header:', err);
    }
}