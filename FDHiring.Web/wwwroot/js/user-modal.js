/* =============================================
   User selection modal
   - Auto-opens when no user is in session
   - Opens on header badge click to switch users
   - Fetches user list from /User/List
   - Sets session via POST /User/SetUser
   ============================================= */

const COLORS = [
    'var(--clr-primary)',
    'var(--clr-success)',
    'hsl(25, 80%, 45%)',
    'hsl(270, 55%, 50%)',
    'hsl(0, 65%, 48%)',
    'hsl(195, 60%, 42%)',
    'hsl(330, 55%, 48%)',
];

const overlay = document.querySelector('[data-modal="select-user"]');
const userList = document.querySelector('[data-user-list]');
const badge = document.querySelector('[data-user-badge]');

let usersLoaded = false;

function initials(first, last) {
    return `${first[0]}${last[0]}`.toUpperCase();
}

function openModal() {
    if (!usersLoaded) loadUsers();
    overlay.classList.add('open');
}

function closeModal() {
    overlay.classList.remove('open');
}

async function loadUsers() {
    try {
        const res = await fetch('/User/List');
        const users = await res.json();
        userList.innerHTML = '';

        users.forEach((u, i) => {
            const color = COLORS[i % COLORS.length];
            const item = document.createElement('div');
            item.className = 'user-select-item';
            item.innerHTML = `
                <div class="user-select-avatar" style="background:${color}">
                    ${initials(u.firstName, u.lastName)}
                </div>
                <div class="user-select-name">${u.firstName} ${u.lastName}</div>
            `;
            item.addEventListener('click', () => selectUser(u.id));
            userList.appendChild(item);
        });

        usersLoaded = true;
    } catch (err) {
        console.error('Failed to load users:', err);
    }
}

async function selectUser(id) {
    try {
        const res = await fetch('/User/SetUser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `id=${id}`,
        });

        if (!res.ok) throw new Error('SetUser failed');

        const u = await res.json();

        // Update header badge
        badge.textContent = initials(u.firstName, u.lastName);
        badge.title = `${u.firstName} ${u.lastName}`;
        document.body.dataset.userId = u.id;

        closeModal();
        window.location.reload();
    } catch (err) {
        console.error('Failed to set user:', err);
    }
}

// Header badge click → open modal
badge?.addEventListener('click', openModal);

// Auto-open if no user in session
if (!parseInt(document.body.dataset.userId, 10)) {
    openModal();
}