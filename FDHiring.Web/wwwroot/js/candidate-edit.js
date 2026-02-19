if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}

function init() {
    const form = document.querySelector('[data-candidate-form]');
    if (!form) return;

    const candidateId = form.dataset.candidateId;
    const deleteBtn = form.querySelector('[data-btn-delete]');
    const cancelBtn = form.querySelector('[data-btn-cancel]');

    let currentCandidate = null;

    // Load candidate data
    loadCandidate(candidateId);

    // Save
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await saveCandidate(candidateId);
    });

    // Cancel
    cancelBtn?.addEventListener('click', () => {
        window.location.href = '/Candidate/Search';
    });

    // Delete
    deleteBtn?.addEventListener('click', async () => {
        const yes = await Confirm.show('Are you sure you want to delete this candidate? This cannot be undone.', {
            title: 'Delete Candidate',
            confirmText: 'Delete',
            cancelText: 'Cancel'
        });

        if (!yes) return;

        try {
            const res = await fetch('/Candidate/DeleteCandidate', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `id=${candidateId}`
            });

            if (res.ok) {
                Toast.success('Candidate deleted.');
                setTimeout(() => {
                    window.location.href = '/Candidate/Search';
                }, 1000);
            }
        } catch (err) {
            console.error('Delete failed:', err);
            Toast.error('Failed to delete candidate.');
        }
    });
}

async function loadCandidate(id) {
    try {
        const res = await fetch(`/Candidate/GetCandidate?id=${id}`);
        if (!res.ok) return;

        const c = await res.json();

        currentCandidate = c;

        document.getElementById('firstName').value = c.firstName || '';
        document.getElementById('lastName').value = c.lastName || '';
        document.getElementById('email').value = c.email || '';
        document.getElementById('phone').value = c.phone || '';
        document.getElementById('linkedin').value = c.linkedIn || '';
        document.getElementById('position').value = c.positionId || '';
        document.getElementById('agency').value = c.agencyId || '';
        document.getElementById('dateFound').value = c.dateFound ? c.dateFound.split('T')[0] : '';
        document.getElementById('notes').value = c.notes || '';
        document.getElementById('wouldHire').checked = c.wouldHire || false;
        document.getElementById('isCurrent').checked = c.isCurrent || false;
        document.getElementById('active').checked = c.active || false;
    } catch (err) {
        console.error('Failed to load candidate:', err);
    }
}

async function saveCandidate(id) {
    const candidate = {
        id: parseInt(id),
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value,
        linkedIn: document.getElementById('linkedin').value,
        imagePath: currentCandidate?.imagePath || null,
        positionId: parseInt(document.getElementById('position').value) || 0,
        agencyId: parseInt(document.getElementById('agency').value) || 0,
        dateFound: document.getElementById('dateFound').value,
        notes: document.getElementById('notes').value,
        wouldHire: document.getElementById('wouldHire').checked,
        isCurrent: document.getElementById('isCurrent').checked,
        active: document.getElementById('active').checked
    };

    try {
        const res = await fetch('/Candidate/UpdateCandidate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(candidate)
        });

        if (res.ok) {
            // Update the candidate header
            const header = document.querySelector('[data-candidate-header]');
            if (header) {
                loadCandidateHeader(id, header);
            }
            Toast.success('Candidate saved successfully.');
        } else {
            Toast.error('Failed to save candidate.');
        }
    } catch (err) {
        console.error('Save failed:', err);
        Toast.error('Failed to save candidate.');
    }
}