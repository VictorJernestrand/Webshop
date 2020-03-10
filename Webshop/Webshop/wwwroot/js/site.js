﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/*
function GetCategories() {

    var categoryElement = document.getElementById("CategoryDropdown");

    // Get all categories from our WebAPI in JSON format
    fetch('https://localhost:44305/api/categories')
        .then((response) => {
            return response.json();
        })
        .then((data) => {
            for (const [key, value] of Object.entries(data)) {
                var optionElement = document.createElement("a");
                optionElement.setAttribute('href', 'Product?catid=' + value.id);
                optionElement.setAttribute('class', 'dropdown-item');
                optionElement.text = value.name;

                categoryElement.appendChild(optionElement);
            }
        });
}

$(document).ready(function () {
    GetCategories();
})
*/


// Add items to cart using AJAX
function AddCrapToCart(productId, name, antiForgeryToken) {

    // Declare new FormData object
    let formData = new FormData();

    // Append form data
    formData.append("Id", productId);
    formData.append("__RequestVerificationToken", antiForgeryToken)

    // Use Fetch-framework to POST formData
    fetch("https://localhost:44364/Product/AddToCart", {
        method: "POST",
        body: formData
    })

        // Handle response
        .then((response) => {

            // If response was ok, update cart and display a message with the product name telling it was added to cart.
            if (response.ok) {
                UpdateCartButton();
                DisplayResponseMessage(name);
                UpdateSingleProductInCart(productId);
            } else {
                alert("Skit också, något gick fel. Försök igen eller kontakta vår sketna support!");
            }
        });
}

// Show user a message notification that the product was added to cart
function DisplayResponseMessage(productName) {

    // If productName is null, break here
    if (productName == null)
        return;

    document.getElementById('item_added_to_cart').innerHTML = productName;
    $('#exampleModalCenter').modal('show');
}

// Update cart button with info on total items and current cost
function UpdateCartButton() {

    // Get cart content using an AJAX call to GetCartContent() actionmethod
    fetch('https://localhost:44364/Product/GetCartContent')

        // Get response as Json data
        .then((response) => {
            return response.json();
        })

        // Update cart button with totalItems and totalCost by manipulating DOM
        .then((data) => {

            document.getElementById('cartTotalItems').innerHTML = data.totalItems;
            document.getElementById('cartTotalCost').innerHTML = data.totalCost;
        });
}


// Update amount for each product in the view (index.cshtml) for ShoppingCart
function UpdateSingleProductInCart(productId) {
    let total = parseInt(document.getElementById('cartProductId_' + productId).innerHTML) + 1;
    document.getElementById('cartProductId_' + productId).innerHTML = total;
}


// Invoke UpdateCartButton() as soon as page has loaded
$(document).ready(function () {
    UpdateCartButton();
})
