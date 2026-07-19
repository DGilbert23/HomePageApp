export function initializeValidation() {
    console.log("initializeValidation called");

    const forms = document.querySelectorAll(".needs-validation");

    console.log("Found forms:", forms.length);

    forms.forEach(form => {

        form.addEventListener("submit", event => {

            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add("was-validated");
        });

    });
}