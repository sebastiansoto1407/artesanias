// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// sije.js - Validaciones front-end personalizadas

document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form.needs-validation");

    forms.forEach(form => {
        form.addEventListener("submit", function (e) {
            let valido = true;
            let mensajes = [];

            form.querySelectorAll("input[type=text], textarea").forEach(input => {
                const valor = input.value.trim();

                if (valor === "") {
                    valido = false;
                    mensajes.push(`El campo "${input.name}" no puede estar vacio.`);
                }

                if (/[^a-zA-Z0-9\sáéíóúÁÉÍÓÚñÑ.,-]/.test(valor)) {
                    valido = false;
                    mensajes.push(`El campo "${input.name}" contiene caracteres invalidos.`);
                }
            });

            form.querySelectorAll("input[type=email]").forEach(input => {
                const valor = input.value.trim();
                const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

                if (!regexEmail.test(valor)) {
                    valido = false;
                    mensajes.push("El email ingresado no es valido.");
                }
            });

            // Validar números
            form.querySelectorAll("input[type=number]").forEach(input => {
                const valor = input.value.trim();

                if (valor === "" || isNaN(valor)) {
                    valido = false;
                    mensajes.push(`El campo "${input.name}" debe ser un numero.`);
                }

                if (Number(valor) < 0) {
                    valido = false;
                    mensajes.push(`El campo "${input.name}" no puede ser negativo.`);
                }
            });

            //si hay errores negarlo
            if (!valido) {
                e.preventDefault();
                alert("Errores detectados:\n\n" + mensajes.join("\n"));
            }
        });
    });
});
