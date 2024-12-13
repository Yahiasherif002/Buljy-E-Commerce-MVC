// Theme switcher functionality
document.addEventListener('DOMContentLoaded', function () {
    // Check for saved theme preference or default to 'light'
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);
});

function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';

    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    updateThemeIcon(newTheme);
}

function updateThemeIcon(theme) {
    const icon = document.getElementById('theme-icon');
    if (icon) {
        icon.className = theme === 'light' ? 'bi bi-moon-fill' : 'bi bi-sun-fill';
    }
}


// Add to Cart Animation
function showAddToCartAnimation(button) {
    // Store original button content
    const originalContent = button.innerHTML;

    // Update button to success state
    button.innerHTML = '<i class="bi bi-check2"></i> Added!';
    button.classList.add('btn-success');
    button.classList.remove('btn-primary');

    // Show toast notification
    const toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 1500,
        timerProgressBar: true
    });

    toast.fire({
        icon: 'success',
        title: 'Item added to cart'
    });

    // Reset button after animation
    setTimeout(() => {
        button.innerHTML = originalContent;
        button.classList.remove('btn-success');
        button.classList.add('btn-primary');
        button.disabled = false;
    }, 1500);
}

// Add to Cart Handler
function addToCart(productId, quantity = 1) {
    const button = event.currentTarget;
    button.disabled = true;

    fetch(`/Customer/Home/Details`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showAddToCartAnimation(button);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!'
                });
                button.disabled = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            button.disabled = false;
        });
}