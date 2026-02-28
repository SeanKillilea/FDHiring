// candidate-files.js — Drop + paste upload, confirm delete
import { showToast } from './toast.js';
import { confirmDelete } from '/js/confirm-modal.js';

const layout = document.querySelector('[data-files-layout]');
if (layout) init();

function init() {
    const candidateId = layout.dataset.candidateId;
    if (!candidateId) return;

    const dropzone = layout.querySelector('[data-dropzone]');

    // ───── DRAG & DROP ─────

    dropzone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropzone.classList.add('dragover');
    });

    dropzone.addEventListener('dragleave', () => {
        dropzone.classList.remove('dragover');
    });

    dropzone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropzone.classList.remove('dragover');
        if (e.dataTransfer.files.length) upload(e.dataTransfer.files);
    });

    // ───── PASTE (works when dropzone is focused or anywhere on page) ─────

    document.addEventListener('paste', (e) => {
        const files = [];
        if (e.clipboardData && e.clipboardData.files.length) {
            for (const f of e.clipboardData.files) {
                files.push(f);
            }
        }
        if (files.length) {
            e.preventDefault();
            upload(files);
        }
    });

    // ───── UPLOAD ─────

    async function upload(files) {
        let uploaded = 0;

        for (const file of files) {
            const fd = new FormData();
            fd.append('candidateId', candidateId);
            fd.append('file', file);

            try {
                const res = await fetch('/Candidate/UploadFile', {
                    method: 'POST',
                    body: fd
                });

                if (res.ok) {
                    uploaded++;
                } else {
                    showToast(`Failed to upload ${file.name}`, 'danger');
                }
            } catch {
                showToast(`Failed to upload ${file.name}`, 'danger');
            }
        }

        if (uploaded > 0) {
            showToast(`${uploaded} file${uploaded > 1 ? 's' : ''} uploaded`, 'success');
            window.location.href = '/Candidate/Files';
        }
    }

    // ───── DELETE WITH CONFIRM MODAL ─────

    const deleteBtn = layout.querySelector('[data-delete-file]');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', async () => {
            const ok = await confirmDelete('Delete this file permanently?');
            if (!ok) return;

            const id = deleteBtn.dataset.deleteId;
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = deleteBtn.dataset.deleteAction;

            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'id';
            input.value = id;

            form.appendChild(input);
            document.body.appendChild(form);
            form.submit();
        });
    }
}