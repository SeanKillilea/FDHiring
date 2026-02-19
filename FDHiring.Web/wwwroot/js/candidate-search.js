if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}

function init() {
    const userLogin = document.querySelector('[data-user-login]');
    const searchName = document.querySelector('[data-search-name]');
    const searchPosition = document.querySelector('[data-search-position]');
    const searchCurrent = document.querySelector('[data-search-current]');
    const searchActive = document.querySelector('[data-search-active]');
    const clearBtn = document.querySelector('[data-search-clear]');
    const resultsContainer = document.querySelector('[data-candidate-results]');
    const showingCount = document.querySelector('[data-search-showing]');
    const totalCount = document.querySelector('[data-search-total]');

    let debounceTimer;

    // User login change
    userLogin?.addEventListener('change', async (e) => {
        await fetch('/Candidate/SetUser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `userId=${e.target.value}`
        });
    });

    // Search on input/change with debounce for text
    searchName?.addEventListener('input', () => {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(search, 300);
    });

    searchPosition?.addEventListener('change', search);
    searchCurrent?.addEventListener('change', search);
    searchActive?.addEventListener('change', search);

    // Clear filters
    clearBtn?.addEventListener('click', () => {
        searchName.value = '';
        searchPosition.value = '';
        searchCurrent.checked = false;
        searchActive.checked = false;
        search();
    });

    const selectedCandidateId = resultsContainer?.dataset.selectedCandidate;

    // Initial search on page load
    search();

    async function search() {
        const params = new URLSearchParams();

        if (searchName?.value) params.append('name', searchName.value);
        if (searchPosition?.value) params.append('positionId', searchPosition.value);
        if (searchCurrent?.checked) params.append('current', 'true');
        if (searchActive?.checked) params.append('active', 'true');

        // Save search state to session
        saveSearchState();

        try {
            const res = await fetch(`/Candidate/SearchCandidates?${params}`);
            const data = await res.json();

            showingCount.textContent = data.showing;
            totalCount.textContent = data.total;

            renderCandidates(data.candidates);
        } catch (err) {
            console.error('Search failed:', err);
        }
    }

    function renderCandidates(candidates) {
        if (!candidates || candidates.length === 0) {
            resultsContainer.innerHTML = '<p class="text-muted">No candidates found.</p>';
            return;
        }

        resultsContainer.innerHTML = candidates.map(c => `
            <div class="card" data-candidate-id="${c.id}">
                <div class="image">
                    <img class="search-bg" src="${c.imagePath || '/images/default-avatar.png'}" alt="${c.firstName} ${c.lastName}" />
                </div>
                <div class="section">
                    <div class="candidate-name">${c.firstName} ${c.lastName}</div>
                    <div class="candidate-position">${c.positionName || ''}</div>
                </div>
                <div class="section">
                    <div>${c.phone || ''}</div>
                    <div>${c.email || ''}</div>
                </div>
                <div class="section primary">
                    <div>${c.notes || ''}</div>
                </div>
                <div class="section">
                    ${c.wouldHire ? '<span class="pill medium would-hire">Would Hire</span>' : ''}
                    ${c.isCurrent ? '<span class="pill medium current">Current</span>' : ''}
                    ${c.active ? '<span class="pill medium active">Active</span>' : ''}
                </div>
            </div>
        `).join('');

        // Click card to select candidate
        resultsContainer.querySelectorAll('.card').forEach(card => {
            card.addEventListener('click', () => {
                resultsContainer.querySelectorAll('.card').forEach(c => c.classList.remove('active'));
                card.classList.add('active');
                selectCandidate(card.dataset.candidateId);
            });
        });

        // Highlight selected candidate from session
        if (selectedCandidateId && selectedCandidateId !== '0') {
            const selected = resultsContainer.querySelector(`[data-candidate-id="${selectedCandidateId}"]`);
            if (selected) {
                selected.classList.add('active');
                selected.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    }

    async function selectCandidate(id) {
        await fetch('/Candidate/SetCandidate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `candidateId=${id}`
        });

        // Update the candidate header
        const header = document.querySelector('[data-candidate-header]');
        if (header) {
            header.classList.remove('hidden');
            header.dataset.candidateId = id;

            // Update edit link
            const editBtn = header.querySelector('[data-btn-edit]');
            if (editBtn) editBtn.href = `/Candidate/Edit/${id}`;

            loadCandidateHeader(id, header);
        }

        // Enable sidebar buttons
        enableSidebar(id);
    }

    function enableSidebar(id) {
        const sidebar = document.querySelector('.side-action-buttons');
        if (!sidebar) return;

        // Replace disabled spans with active links
        const disabledButtons = sidebar.querySelectorAll('span.btn.action.disabled');
        if (disabledButtons.length === 0) return;

        const buttonConfig = [
            { title: 'Edit Candidate', href: `/Candidate/Edit/${id}` },
            { title: 'Workflow', href: `/Candidate/Workflow/${id}` },
            { title: 'Interviews', href: `/Candidate/Interview/${id}` },
            { title: 'Files', href: `/Candidate/Files/${id}` },
            { title: 'Communicate', href: `/Candidate/Communicate/${id}` }
        ];

        disabledButtons.forEach((span, index) => {
            const config = buttonConfig[index];
            if (!config) return;

            const link = document.createElement('a');
            link.href = config.href;
            link.className = 'btn action';
            link.title = config.title;
            link.innerHTML = span.innerHTML;
            span.replaceWith(link);
        });
    }

    async function saveSearchState() {
        const params = new URLSearchParams();
        params.append('name', searchName?.value || '');
        params.append('positionId', searchPosition?.value || '0');
        params.append('current', searchCurrent?.checked ? 'true' : 'false');
        params.append('active', searchActive?.checked ? 'true' : 'false');

        await fetch('/Candidate/SaveSearchState', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: params.toString()
        });
    }
}