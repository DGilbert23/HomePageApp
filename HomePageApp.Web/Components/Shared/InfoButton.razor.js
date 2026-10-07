// JavaScript for InfoButton component
const handlers = new WeakMap();

export function initialize(container, dotNetReference) {
    const handler = (event) => {
        if (!container.contains(event.target)) {
            dotNetReference.invokeMethodAsync("CloseInfo");
        }
    };

    handlers.set(container, handler);

    document.addEventListener("click", handler);
}

export function dispose(container) {
    const handler = handlers.get(container);

    if (handler) {
        document.removeEventListener("click", handler);
        handlers.delete(container);
    }
}