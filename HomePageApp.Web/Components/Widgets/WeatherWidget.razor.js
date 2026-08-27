export function initHorizontalMouseWheel(elementId) {
    console.log("initHorizontalMouseWheel called with:", elementId);

    const scrollContainer = document.getElementById(elementId);

    if (scrollContainer) {

        scrollContainer.addEventListener("wheel", (evt) => {
            console.log("Wheel event:", evt.deltaY);

            evt.preventDefault();

            scrollContainer.scrollLeft -= evt.deltaY;
        });
    }
}

export function getCurrentPosition() {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            console.log("Geolocation is not supported by this browser.");
            reject("Geolocation is not supported by this browser.");
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (position) => {
                const latitude = position.coords.latitude;
                const longitude = position.coords.longitude;

                console.log("Latitude:", latitude);
                console.log("Longitude:", longitude);

                resolve({
                    latitude: latitude,
                    longitude: longitude
                });
            },
            (error) => {
                console.log("Geolocation error:", error.message);
                reject(error.message);
            }
        );
    });
}