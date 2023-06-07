const stars = [...document.querySelectorAll(".star")]
const starsRadio = [...document.querySelectorAll(".starRadio")]
starsRadio.forEach(radio => radio.style.display = "none")

stars.forEach(star => {
    star.addEventListener("click", () => {
        i = stars.indexOf(star);
        starsRadio[i].checked = true;
        if (star.classList.contains('fa-regular')) {
            for (i; i >= 0; --i) {
                stars[i].classList.replace('fa-regular', 'fa-solid');
                stars[i].classList.replace('text-muted', 'text-warning');
            }
        } else {
            for (i++; i < stars.length; ++i) {
                stars[i].classList.replace('fa-solid', 'fa-regular');
                stars[i].classList.replace('text-warning', 'text-muted');
            }
        }
    });
});