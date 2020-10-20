const inputs = document.querySelectorAll('.input')

function focusFunc() {
    this.parentNode.parentNode.classList.add('focus')
}

function blurFunc() {
    if (this.value == "") {
        this.parentNode.parentNode.classList.remove('focus')
    }
}
inputs.forEach(i => {
    i.addEventListener('focus', focusFunc)
    i.addEventListener('blur', blurFunc)
})