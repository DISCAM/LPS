using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public static class Urls
    {
        //Users / Auth

        public const string USERS = "/users";
        public const string USER_ID = "/user/{id}" ;

        public const string REGISTER = "/register";
        public const string LOGIN = "/login";
        public const string CDR = "/create-default-role";
        public const string AR = "/assign-role";
        public const string DELETE = "/delete-user";
        public const string EDIT = "/edit-user";

        // Products 

        public const string PRODUCTS = "products";
        public const string PRODUCT_ID = "product/{id}";
        public const string PRODUCT_ADD = "product-add";
        public const string PRODUCT_EDIT = "product-edit/{id}";
        public const string PRODUCT_DELETE = "product-delete/{id}";


        // Customers 

        public const string CUSTOMERS = "customers";
        public const string CUSTOMER_ID = "customer/{id}";
        public const string CUSTOMER_ADD = "customer-add";
        public const string CUSTOMER_EDIT = "customer-edit/{id}";
        public const string CUSTOMER_DELETE = "customer-delete/{id}";

    }
}
