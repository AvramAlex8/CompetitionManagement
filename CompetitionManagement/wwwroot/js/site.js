// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const navbarItems = document.querySelectorAll('.navbar-nav .nav-item');

navbarItems.forEach(item => {
    item.addEventListener('click', () => {
        // Elimină clasa selectată de la toate elementele
        navbarItems.forEach(item => {
            item.classList.remove('selected');
        });

        // Adaugă clasa selectată elementului curent
        item.classList.add('selected');
    });
});