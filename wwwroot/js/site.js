// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var registerBtn = document.querySelector(".register-btn");
var registerForm = document.querySelector(".register-event-form");

registerBtn.addEventListener('click', function () {
    registerForm.submit();
})