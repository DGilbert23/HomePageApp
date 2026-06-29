export function autoResizeTextArea(element) {
    if (!element) return;

    // 1. Reset height to auto so it can shrink if text was deleted
    element.style.height = 'auto';

    // 2. Set the height to match the scroll height of the internal text
    element.style.height = element.scrollHeight + 'px';
}