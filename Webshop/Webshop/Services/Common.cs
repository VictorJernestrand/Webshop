using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class Common
    {
        public const string USER_ID = "UserId";
        public const string USER_NAME = "UserName";

        // Cart cookie
        public const string CART_COOKIE_NAME = "CustomerCartSessionId";
    }

    public class ApiURL
    {
        private const string URL = "https://localhost:44305/api/";

        // API-paths to User-controllers
        public const string USERS               = URL + "users/";
        public const string USERS_LOGIN         = URL + "users/login/";
        public const string USERS_REGISTER      = URL + "users/register/";
        public const string USERS_LOGIN_UPDATE  = URL + "users/loginupdate/";
        public const string USERS_INFO_UPDATE   = URL + "users/infoupdate/";

        // API-paths to content-controllers
        public const string PRODUCTS            = URL + "products/";
        public const string PRODUCTS_IN_CAT     = URL + "products/category/";
        public const string CATEGORIES          = URL + "categories/";
        public const string BRANDS              = URL + "brands/";
        public const string PAYMENTS            = URL + "payments/";

        // API-paths to cart-contorllers
        public const string CARTS               = URL + "carts/";
        public const string CARTS_CONTENT       = URL + "carts/content/";
        public const string CARTS_CONTENT_PAY   = URL + "carts/content_and_payment/";

        // API-paths to order-controllers
        public const string ORDERS              = URL + "orders/";
        public const string ORDERS_BY_USER      = URL + "orders/userorders/";
    }
}
