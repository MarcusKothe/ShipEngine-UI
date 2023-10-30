using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipEngine_UI
{
    public class ShipEngineUI
    {
        public static bool has_error = false;

        public static string label_id = "";
        public static string void_label_id_Response = "";

        public static string Tracking_number = "";

        public static string RATE_AMOUNT = "";

        public static string create_label_error = "";

        public static string branded_tracking_url = "";

        public static string rate_response = "";

        //Numbers

        public static string[] Numbers = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "."};

        //SHIPENGINE GET LABEL URL

        public static string get_label_URL = "";

        //MANIFEST
        public static string manifest_pdf_url = "";

        //GET LABEL IMAGE URL FROM FORM LOAD
        public static string label_url = "";
        public static string label_url_label_id = "";

        //Sales order ID
        public static string external_order_number = "";
        public static string sales_order_id = "";
        public static string fulfillment_status = "";

        //API KEY
        public static string apiKey = "";

        //ENDPOINT URL
        public static string urlString = "";

        //SHIP TO
        public static string shipTo_name = ""; //Required
        public static string shipTo_phone = ""; //Required
        public static string shipTo_email = "";
        public static string shipTo_company_name = "";
        public static string shipTo_address_line1 = ""; //Required
        public static string shipTo_address_line2 = "";
        public static string shipTo_address_line3 = "";
        public static string shipTo_city_locality = ""; //Required
        public static string shipTo_state_province = ""; //Required
        public static string shipTo_postal_code = ""; //Required
        public static string shipTo_country_code = ""; //Required
        public static string shipTo_address_residential_indicator = ""; //Required
        public static string shipTo_instructions = "";

        //SHIP FROM
        public static string shipFrom_name = ""; //Required
        public static string shipFrom_phone = ""; //Required
        public static string shipFrom_email = "";
        public static string shipFrom_company_name = "";
        public static string shipFrom_address_line1 = ""; //Required
        public static string shipFrom_address_line2 = "";
        public static string shipFrom_address_line3 = "";
        public static string shipFrom_city_locality = ""; //Required
        public static string shipFrom_state_province = ""; //Required
        public static string shipFrom_postal_code = ""; //Required
        public static string shipFrom_country_code = ""; //Required
        public static string shipFrom_address_residential_indicator = ""; //Required
        public static string shipFrom_instructions = "";

        //RETURNS, BOOL TRUE OR FALSE
        public static string is_return = "false";

        //CONFIRMATION
        public static string[] confirmation = new string[] { "none", "delivery", "signature", 
        "adult_signature", "direct_signature", "delivery_mailed", "verbal_confirmation"};

        //CUSTOMS
        public static string[] contents = new string[] { "merchandise", "documents", "gift", "returned_goods", "sample"};
        public static string[] non_delivery = new string[] { "return_to_sender", "treat_as_abandoned"};

        public static string customs_items_description = "";
        public static string customs_items_quantity = "";
        public static string customs_items_value_currency = "";
        public static string customs_items_value_amount = "";
        public static string customs_items_weight_value = "";
        public static string customs_items_weight_unit = "";
        public static string customs_items_harmonized_tariff_code = "";
        public static string customs_items_country_of_origin = "";
        public static string customs_items_unit_of_measure = "";
        public static string customs_items_sku = "";
        public static string customs_items_sku_description = "";

        //ADVANCED OPTIONS
        public static string advanced_options_bill_to_account = "";
        public static string advanced_options_bill_to_country_code = "";
        public static string advanced_options_bill_to_party = null;
        public static string advanced_options_bill_to_postal_code = "";
        public static string advanced_options_contains_alcohol = "";
        public static string advanced_options_delivered_duty_paid = "";
        public static string advanced_options_dry_ice = "";
        public static string advanced_options_dry_ice_weight_value = "";
        public static string advanced_options_dry_ice_weight_unit = "";
        public static string advanced_options_non_machinable = "";
        public static string advanced_options_saturday_delivery = "";
        public static string advanced_options_fedex_freight = "";
        public static string advanced_options_shipper_load_and_count = "";
        public static string advanced_options_booking_confirmation = "";
        public static string advanced_options_use_ups_ground_freight_pricing = "";
        public static string advanced_options_freight_class = "";
        public static string advanced_options_custom_field1 = "";
        public static string advanced_options_custom_field2 = "";
        public static string advanced_options_custom_field3 = "";
        public static string advanced_options_origin_type = "";
        public static string advanced_options_shipper_release = "";
        public static string advanced_options_collect_on_delivery_payment_type = "";
        public static string advanced_options_collect_on_delivery_payment_amount_currency = "";
        public static string advanced_options_collect_on_delivery_payment_amount_amount = "";
        public static string advanced_options_third_party_consignee = "";
        public static string advanced_options_order_source_code = "";

        //PACKAGES
        public static string packages_package_id = "";
        public static string packages_package_code = "";
        public static string packages_content_description = "";
        public static string packages_weight_value = "";
        public static string packages_wieght_unit = "";
        public static string packages_dimensions_unit = "";
        public static string packages_dimensions_length = "";
        public static string packages_dimensions_width = "";
        public static string packages_dimensions_height = "";
        public static string insurance_provider = "";
        public static string packages_insured_value_currency = "";
        public static string packages_insured_value_amount = "";
        public static string packages_label_messages_reference1 = "";
        public static string packages_label_messages_reference2 = "";
        public static string packages_label_messages_reference3 = "";
        public static string packages_external_package_id = "";


        public static string is_return_label = "";
        public static string rma_number = "";
        public static string charge_event = "";
        public static string outbound_label_id = "";
        public static string validate_address = "";

        //Branded Tracking

        public static string bTracking_carrier_code = "";
        public static string bTracking_service_code = "";
        public static string bTracking_to_city_locality = "";
        public static string bTracking_to_state_province = "";
        public static string bTracking_to_postal_code = "";
        public static string bTracking_to_country_code = "";
        public static string bTracking_from_city_locality = "";
        public static string bTracking_from_state_province = ""; 
        public static string bTracking_from_postal_code = "";
        public static string bTracking_from_country_code = "";

    }
}
