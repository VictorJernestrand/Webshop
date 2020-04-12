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
        public const string CART_COOKIE_NAME = "HeavyMetalCart";
    }

    public class ApiURL
    {
        private const string URL = "https://localhost:44305/api/";

        // API-paths to User-controllers
        public const string USERS               = URL + "users/";
        public const string USER_BY_ID          = URL + "users/userid/";
        public const string USERS_LOGIN         = URL + "users/login/";
        public const string USERS_REGISTER      = URL + "users/register/";
        public const string USERS_LOGIN_UPDATE  = URL + "users/loginupdate/";
        public const string USERS_INFO_UPDATE   = URL + "users/infoupdate/";

        // API-paths to products-controllers
        public const string PRODUCTS            = URL + "products/";
        public const string PRODUCTS_IN_CAT     = URL + "products/category/";
        public const string CATEGORIES          = URL + "categories/";
        public const string BRANDS              = URL + "brands/";
        public const string PAYMENTS            = URL + "payments/";
        public const string SEARCH              = URL + "products/search/";

        // API-paths to cart-controllers
        public const string CARTS               = URL + "carts/";
        public const string CARTS_CONTENT       = URL + "carts/content/";
        public const string CARTS_CONTENT_PAY   = URL + "carts/content_and_payment/";

        // API-paths to order-controllers
        public const string ORDERS              = URL + "orders/";
        public const string ORDER_BY_ID         = URL + "orders/id/";
        public const string ORDERS_BY_USER      = URL + "orders/userorders/";
        public const string All_ORDERS_BY_STATUS= URL + "orders/allorders/";
        public const string ORDERREQBYID        = URL + "orders/orderrequest/";

        //API-path to status-controllers
        public const string STATUS              = URL + "status/";

        // API-path to ratings-controllers
        public const string RATINGS_BY_PRODUCT_ID = URL + "ratings/product/";
        public const string RATING_BY_ID        = URL + "ratings/";
        public const string RATINGS_POST        = URL + "ratings/";

    }
}
