/* =============================================
   Header search — global quick search
   Enter on search page: updates search input and triggers search
   Enter on other pages: navigates to search page with query
   ============================================= */

const headerInput = document.querySelector('[data-global-search-input]');

if (headerInput) {
    headerInput.addEventListener('keydown', (e) => {
        if (e.key !== 'Enter') return;
        e.preventDefault();

        const query = headerInput.value.trim();
        const searchPageInput = document.querySelector('[data-search-input]');

        if (searchPageInput) {
            searchPageInput.value = query;
            searchPageInput.dispatchEvent(new Event('input', { bubbles: true }));
            headerInput.value = '';
            headerInput.blur();
        } else {
            window.location.href = query
                ? `/Candidate/Search?q=${encodeURIComponent(query)}`
                : '/Candidate/Search';
        }
    });
}