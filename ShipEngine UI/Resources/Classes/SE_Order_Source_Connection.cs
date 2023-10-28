using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipEngine_UI.Resources.Classes
{
    public class SE_Order_Source_Connection
    {

        public static string Order_Source_Connection_deactivate = "/v-beta/connections/order_sources/amazon_us/:id/deactivate";

        public static string Order_Source_Connection_reactivate = "/v-beta/connections/order_sources/amazon_us/:id/reactivate";


        //AMAZON

        public static string Order_Source_Connection_amazon_url = "/v-beta/connections/order_sources/amazon";

        public static string Order_Source_Connection_insructions_amazon = "" +
            "Go to http://developer.amazonservices.com, click Sign up for MWS, and log in with your Amazon Seller credentials." +
            "\r\nSelect the \"I want to give a developer access...\" option, and enter: Developer Name: ShipStation Developer Account No: 4289-4417-4486" +
            "\r\nFollow the prompts, then use the Merchant Seller ID and MWS Auth Token in your request.";

        public static string Order_Source_Connection_amazon = "{" +
            "\r\n  \"order_source_nickname\": \"Amazon Account\"," +
            "\r\n  \"merchant_seller_id\": \"<YOUR_SELLER_ID_HERE>\"," +
            "\r\n  \"product_identifier\": \"<YOUR_SKU_HERE>\"," +
            "\r\n  \"mws_auth_token\": \"<MWS_AUTH_TOKEN_HERE>" +
            "\"\r\n}";

        //Brightpearl

        public static string Order_Source_Connection_brightpearl_url = "/v-beta/connections/order_sources/brightpearl";

        public static string Order_Source_Connection_brightpearl = "{" +
            "\r\n  \"order_source_nickname\": \"Brightpearl Account\"," +
            "\r\n  \"user_name\": \"<YOUR_USERNAME_HERE>\"," +
            "\r\n  \"integration_url\": \"<YOUR_INTEGRATION_URL_HERE>\"," +
            "\r\n  \"password\": \"<YOUR_PASSWORD_HERE>\"," +
            "\r\n  \"settings\": {" +
            "\r\n    \"automatically_import_sales_orders\": true," +
            "\r\n    \"automatically_create_shipments\": true" +
            "\r\n  }" +
            "\r\n}";

        //Cratejoy

        public static string Order_Source_Connection_cratejoy_url = "/v-beta/connections/order_sources/cratejoy";

        public static string Order_Source_Connection_cratejoy = "{" +
            "\r\n  \"order_source_nickname\": \"Cratejoy Account\"," +
            "\r\n  \"user_name\": \"<YOUR_USERNAME_HERE>\"," +
            "\r\n  \"password\": \"<YOUR_PASSWORD_HERE>\"," +
            "\r\n  \"url\": \"<YOUR_URL_HERE>\"" +
            "\r\n}";

        //ChannelAdvisor

        public static string Order_Source_Connection_channeladvisor_url = "/v-beta/connections/order_sources/channeladvisor";

        public static string Order_Source_Connection_channeladvisor = "{" +
            "\r\n  \"order_source_nickname\": \"ChannelAdvisor Account\"," +
            "\r\n  \"profile_id\": \"<YOUR_PROFILE_ID_HERE>\"," +
            "\r\n  \"product_identifier\": \"<YOUR_PRODUCT_IDENTIFIER_HERE>\"" +
            "\r\n}";

        //Magento

        public static string Order_Source_Connection_magento_url = "/v-beta/connections/order_sources/magento";

        public static string Order_Source_Connection_magento = "{" +
            "\r\n  \"order_source_nickname\": \"Magento Account\"," +
            "\r\n  \"order_source_url\": \"<YOUR_ORDER_SOURCE_URL_HERE>\"," +
            "\r\n  \"username\": \"<YOUR_USERNAME_HERE>\"," +
            "\r\n  \"password\": \"<YOUR_PASSWORD_HERE>\"," +
            "\r\n  \"settings\": {" +
            "\r\n    \"automatically_import_sales_orders\": true," +
            "\r\n    \"automatically_create_shipments\": true" +
            "\r\n  }" +
            "\r\n}";

        //Volusion

        public static string Order_Source_Connection_volusion_url = "/v-beta/connections/order_sources/volusion";

        public static string Order_Source_Connection_volusion = "{" +
            "\r\n  \"order_source_nickname\": \"Volusion Account\"," +
            "\r\n  \"api_url\": \"<YOUR_API_URL_HERE>\"" +
            "\r\n}";

        //Walmart

        public static string Order_Source_Connection_walmart_url = "/v-beta/connections/order_sources/walmart";

        public static string Order_Source_Connection_walmart = "{" +
            "\r\n  \"order_source_nickname\": \"Walmart Account\"," +
            "\r\n  \"consumer_id\": \"<YOUR_CONSUMER_ID_HERE>\"," +
            "\r\n  \"private_key\": \"<YOUR_PRIVATE_KEY_HERE>\"" +
            "\r\n}";

        //Woocommerce
        public static string Order_Source_Connection_woocommerce_url = "/v-beta/connections/order_sources/woocommerce";

        public static string Order_Source_Connection_woocommerce = "{" +
            "\r\n  \"order_source_nickname\": \"Woocommerce Account\"," +
            "\r\n  \"auth_key\": \"<YOUR_AUTH_KEY_HERE>\"," +
            "\r\n  \"order_source_url\": \"<YOUR_ORDER_SOURCE_URL_HERE>\"," +
            "\r\n  \"settings\": {" +
            "\r\n    \"automatically_import_sales_orders\": true," +
            "\r\n    \"automatically_create_shipments\": true" +
            "\r\n  }" +
            "\r\n}";
    }
}
