using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipEngine_UI.Resources.Classes
{
    public class SE_Carrier_Connection
    {

        public static string connections_baseURL = "https://api.shipengine.com/v1/connections/carriers/";
        public static bool has_settings = false;

        //STAMPS x USPS
        public static string connections_stamps_com = "{" +
            "\r\n    \"nickname\": \"My stamps.com account\"," +
            "\r\n    \"username\": \"__YOUR_USERNAME_HERE__\"," +
            "\r\n    \"password\": \"__YOUR_PASSWORD_HERE__\"" +
            "\r\n}" +
            "\r\n";

        //ENDICIA
        public static string connections_endicia = "{" +
            "\r\n  \"nickname\": \"My Endicia account\"," +
            "\r\n  \"account\": \"__YOUR_USERNAME_HERE__\"," +
            "\r\n  \"passphrase\": \"__YOUR_PASSWORD_HERE__\"" +
            "\r\n}";

        //FEDEX - has_settings = true;
        public static string connections_fedex = "{" +
            "\r\n  \"nickname\": \"My FedEx account\"," +
            "\r\n  \"account_number\": \"123456789\"," +
            "\r\n  \"company\": \"Example Corp.\"," +
            "\r\n  \"first_name\": \"John\"," +
            "\r\n  \"last_name\": \"Doe\"," +
            "\r\n  \"phone\": \"111-111-1111\"," +
            "\r\n  \"address1\": \"4009 Marathon Blvd.\"," +
            "\r\n  \"address2\": \"Suite 300\"," +
            "\r\n  \"city\": \"Austin\"," +
            "\r\n  \"state\": \"TX\"," +
            "\r\n  \"postal_code\": \"78756\"," +
            "\r\n  \"country_code\": \"US\"," +
            "\r\n  \"email\": \"john.doe@example.com\"," +
            "\r\n  \"agree_to_eula\": \"true\"" +
            "\r\n}";

        public static string connections_fedex_settings = "{" +
            "\r\n    \"nickname\": \"my fedex account\"," +
            "\r\n    \"pickup_type\": \"regular_pickup\"," +
            "\r\n    \"smart_post_hub\": \"dallas_tx\"," +
            "\r\n    \"smart_post_endorsement\": \"address_service_requested\"," +
            "\r\n    \"is_primary_account\": \"false\"," +
            "\r\n    \"signature_image\" : \"base64string\"," +
            "\r\n    \"letterhead_image\" : \"base64string\"" +
            "\r\n}";

        //UPS - has_settings = true;
        public static string connections_ups = "{" +
            "\r\n  \"nickname\": \"My UPS account\"," +
            "\r\n  \"account_number\": \"123456789\"," +
            "\r\n  \"account_country_code\": \"US\"," +
            "\r\n  \"account_postal_code\": \"78756\"," +
            "\r\n  \"company\": \"Example Corp.\"," +
            "\r\n  \"first_name\": \"John\"," +
            "\r\n  \"last_name\": \"Doe\"," +
            "\r\n  \"address1\": \"4009 Marathon Blvd.\"," +
            "\r\n  \"address2\": \"Suite 300\"," +
            "\r\n  \"city\": \"Austin\"," +
            "\r\n  \"state\": \"TX\"," +
            "\r\n  \"postal_code\": \"78756\"," +
            "\r\n  \"country_code\": \"US\"," +
            "\r\n  \"phone\": \"111-111-1111\"," +
            "\r\n  \"email\": \"john.doe@example.com\"," +
            "\r\n  \"agree_to_technology_agreement\": \"true\"," +
            "\r\n  \"invoice\": {" +
            "\r\n    \"control_id\": \"10Z3\"," +
            "\r\n    \"invoice_number\": \"0000A123B4567\"," +
            "\r\n    \"invoice_amount\": \"100.98\"," +
            "\r\n    \"invoice_date\": \"2017-04-01\"" +
            "\r\n  }" +
            "\r\n}";

        public static string connections_ups_settings = "{" +
            "\r\n  \"nickname\": \"my ups account\"," +
            "\r\n  \"is_primary_account\": \"true\"," +
            "\r\n  \"pickup_type\": \"daily_pickup\"," +
            "\r\n  \"use_carbon_neutral_shipping_program\": \"true\"," +
            "\r\n  \"use_ground_freight_pricing\": \"true\"," +
            "\r\n  \"use_negotiated_rates\": \"true\"," +
            "\r\n  \"account_postal_code\": \"78756\"," +
            "\r\n  \"invoice\": {" +
            "\r\n    \"control_id\": \"1234\"," +
            "\r\n    \"invoice_number\": \"12345\"," +
            "\r\n    \"invoice_amount\": \"1.99\"," +
            "\r\n    \"invoice_date\": \"2019-7-26\"" +
            "\r\n  }," +
            "\r\n  \"use_consolidation_services\": \"true\"," +
            "\r\n  \"use_order_number_on_mail_innovations_labels\": \"true\"," +
            "\r\n  \"mail_innovations_endorsement\": \"change_service_requested\"," +
            "\r\n  \"mail_innovations_cost_center\": \"TEST\"" +
            "\r\n}";
    }
}
