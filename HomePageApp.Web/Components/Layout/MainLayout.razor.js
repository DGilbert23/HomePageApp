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

export function initializeTheme() {
    const cookies = document.cookie.split(";");

    for (const cookie of cookies) {
        const [name, value] = cookie.trim().split("=");

        if (name === "IsDarkMode") {
            const isDarkMode = value === "true";

            document.documentElement.setAttribute(
                "data-bs-theme",
                isDarkMode ? "dark" : "light"
            );

            console.log(
                "Theme restored from cookie:",
                isDarkMode ? "dark" : "light"
            );

            return;
        }
    }

    // No cookie exists, so leave the existing theme alone.
    console.log("No IsDarkMode cookie found.");
}