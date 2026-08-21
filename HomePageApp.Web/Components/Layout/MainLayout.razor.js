export function toggleTheme() {
    console.log("toggleTheme JavaScript function called!");

    const html = document.documentElement;
    const current = html.getAttribute("data-bs-theme");

    console.log("Current theme:", current);

    const newTheme = current === "dark" ? "light" : "dark";

    html.setAttribute("data-bs-theme", newTheme);

    // Save the user's preference in a cookie
    document.cookie = `IsDarkMode=${newTheme === "dark"}; path=/; max-age=31536000; SameSite=Lax`;

    console.log(
        "New theme:",
        html.getAttribute("data-bs-theme")
    );

    console.log(
        "IsDarkMode cookie:",
        newTheme === "dark"
    );
}