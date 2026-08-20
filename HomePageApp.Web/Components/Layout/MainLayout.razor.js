// JavaScript for MainLayout component
export function toggleTheme() {
    console.log("toggleTheme JavaScript function called!");

    const html = document.documentElement;
    const current = html.getAttribute("data-bs-theme");

    console.log("Current theme:", current);

    html.setAttribute(
        "data-bs-theme",
        current === "dark" ? "light" : "dark"
    );

    console.log(
        "New theme:",
        html.getAttribute("data-bs-theme")
    );
}