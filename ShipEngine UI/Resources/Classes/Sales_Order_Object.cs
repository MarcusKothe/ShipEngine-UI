using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipEngine_UI
{
    //THIS CLASS REPRESENTS THE SALES ORDER OBJECT
    public class Sales_Order_Object
    {

        //SALES ORDER ---------------------------------------------------------------------------------------------------------------------------------
        public static string sales_order_id = "";
        public static string sales_order_external_order_id = ""; //This is the Order ID we received from the order source.
        public static string sales_order_external_order_number = ""; //This is the Order ID we received from the order source.

        //Order Source
        public static string sales_order_source_id = "";
        public static string sales_order_source_nickname = ""; //Nickname you assign to the order source when connecting, e.g. "My Amazon Store".
        public static string sales_order_source_code = ""; //The API code used to identify the type of order source, e.g. "amazon_ca"
        public static string sales_order_source_friendly_name = ""; //The human-readable name for the order source, e.g. "Amazon CA" instead of "amazon_ca"

        //refresh_info
        public static string sales_order_status = ""; //enumerated string, idle, preparing_update, updating, error, unknown
        public static string sales_order_last_refresh_attempt = ""; //date string, (ISO 8601 Standard)2019-07-25T15:24:46.657Z
        public static string sales_order_refresh_date = ""; //date string, (ISO 8601 Standard)2019-07-25T15:24:46.657Z

        public static string sales_order_active = ""; //Bool Whether or not your order source is currently activated or deactivated

        //SALES ORDER ITEM ----------------------------------------------------------------------------------------------------------------------------
        public static string sales_order_item_id = "";

        //Line item Details
        public static string sales_order_item_line_item_details_name = "";
        public static string sales_order_item_line_item_details_sku = ""; 
        public static string sales_order_item_line_item_details_weight_unit = "";
        public static string sales_order_item_line_item_details_weight_value = "";

        //Ship To
        public static string sales_order_item_ship_to_name = "";
        public static string sales_order_item_ship_to_company_name = "";
        public static string sales_order_item_ship_to_phone = "";
        public static string sales_order_item_ship_to_address_line1 = "";
        public static string sales_order_item_ship_to_address_line2 = "";
        public static string sales_order_item_ship_to_address_line3 = "";
        public static string sales_order_item_ship_to_city_locality = "";
        public static string sales_order_item_ship_to_state_province = "";
        public static string sales_order_item_ship_to_postal_code = "";
        public static string sales_order_item_ship_to_country_code = "";

        //Requested Shipping Options
        public static string sales_order_item_requested_shipping_options_shipping_service = "";
        public static string sales_order_item_requested_shipping_options_ship_date = "";

        //Price Summary
        public static string sales_order_item_price_summary_unit_price = "";
        public static string sales_order_item_price_summary_estimated_tax = "";
        public static string sales_order_item_price_summary_estimated_shipping = "";
        public static string sales_order_item_price_summary_total = "";


        public static string sales_order_item_quantity = "";
        public static string sales_order_item_is_gift = "";

        //SALES ORDER STATUS ----------------------------------------------------------------------------------------------------------------------------

        public static string sales_order_status_payment_status = ""; //enumerated string, unknown, paid, unpaid, partially_paid
        public static string sales_order_status_fulfillment_status = ""; //enumerated string, unknown, fulfilled, unfulfilled, partially_fulfilled, on_hold.
        public static string sales_order_status_is_cancelled = "";

        //SALES ORDER PAYMENT DETAILS ------------------------------------------------------------------------------------------------------------------------

        public static string sales_order_subtotal = "";
        public static string sales_order_estimated_shipping = "";
        public static string sales_order_estimated_tax = "";
        public static string sales_order_grand_total = "";

        //SALES ORDER CUSTOMER ----------------------------------------------------------------------------------------------------------------------------

        public static string sales_order_customer_name = "";
        public static string sales_order_customer_phone = "";
        public static string sales_order_customer_email = "";

        //SALES ORDER BILL TO----------------------------------------------------------------------------------------------------------------------------
        public static string sales_order_bill_to_email = "";
        public static string sales_order_bill_to_name = "";
        public static string sales_order_bill_to_company_name = "";
        public static string sales_order_bill_to_phone = "";
        public static string sales_order_bill_to_address_line1 = "";
        public static string sales_order_bill_to_address_line2 = "";
        public static string sales_order_bill_to_address_line3 = "";
        public static string sales_order_bill_to_city_locality = "";
        public static string sales_order_bill_to_state_province = "";
        public static string sales_order_bill_to_postal_code = "";
        public static string sales_order_bill_to_country_code = "";
    }
}
