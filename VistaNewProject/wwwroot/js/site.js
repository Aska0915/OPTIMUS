﻿// Definir la función opcionesDePerfil
function opcionesDePerfil() {
    var profilePopup = document.getElementById("perfilVentanaEmergente");
    if (profilePopup.style.display === "none") {
        profilePopup.style.display = "block";
    } else {
        profilePopup.style.display = "none";
    }
}
function confirmLogout() {
    var result = confirm("¿Estás seguro de que deseas cerrar sesión?");
    if (result) {
        window.location.href = "/Login/Logout"; // Redirige al usuario a la URL definida
    } else {
        window.location.reload();
    }
}
$(function () {
   $("#menu-toggle").on("click", function (e) {
    e.preventDefault();
    closeSubMenus(); // Cierra todos los submenús al hacer clic en el menú principal
    $("#Menu").toggleClass("menu-hidden");
    $(".container").toggleClass("menu-hidden");
    });
    // Manejar el clic en las opciones del menú
    $(".sidebar-label").on("click", function (e) {
        var optionId = $(this).attr("for");
        showSubMenu(optionId);
    });

    // Cierra los submenús al hacer clic en cualquier parte fuera del menú
    $(document).on("click", function (event) {
        var target = event.target;
        if (!$(target).is("#menu-toggle") && !$(target).closest("#Menu").length) {
            closeSubMenus();
        }
    });

    // Cierra todos los submenús
    function closeSubMenus() {
        var subMenus = document.querySelectorAll('.sub-menu');
        subMenus.forEach(function (subMenu) {
            subMenu.style.maxHeight = '0';
        });
    }

    // Función para mostrar y ocultar los submenús
    function showSubMenu(optionId) {
        var subMenu = $("#" + optionId).next(".sub-menu");

        // Cierra el submenú si ya está abierto
        if (subMenu.css("max-height") !== '0px') {
            subMenu.css("max-height", "0");
        } else {
            // Oculta todos los submenús
            closeSubMenus();

            // Muestra el submenú correspondiente al clicar en la opción
            subMenu.css("max-height", subMenu.prop("scrollHeight") + 'px');
        }
    }
    
});
/**/
function setHoraActual(campo) {
    // Crear un nuevo objeto Date que representa la fecha y hora actual
    var fechaHoraActual = new Date();

    // Formatear la fecha y hora en el formato necesario ('YYYY-MM-DDTHH:mm')
    var anio = fechaHoraActual.getFullYear();
    var mes = String(fechaHoraActual.getMonth() + 1).padStart(2, '0');
    var dia = String(fechaHoraActual.getDate()).padStart(2, '0');
    var hora = String(fechaHoraActual.getHours()).padStart(2, '0');
    var minutos = String(fechaHoraActual.getMinutes()).padStart(2, '0');

    var fechaHoraFormateada = `${anio}-${mes}-${dia}T${hora}:${minutos}`;
    var x = campo; 
    // Asignar la fecha y hora formateada al campo datetime-local
    document.getElementById(x).value = fechaHoraFormateada;
}



