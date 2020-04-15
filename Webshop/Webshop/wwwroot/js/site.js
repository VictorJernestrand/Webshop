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

    //console.log(productId + ", " + name);

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


// Remove 1 in quantity from selected product
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
                UpdateCartButton();                         // Update Cartbutton with total products and cost
                DecreaseSingleProductInCart(productId);     // Remove 1 from selected product from cart
                DisableBuyButton();                         // Disable "Buy" button
            } else {
                alert("Skit också, något gick fel. Försök igen eller kontakta vår sketna support!");
            }
        });
}


// Delete selected product from cart completely!
function DeleteCrapFromCart(cartProductId) {

    // Declare new FormData object
    let formData = new FormData();

    // Append form data
    formData.append("Id", cartProductId);
    formData.append("__RequestVerificationToken", GetAntiForgerytoken());

    fetch("https://localhost:44364/ShoppingCart/DeleteItemFromCart", {
        method: "POST",
        body: formData
    })
        .then((response) => {
            if (response.ok) {

                // Remove <tr> element from table
                document.getElementById("DumpCrap_" + cartProductId).remove();
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

    console.log("Update cart button");

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



// Update cartbutton information
function UpdateBuyButton() {
    let productCount = document.querySelectorAll("tr").length;

    // If cart is empty, reload cart and display a "Cart empty" message!
    if (productCount == 0) {
        console.log(productCount);
        location.reload(true);
    }

}



// Update amount for each product in the view (index.cshtml) for ShoppingCart
function IncreaseSingleProductInCart(productId) {

    var cartProduct = document.getElementById('cartProductId_' + productId);

    if (cartProduct == null)
        return;

    let total = parseInt(document.getElementById('cartProductId_' + productId).innerHTML) + 1;

    if (total > 0) {
        document.getElementById('DecreaseBtn_' + productId).disabled = false;
        document.getElementById('CashOut').disabled = false;
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


// Disable "Buy" button
function DisableBuyButton() {
    let elements = document.querySelectorAll(".productCounter");

    for (let i = 0; i < elements.length; i++) {
        if (elements[i].innerHTML != 0) 
            return;
    }

    // Disable "Buy" button!
    document.getElementById('CashOut').disabled = true;
}


// Invoke UpdateCartButton() as soon as page has loaded
$(document).ready(function () {
    UpdateCartButton();
})

// Go back to previously page (Used in ProductDetail View)
function goBack() {
    window.history.back();
}

// Editor for product FullDescription
ClassicEditor
    .create(document.querySelector('#editor'))
    .then(editor => {
        console.log(editor);
    })
    .catch(error => {
        console.error(error);
    });

// Editor for product specification
ClassicEditor
    .create(document.querySelector('#editor2'))
    .then(editor => {
        console.log(editor);
    })
    .catch(error => {
        console.error(error);
    });



// When the user scrolls the page, execute stickyFunction
window.onscroll = function () { stickyFunction() };

// Get the navbar
var navbar = document.getElementById("stickyNavbar");

// Get the offset position of the navbar
var sticky = navbar.offsetTop;

// Add the sticky class to the navbar when you reach its scroll position. Remove "sticky" when you leave the scroll position
function stickyFunction() {
    if (window.pageYOffset >= sticky) {
        navbar.classList.add("sticky")
    } else {
        navbar.classList.remove("sticky");
    }
}