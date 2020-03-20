// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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


// Add items to cart using AJAX and fetch framework
function AddCrapToCart(productId, name) {

    // Declare new FormData object
    let formData = new FormData();

    // Append form data
    formData.append("Id", productId);
    formData.append("__RequestVerificationToken", GetAntiForgerytoken())

    // Use Fetch-framework to POST formData
    fetch("https://localhost:44364/ShoppingCart/AddToCart", {
        method: "POST",
        body: formData
    })

        // Handle response
        .then((response) => {


            console.log(response);

            // If response was ok, update cart and display a message with the product name telling it was added to cart.
            if (response.ok) {
                UpdateCartButton();
                DisplayResponseMessage(name);
                IncreaseSingleProductInCart(productId);
            } else {
                alert("Skit också, något gick fel. Försök igen eller kontakta vår sketna support!");
            }
        });
}



function RemoveCrapFromCart(cartItemId, productId) {

    // Declare new FormData object
    let formData = new FormData();

    // Append form data
    formData.append("Id", cartItemId);
    formData.append("__RequestVerificationToken", GetAntiForgerytoken());

    fetch("https://localhost:44364/ShoppingCart/RemoveFromCart", {
        method: "POST",
        body: formData
    })
        .then((response) => {
            if (response.ok) {
                UpdateCartButton();
                DecreaseSingleProductInCart(productId);
            } else {
                alert("Skit också, något gick fel. Försök igen eller kontakta vår sketna support!");
            }
        });
}



function DeleteCrapFromCart(cartId) {

    // Declare new FormData object
    let formData = new FormData();

    // Append form data
    formData.append("Id", cartId);
    formData.append("__RequestVerificationToken", GetAntiForgerytoken());

    fetch("https://localhost:44364/ShoppingCart/DeleteItemFromCart", {
        method: "POST",
        body: formData
    })
        .then((response) => {
            if (response.ok) {
                //document.getElementById("DumpCrap_" + cartId).style.display = "none";
                
                // Remove <tr> element from table
                document.getElementById("DumpCrap_" + cartId).remove();
                UpdateCartButton();
                UpdateBuyButton();
            } else {
                alert("Skit också, något gick fel. Försök igen eller kontakta vår sketna support!");
            }
        });
}



// Show user a message notification that the product was added to cart
function DisplayResponseMessage(productName) {

    // If productName is null, stop here! Dont show any message
    if (productName == null)
        return;

    document.getElementById('item_added_to_cart').innerHTML = productName;
    $('#exampleModalCenter').modal('show');
}



// Update cart button with info on total items and current cost
function UpdateCartButton() {

    // Get cart content using an AJAX call to GetCartContent() actionmethod
    fetch('https://localhost:44364/ShoppingCart/GetCartContent')

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



function UpdateBuyButton() {
    let productCount = document.querySelectorAll("tr").length;
    document.getElementById('CashOutButton').disabled = (productCount == 0) ? true : false;
}



// Update amount for each product in the view (index.cshtml) for ShoppingCart
function IncreaseSingleProductInCart(productId) {
    let total = parseInt(document.getElementById('cartProductId_' + productId).innerHTML) + 1;

    if (total > 0) {
        document.getElementById('DecreaseBtn_' + productId).disabled = false;
    }

    if (total >= 50) {
        document.getElementById('IncreaseBtn_' + productId).disabled = true;
    }

    document.getElementById('cartProductId_' + productId).innerHTML = total;
}



// Update amount for each product in the view (index.cshtml) for ShoppingCart
function DecreaseSingleProductInCart(productId) {
    let total = parseInt(document.getElementById('cartProductId_' + productId).innerHTML) - 1;

    if (total == 0) {
        document.getElementById('DecreaseBtn_' + productId).disabled = true;
    }

    if (total <= 50) {
        document.getElementById('IncreaseBtn_' + productId).disabled = false;
    }

    document.getElementById('cartProductId_' + productId).innerHTML = total;
}



// Get antiforgerytoken from hidden div-element
function GetAntiForgerytoken() {
    return document.getElementById('AntiForgeryToken').innerHTML;
}



// Invoke UpdateCartButton() as soon as page has loaded
$(document).ready(function () {
    UpdateCartButton();
})
