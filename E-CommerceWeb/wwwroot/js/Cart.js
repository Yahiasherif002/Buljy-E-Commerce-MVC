document.addEventListener('DOMContentLoaded', function () {
    // Handle quantity updates
    document.querySelectorAll('.quantity-form').forEach(form => {
        const productId = form.getAttribute('data-id');
        const quantityInput = form.querySelector('.quantity-input');
        const decreaseBtn = form.querySelector('.decrease-btn');
        const increaseBtn = form.querySelector('.increase-btn');

        // Increase quantity
        increaseBtn.addEventListener('click', (e) => {
            e.preventDefault();
            updateQuantity(productId, 'increase');
        });

        // Decrease quantity
        decreaseBtn.addEventListener('click', (e) => {
            e.preventDefault();
            updateQuantity(productId, 'decrease');
        });
    });

    function updateQuantity(productId, operation) {
        const form = document.querySelector(`form[data-id="${productId}"]`);
        const cartItemElement = form.closest('.cart-item');

        // Show loading state
        cartItemElement.style.opacity = '0.5';

        // Get the antiforgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const formData = new FormData();
        formData.append('id', productId);
        formData.append('operation', operation);

        fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    if (operation === 'decrease' && data.quantity === 0) {
                        // Remove the cart item from DOM if quantity is 0
                        cartItemElement.remove();

                        // Update cart count in title
                        const cartTitle = document.querySelector('.cart-title');
                        if (cartTitle) {
                            cartTitle.textContent = `Shopping Cart (${data.itemCount} items)`;
                        }

                        // If no items left, reload the page to show empty cart message
                        if (data.itemCount === 0) {
                            window.location.reload();
                            return;
                        }
                    } else {
                        // Update quantity input
                        const quantityInput = form.querySelector('.quantity-input');
                        quantityInput.value = data.quantity;

                        // Update item price
                        const priceDisplay = cartItemElement.querySelector('.price-display');
                        if (priceDisplay) {
                            priceDisplay.textContent = data.currentPrice;
                        }

                        // Update cart total
                        const cartTotalElements = document.querySelectorAll('#cart-total');
                        cartTotalElements.forEach(element => {
                            element.textContent = data.cartTotal;
                        });

                        // Update order summary total
                        const orderTotal = document.querySelector('.summary-item .fw-bold.fs-5');
                        if (orderTotal) {
                            orderTotal.textContent = data.orderTotal;
                        }
                    }

                    // Show success notification
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            })
            .catch(error => {
                console.error('Error:', error);
                toastr.error('An error occurred while updating the cart');
            })
            .finally(() => {
                // Remove loading state
                cartItemElement.style.opacity = '1';
            });
    }
});