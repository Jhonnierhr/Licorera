// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Configuración de Toastr
if (typeof toastr !== 'undefined') {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}

// Función global para mostrar notificaciones
window.showNotification = function(type, message, title) {
    if (typeof toastr !== 'undefined') {
        toastr[type](message, title);
    } else {
        alert(message);
    }
};

// Función para confirmar acciones destructivas
window.confirmAction = function(message, callback) {
    if (confirm(message)) {
        if (typeof callback === 'function') {
            callback();
        }
        return true;
    }
    return false;
};

// Función para formatear números como moneda
window.formatCurrency = function(amount) {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0
    }).format(amount);
};

// Función para actualizar contador del carrito
window.updateCartCounter = function() {
    // Implementar lógica para actualizar contador del carrito en navbar
    // Esta función se puede llamar cuando se agreguen/eliminen productos
    console.log('Actualizando contador del carrito...');
};

// Manejar errores AJAX globalmente
$(document).ajaxError(function(event, xhr, settings, thrownError) {
    if (xhr.status === 401) {
        window.showNotification('warning', 'Su sesión ha expirado. Por favor, inicie sesión nuevamente.', 'Sesión Expirada');
        setTimeout(function() {
            window.location.href = '/Account/Login';
        }, 2000);
    } else if (xhr.status === 403) {
        window.showNotification('error', 'No tiene permisos para realizar esta acción.', 'Acceso Denegado');
    } else if (xhr.status >= 500) {
        window.showNotification('error', 'Ocurrió un error en el servidor. Por favor, intente nuevamente.', 'Error del Servidor');
    }
});

// Configurar CSRF token para todas las peticiones AJAX
$(document).ready(function() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    if (token) {
        $.ajaxSetup({
            beforeSend: function(xhr) {
                xhr.setRequestHeader("RequestVerificationToken", token);
            }
        });
    }
});

// Función para manejar la carga de imágenes con fallback
window.handleImageError = function(img) {
    img.src = '/images/default-product.jpg';
    img.onerror = null; // Prevenir loop infinito
};

// Auto-aplicar formato de números en inputs de tipo number con clase currency
$(document).on('input', '.currency-input', function() {
    var value = $(this).val().replace(/[^0-9]/g, '');
    if (value) {
        $(this).val(parseInt(value).toLocaleString());
    }
});
