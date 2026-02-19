document.addEventListener('DOMContentLoaded', () => {
    const userLogin = document.querySelector('[data-user-login]');

    if (userLogin) {
        userLogin.addEventListener('change', async (e) => {
            await fetch('/Candidate/SetUser', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `userId=${e.target.value}`
            });
        });
    }
});