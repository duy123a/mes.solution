window.togglePasswordVisibility = function (inputId, btn) {
    const input = document.getElementById(inputId);
    if (input) {
        const icon = btn.querySelector('i');

        if (input.type === "password") {
            input.type = "text";
            icon.classList.remove("bi-eye");
            icon.classList.add("bi-eye-slash");
        } else {
            input.type = "password";
            icon.classList.remove("bi-eye-slash");
            icon.classList.add("bi-eye");
        }
    }
};

window.showLoading = function () {
    document.getElementById("loadingOverlay")?.classList.remove("d-none");

    document.querySelectorAll("button[type=submit], input[type=submit]").forEach(btn => {
        btn.disabled = true;
    });
};

window.hideLoading = function () {
    document.getElementById("loadingOverlay")?.classList.add("d-none");

    document.querySelectorAll("button[type=submit], input[type=submit]").forEach(btn => {
        btn.disabled = false;
    });
};

window.showLogoutModal = function () {
    const modal = new bootstrap.Modal(document.getElementById('confirmLogoutModal'));
    modal.show();
}

// Handle logout confirmation
document.addEventListener('DOMContentLoaded', function () {
    const logoutBtn = document.getElementById('confirmLogoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function () {
            window.location.href = '/Account/Logout';
        });
    }
});