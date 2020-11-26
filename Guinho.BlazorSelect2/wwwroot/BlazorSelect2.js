let instances = {};

document.addEventListener('click', function (event) {

    let isClickInside = true;

    let elements = document.getElementsByClassName('blazor-select2-container opened');

    let arr = Array.prototype.slice.call(elements)

    isClickInside = arr.some((el) => el.contains(event.target));

    if (!isClickInside) {
        var keys = Object.keys(instances);

        for (let i = 0; i < keys.length; i++) {
            let inst = instances[keys[i]];

            if (inst) {
                inst.invokeMethodAsync('Close', '');
            }
        }

    }
});


export function setSelect2Instance(inst, id) {
    instances[id] = inst;
}
export function removeSelect2Instance(id) {
    instances[id] = null;
}
export function setContentSize(id) {
    let el = document.getElementById(id);

    if (!el) return;

    el.style.width = `${(document.getElementById(id).parentElement.clientWidth - 22)}px`;
}
export function keyboardsEvent(id) {
    keydownRef = keydown.bind(this, id);
    window.addEventListener("keydown", keydownRef);
}
export function setFocus(id) {
    document.getElementById(id).focus();
}
export function removeEventListener() {
    window.removeEventListener("keydown", keydownRef);
}


let keydownRef = null;

let keydown = function (id, event) {


    if (event.keyCode == 40) { // down
        let selected = document.getElementById(id).querySelector("li.blazor-select2-content-option-item.selected");

        if (selected && selected.nextElementSibling) {
            selected.classList.remove("selected");
            selected.nextElementSibling.classList.add("selected");

            document.getElementById(id).scrollTop = selected.offsetTop - 35;
        } else if (selected) {
            document.getElementById(id).scrollTop = selected.offsetTop;
        }
    }

    if (event.keyCode == 38) { // up
        let selected = document.getElementById(id).querySelector("li.blazor-select2-content-option-item.selected");

        if (selected && selected.previousElementSibling) {
            selected.classList.remove("selected");
            selected.previousElementSibling.classList.add("selected");

            document.getElementById(id).scrollTop = selected.offsetTop - 85;
        }
    }

    if (event.keyCode == 13) { // enter
        let el = document.getElementById(id);
        let selected = el.querySelector("li.blazor-select2-content-option-item.selected");

        if (selected) {

            instances[id].invokeMethodAsync('SelectValue', selected.dataset.value);
        }
    }
};
