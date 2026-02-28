// candidate-workflow.js — Row selection, complete toggle, confirm delete, add validation, drag reorder
import { showToast } from '/js/toast.js';
import { confirmDelete } from '/js/confirm-modal.js';

const list = document.querySelector('[data-workflow-list]');
const addForm = document.querySelector('[data-add-step-form]');

// ───── ROW CLICK → SELECT (navigate to edit) ─────

if (list) {
    list.addEventListener('click', (e) => {
        const row = e.target.closest('.wf-list-row');
        if (!row) return;

        // Don't navigate if clicking checkbox, delete button, or drag handle
        if (e.target.closest('[data-toggle-complete]') ||
            e.target.closest('[data-delete-step]') ||
            e.target.closest('.wf-col-drag')) return;

        const href = row.dataset.href;
        if (href) window.location.href = href;
    });
}

// ───── COMPLETE CHECKBOX → SUBMIT FORM ─────

if (list) {
    list.addEventListener('change', (e) => {
        const cb = e.target.closest('[data-toggle-complete]');
        if (!cb) return;

        const form = cb.closest('form');
        if (form) form.submit();
    });
}

// ───── DELETE WITH CONFIRM ─────

if (list) {
    list.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-delete-step]');
        if (!btn) return;

        e.stopPropagation();

        const name = btn.dataset.deleteName || 'this step';
        const ok = await confirmDelete(`Delete "${name}"? This cannot be undone.`);
        if (!ok) return;

        const id = btn.dataset.deleteId;
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Candidate/DeleteWorkflowStep';

        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'id';
        input.value = id;

        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
    });
}

// ───── ADD FORM VALIDATION + CUSTOM STEP ─────

if (addForm) {
    const stepSelect = addForm.querySelector('[data-step-select]');
    const customInput = addForm.querySelector('[data-custom-step]');

    // Toggle custom input visibility
    if (stepSelect && customInput) {
        stepSelect.addEventListener('change', () => {
            if (stepSelect.value === '__custom__') {
                customInput.style.display = '';
                customInput.focus();
            } else {
                customInput.style.display = 'none';
                customInput.value = '';
            }
        });
    }

    addForm.addEventListener('submit', (e) => {
        addForm.querySelectorAll('.error').forEach(el => el.classList.remove('error'));

        // If custom, swap value into the select's name field
        if (stepSelect && stepSelect.value === '__custom__') {
            const custom = customInput.value.trim();
            if (!custom) {
                e.preventDefault();
                customInput.classList.add('error');
                showToast('Custom step name is required', 'warning');
                customInput.focus();
                return;
            }
            // Add a hidden option with the custom value so it submits
            const opt = document.createElement('option');
            opt.value = custom;
            opt.selected = true;
            stepSelect.appendChild(opt);
        }

        const errors = [];

        addForm.querySelectorAll('[data-required]').forEach(field => {
            if (!field.value.trim() || field.value === '__custom__') {
                // Skip if custom was already handled above
                if (field === stepSelect && customInput.value.trim()) return;
                errors.push(field.dataset.required + ' is required');
                field.classList.add('error');
            }
        });

        if (errors.length) {
            e.preventDefault();
            showToast(errors[0], 'warning');
            const first = addForm.querySelector('.error');
            if (first) first.focus();
        }
    });

    addForm.addEventListener('change', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });

    addForm.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}

// ───── EDIT FORM — CUSTOM STEP TOGGLE + SUBMIT ─────

const editForm = document.querySelector('[data-workflow-edit] form');

if (editForm) {
    const editSelect = editForm.querySelector('[data-step-select-edit]');
    const editCustom = editForm.querySelector('[data-custom-step-edit]');

    if (editSelect && editCustom) {
        editSelect.addEventListener('change', () => {
            if (editSelect.value === '__custom__') {
                editCustom.style.display = '';
                editCustom.focus();
            } else {
                editCustom.style.display = 'none';
                editCustom.value = '';
            }
        });
    }

    editForm.addEventListener('submit', (e) => {
        if (editSelect && editSelect.value === '__custom__') {
            const custom = editCustom.value.trim();
            if (!custom) {
                e.preventDefault();
                editCustom.classList.add('error');
                showToast('Custom step name is required', 'warning');
                editCustom.focus();
                return;
            }
            const opt = document.createElement('option');
            opt.value = custom;
            opt.selected = true;
            editSelect.appendChild(opt);
        }
    });

    editForm.addEventListener('input', (e) => {
        if (e.target.classList.contains('error')) e.target.classList.remove('error');
    });
}

// ───── DRAG & DROP REORDER ─────

if (list) {
    let dragRow = null;
    let placeholder = null;

    const rows = () => [...list.querySelectorAll('.wf-list-row')];

    // Only start drag from the grip handle
    list.addEventListener('mousedown', (e) => {
        const handle = e.target.closest('.wf-col-drag');
        if (!handle) return;

        dragRow = handle.closest('.wf-list-row');
        if (!dragRow) return;

        e.preventDefault();

        // Create placeholder
        placeholder = document.createElement('div');
        placeholder.className = 'wf-list-row';
        placeholder.style.border = '2px dashed var(--clr-primary-light)';
        placeholder.style.background = 'var(--clr-primary-bg)';
        placeholder.style.height = dragRow.offsetHeight + 'px';

        // Style the dragged row
        dragRow.style.opacity = '0.5';

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });

    function onMouseMove(e) {
        if (!dragRow) return;

        const allRows = rows();
        const mouseY = e.clientY;

        // Find the row we're hovering over
        let target = null;
        for (const row of allRows) {
            if (row === dragRow || row === placeholder) continue;
            const rect = row.getBoundingClientRect();
            const midY = rect.top + rect.height / 2;
            if (mouseY < midY) {
                target = row;
                break;
            }
        }

        // Insert placeholder
        if (placeholder.parentNode) placeholder.remove();

        if (target) {
            list.insertBefore(placeholder, target);
        } else {
            // Append after last row
            list.appendChild(placeholder);
        }
    }

    function onMouseUp() {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        if (!dragRow) return;

        // Move the actual row to placeholder position
        if (placeholder.parentNode) {
            list.insertBefore(dragRow, placeholder);
            placeholder.remove();
        }

        dragRow.style.opacity = '';
        placeholder = null;

        // Collect new order and POST to server
        const items = rows().map((row, i) => ({
            id: parseInt(row.dataset.wfId),
            stepOrder: i + 1
        }));

        fetch('/Candidate/ReorderWorkflowSteps', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(items)
        }).then(res => {
            if (res.ok) {
                showToast('Order updated', 'success');
            } else {
                showToast('Failed to save order', 'danger');
            }
        }).catch(() => {
            showToast('Failed to save order', 'danger');
        });

        dragRow = null;
    }
}