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

        // Printers

        public const string PRINTERS = "/printers";
        public const string PRINTER_ID = "/printer/{id}";
        public const string PRINTER_ADD = "/printer-add";
        public const string PRINTER_EDIT = "/printer-edit/{id}";
        public const string PRINTER_DELETE = "/printer-delete/{id}";

        // Label Templates 

        public const string LABEL_TEMPLATES = "/label-templates";
        public const string LABEL_TEMPLATE_ID = "/label-template/{id}";
        public const string LABEL_TEMPLATE_ADD = "/label-template-add";
        public const string LABEL_TEMPLATE_EDIT = "/label-template-edit/{id}";
        public const string LABEL_TEMPLATE_DELETE = "/label-template-delete/{id}";

        // print label 

        public const string PRINT_EAN = "/print-ean";
        public const string PRINT_PRODUCTION_LABEL = "/print-production-label";

        // print Job

        public const string PRINT_JOBS = "print-jobs";
        public const string PRINT_JOBS_ID = "print-jobs/{id}";
        public const string PRINT_JOBS_CANCEL = "print-jobs/{id}/cancel";
        public const string PRINT_JOBS_REPRINT = "print-jobs/{id}/reprint";
        public const string PRINT_JOBS_PRINT = "print-jobs/{id}/print";
        public const string PRINT_JOBS_PREVIEW = "print-jobs/{id}/preview";

        // production orders 

        public const string PRODUCTION_ORDERS = "production-orders";
        public const string PRODUCTION_ORDERS_ID = "production-orders/{id}";
        public const string PRODUCTION_ORDER_LOTS = "production-orders/{productionOrderId}/production-lots";

        // operacje magazynowe 

        public const string STOCK_MOVEMENTS = "stock-movements";

        public const string WAREHOUSE_RECEIPTS = "warehouse-receipts";
    }
}
