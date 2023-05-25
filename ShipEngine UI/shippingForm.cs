using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace ShipEngine_UI
{
    public partial class shippingForm : Form
    {
        public shippingForm()
        {
            InitializeComponent();
        }

        private void shippingForm_Load(object sender, EventArgs e)
        {

            ship_Date_DateTimePicker.MinDate = DateTime.Today;
            ship_Date_DateTimePicker.Format = DateTimePickerFormat.Custom;
            ship_Date_DateTimePicker.CustomFormat = "dd-MM-yyyy";

            shipFrom_address_residential_indicator_comboBox.SelectedIndex = 1;
            shipTo_address_residential_indicator_comboBox.SelectedIndex = 1;

            //GET CARRIER ACCOUNTS
            try
            {
                //URL SOURCE
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/carriers";

                //REQUEST
                WebRequest requestObject = WebRequest.Create(ShipEngineUI.urlString);
                requestObject.Method = "GET";

                //ADD API KEY TO HEADER
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;
                
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("carrier_id") == true)
                            {

                                string carrier_name1 = currentLine.Replace(" \"carrier_id\": \"", "");
                                string carrier_name = carrier_name1.Replace("\",", "");

                                //add to textbox
                                carrier_id_RichTextBox.Text = carrier_id_RichTextBox.Text.Trim() + "," + Environment.NewLine + carrier_name.Trim() + "|";

                            }
                            else if (currentLine.Contains("carrier_code") == true)
                            {
                                string carrier_code1 = currentLine.Replace("\"carrier_code\": \"", "");
                                string carrier_code = carrier_code1.Replace("\",", "");

                                carrier_id_RichTextBox.Text = carrier_id_RichTextBox.Text + carrier_code.Trim();
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] carrier_id_list1 = carrier_id_RichTextBox.Text.Split(',');
                        string[] carrier_id_list = carrier_id_list1.Distinct().ToArray();
                        foreach (string carrier_id in carrier_id_list)
                        {
                            if (carrier_id.Trim() == "")
                                continue;

                            carrier_id_ComboBox.Items.Add(carrier_id.Trim());
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                this.Close();

                if (HTTPexception.Message.Contains("401"))
                {
                    apiKeyPrompt ApiKeyForm = new apiKeyPrompt();
                    MessageBox.Show("ShipEngine returned 401 Unauthorized, please check your API Key.");

                    ShipEngineUI.has_error = true;

                    ApiKeyForm.ShowDialog();
                }

            }

            //GET WAREHOUSES
            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/warehouses";

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET";

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;


                //Get all warehouseId's
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("warehouse_id") == true)
                            {

                                string warehouse_id1 = currentLine.Replace(" \"warehouse_id\": \"", "");
                                string warehouse_id = warehouse_id1.Replace("\",", "");

                                //add to textbox
                                warehouse_id_RichTextBox.Text = warehouse_id_RichTextBox.Text.Trim() + "," + Environment.NewLine + warehouse_id.Trim();

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] warehouse_id_list1 = warehouse_id_RichTextBox.Text.Split(',');
                        string[] warehouse_id_list = warehouse_id_list1.Distinct().ToArray();
                        foreach (string warehouse_id in warehouse_id_list)
                        {
                            if (warehouse_id.Trim() == "")
                                continue;

                            warehouse_id_ComboBox.Items.Add(warehouse_id.Trim());
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                this.Close();
            }

            //GET SALES ORDERS
            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v-beta/sales_orders";

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET";

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;


                //Get all sales orders
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {
                        
                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("sales_order_id") == true)
                            {

                                string sales_order_id1 = currentLine.Replace("\"sales_order_id\": \"", "");
                                string sales_order_id = sales_order_id1.Replace("\",", "");

                                //add to textbox
                                sales_order_RichTextBox.Text += sales_order_id.Trim() + " | ";

                                //ShipEngineUI.sales_order_id = sales_order_id;

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains("external_order_number") == true)
                            {

                                string external_order_number1 = currentLine.Replace("\"external_order_number\": \"", "");
                                string external_order_number = external_order_number1.Replace("\",", "");

                                //add to textbox
                                sales_order_RichTextBox.Text = sales_order_RichTextBox.Text.Trim() + external_order_number.Trim() + " - ";

                                //ShipEngineUI.external_order_number = external_order_number;
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains("fulfillment_status") == true)
                            {

                                string fulfillment_status1 = currentLine.Replace("\"fulfillment_status\": \"", "");
                                string fulfillment_status = fulfillment_status1.Replace("\",", "");

                                //add to textbox
                                sales_order_RichTextBox.Text += fulfillment_status.Trim() + Environment.NewLine + ",";

                                //ShipEngineUI.fulfillment_status = fulfillment_status;
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            //string line = ShipEngineUI.external_order_number.Trim() + " - " + ShipEngineUI.fulfillment_status.Trim() + " | " +
                               // ShipEngineUI.sales_order_id.Trim() + Environment.NewLine + ",";

                           // sales_order_RichTextBox.Text += line;

                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] sales_order_list1 = sales_order_RichTextBox.Text.Split(',');
                        string[] sales_order_list = sales_order_list1.Distinct().ToArray();
                        foreach (string sales_order in sales_order_list)
                        {
                            if (sales_order.Trim() == "")
                                continue;

                            sales_order_ListBox.Items.Add(sales_order.Trim());
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                sales_order_ListBox.Items.Add("ShipEngine found no sales orders to import at this time.");
                sales_order_ListBox.Enabled = false;
            }

        }

        private void carrier_id_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            service_code_RichTextBox.Clear();
            package_code_RichTextBox.Clear();
            service_code_ComboBox.Items.Clear();
            package_code_ComboBox.Items.Clear();
                    
            //GET CARRIER ID
            string carrier_id1 = carrier_id_ComboBox.SelectedItem.ToString();
            carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
            string carrier_id = carrier_id1.Replace("|", "");

            //GET CARRIER SERVICES
            try
            {
                //URL SOURCE
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/carriers/" + carrier_id + "/services";

                //REQUEST
                WebRequest requestObject = WebRequest.Create(ShipEngineUI.urlString);
                requestObject.Method = "GET";

                //ADD API KEY TO HEADER
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;


                //GET ALL WAREHOUSE ID's
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("service_code") == true)
                            {

                                string service_code1 = currentLine.Replace(" \"service_code\": \"", "");
                                string service_code = service_code1.Replace("\",", "");

                                //add to textbox
                                service_code_RichTextBox.Text = service_code_RichTextBox.Text.Trim() + "," + Environment.NewLine + service_code.Trim() + "|";

                            }
                            else if (currentLine.Contains("name") == true)
                            {
                                string carrier_code1 = currentLine.Replace(" \"name\": \"", "");
                                string carrier_code = carrier_code1.Replace("\",", "");

                                service_code_RichTextBox.Text = service_code_RichTextBox.Text + carrier_code.Trim();
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] service_code_list1 = service_code_RichTextBox.Text.Split(',');
                        string[] service_code_list = service_code_list1.Distinct().ToArray();
                        foreach (string service_code in service_code_list)
                        {
                            if (service_code.Trim() == "")
                                continue;

                            service_code_ComboBox.Items.Add(service_code.Trim());
                        }
                        
                        service_code_ComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch
            {

            }

            //GET CARRIER PACKAGES
            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/carriers/" + carrier_id + "/packages";

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET";

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;


                //Get all packages
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("package_code") == true)
                            {

                                string package_code1 = currentLine.Replace(" \"package_code\": \"", "");
                                string package_code = package_code1.Replace("\",", "");

                                //add to textbox
                                package_code_RichTextBox.Text = package_code_RichTextBox.Text.Trim() + "," + Environment.NewLine + package_code.Trim();

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] package_code_list1 = package_code_RichTextBox.Text.Split(',');
                        string[] package_code_list = package_code_list1.Distinct().ToArray();
                        foreach (string package_code in package_code_list)
                        {
                            if (package_code.Trim() == "")
                                continue;

                            package_code_ComboBox.Items.Add(package_code.Trim());
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                MessageBox.Show(HTTPexception.ToString());
            }

        }

        private void service_code_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void warehouse_id_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/warehouses/" + warehouse_id_ComboBox.SelectedItem.ToString();

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET";;

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;



                //Get Address
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    //
                    int originAddress1 = streamResponse.IndexOf(" \"origin_address\": ") + " \"origin_address\": ".Length;
                    int originAddress2 = streamResponse.LastIndexOf(" \"return_address\":");

                    string originAddress = streamResponse.Substring(originAddress1, originAddress2 - originAddress1);

                    using (var reader = new StringReader(originAddress))
                    {
                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            //NAME
                            if (currentLine.Contains(" \"name\": \"") == true)
                            {

                                //Replace "warehouse_id": " ",
                                string Wh_Name1 = currentLine.Replace("\"name\": \"", "");
                                string Wh_Name = Wh_Name1.Replace("\",", "");

                                //add to textbox

                                shipFrom_name_TextBox.Text = Wh_Name;

                            }

                            //PHONE
                            if (currentLine.Contains("\"phone\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_Phone1 = currentLine.Replace("\"phone\": \"", "");
                                string Wh_Phone = Wh_Phone1.Replace("\",", "");

                                //add to textbox

                                shipFrom_phone_TextBox.Text = Wh_Phone;
                            }

                            //Company
                            if (currentLine.Contains("\"company_name\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_CompanyName1 = currentLine.Replace("\"company_name\": \"", "");
                                string Wh_CompanyName = Wh_CompanyName1.Replace("\",", "");

                                //add to textbox

                                shipFrom_company_name_TextBox.Text = Wh_CompanyName;
                            }

                            //AddressLine 1
                            if (currentLine.Contains("\"address_line1\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_AddressL1 = currentLine.Replace("\"address_line1\": \"", "");
                                string Wh_AddressL = Wh_AddressL1.Replace("\",", "");

                                //add to textbox

                                shipFrom_address_line1_TextBox.Text = Wh_AddressL;
                            }

                            //AddressLine 2
                            if (currentLine.Contains("\"address_line2\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_AddressL2 = currentLine.Replace("\"address_line2\": \"", "");
                                string Wh_AddressL3 = Wh_AddressL2.Replace("\",", "");

                                //add to textbox

                                shipFrom_address_line2_TextBox.Text = Wh_AddressL3;
                            }


                            //AddressLine 3
                            if (currentLine.Contains("\"address_line3\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_AddressL4 = currentLine.Replace("\"address_line3\": \"", "");
                                string Wh_AddressL5 = Wh_AddressL4.Replace("\",", "");

                                //add to textbox

                                shipFrom_address_line3_TextBox.Text = Wh_AddressL5;
                            }

                            //City
                            if (currentLine.Contains("\"city_locality\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_City1 = currentLine.Replace("\"city_locality\": \"", "");
                                string Wh_City = Wh_City1.Replace("\",", "");

                                //add to textbox

                                shipFrom_city_locality_TextBox.Text = Wh_City;
                            }

                            //State Province
                            if (currentLine.Contains("\"state_province\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_StateProvince1 = currentLine.Replace("\"state_province\": \"", "");
                                string Wh_StateProvince = Wh_StateProvince1.Replace("\",", "");

                                //add to textbox

                                shipFrom_state_province_TextBox.Text = Wh_StateProvince;
                            }

                            //Postal Code
                            if (currentLine.Contains("\"postal_code\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",
                                string Wh_PostalCode1 = currentLine.Replace("\"postal_code\": \"", "");
                                string Wh_PostalCode = Wh_PostalCode1.Replace("\",", "");

                                //add to textbox

                                shipFrom_postal_code_TextBox.Text = Wh_PostalCode;
                            }

                            //Country Code
                            if (currentLine.Contains("\"country_code\": \"") == true)
                            {
                                //Replace "warehouse_id": " ",

                                string Wh_CountryCode1 = currentLine.Replace("\"country_code\": \"", "");
                                string Wh_CountryCode = Wh_CountryCode1.Replace("\",", "");


                                //add to textbox

                                shipFrom_country_code_TextBox.Text = Wh_CountryCode;
                                //shipFrmCountryTextBox.Text = shipFrmCountryTextBox.Text.Replace("    ","");

                            }

                            //remove spaces
                            //List Textboxes
                            IList<T> GetAllControls<T>(Control control) where T : Control
                            {
                                var TextBoxes = new List<T>();
                                foreach (Control item in control.Controls)
                                {
                                    var ctr = item as T;
                                    if (ctr != null)
                                        TextBoxes.Add(ctr);
                                    else
                                        TextBoxes.AddRange(GetAllControls<T>(item));
                                }
                                return TextBoxes;
                            }

                            //remove spaces loop
                            var textBoxesList = GetAllControls<System.Windows.Forms.TextBox>(this);
                            foreach (System.Windows.Forms.TextBox TextBoxes in textBoxesList)
                            {
                                TextBoxes.Text = TextBoxes.Text.Replace("    ", "");
                            }
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                MessageBox.Show(HTTPexception.ToString());
            }
        }

        private void ship_Date_DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime ship_Date = ship_Date_DateTimePicker.Value;
            ship_date_TextBox.Text = ship_Date.ToString();
        }

        private void get_Rates_Button_Click(object sender, EventArgs e)
        {
            rate_response_RichTextBox.Clear();

            try
            {
                string carrier_id1 = carrier_id_ComboBox.SelectedItem.ToString();
                carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
                string carrier_id = carrier_id1.Replace("|", "");

                string service_code1 = service_code_ComboBox.SelectedItem.ToString();
                service_code1 = service_code1.Remove(service_code1.IndexOf("|") + 1);
                string service_code = service_code1.Replace("|", "");

                Random logID = new Random();
                string rateLogId = logID.Next(0, 1000000).ToString("D6");

                //URI - POST
                WebRequest request = WebRequest.Create("https://api.shipengine.com/v1/rates");
                request.Method = "POST";

                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                string warehouse_id = warehouse_id_ComboBox.SelectedItem.ToString();
                string ship_date = ship_date_TextBox.Text;

                ShipEngineUI.shipTo_name = shipTo_name_TextBox.Text;
                ShipEngineUI.shipTo_phone = shipTo_phone_TextBox.Text;
                ShipEngineUI.shipTo_company_name = shipTo_company_name_TextBox.Text;
                ShipEngineUI.shipTo_address_line1 = shipTo_address_line1_TextBox.Text;
                ShipEngineUI.shipTo_address_line2 = shipTo_address_line2_TextBox.Text;
                ShipEngineUI.shipTo_address_line3 = shipTo_address_line3_TextBox.Text;
                ShipEngineUI.shipTo_city_locality = shipTo_city_locality_TextBox.Text;
                ShipEngineUI.shipTo_state_province = shipTo_state_province_TextBox.Text;
                ShipEngineUI.shipTo_postal_code = shipTo_postal_code_TextBox.Text;
                ShipEngineUI.shipTo_country_code = shipTo_country_code_TextBox.Text;
                ShipEngineUI.shipTo_address_residential_indicator = shipTo_address_residential_indicator_comboBox.SelectedItem.ToString();
                if (shipTo_address_residential_indicator_comboBox.SelectedItem == "")
                {
                    ShipEngineUI.shipTo_address_residential_indicator = "no";
                }

                ShipEngineUI.shipFrom_name = shipFrom_name_TextBox.Text;
                ShipEngineUI.shipFrom_phone = shipFrom_phone_TextBox.Text;
                ShipEngineUI.shipFrom_company_name = shipFrom_company_name_TextBox.Text;
                ShipEngineUI.shipFrom_address_line1 = shipFrom_address_line1_TextBox.Text;
                ShipEngineUI.shipFrom_address_line2 = shipFrom_address_line2_TextBox.Text;
                ShipEngineUI.shipFrom_address_line3 = shipFrom_address_line3_TextBox.Text;
                ShipEngineUI.shipFrom_city_locality = shipFrom_city_locality_TextBox.Text;
                ShipEngineUI.shipFrom_state_province = shipFrom_state_province_TextBox.Text;
                ShipEngineUI.shipFrom_postal_code = shipFrom_postal_code_TextBox.Text;
                ShipEngineUI.shipFrom_country_code = shipFrom_country_code_TextBox.Text;
                ShipEngineUI.shipFrom_address_residential_indicator = shipFrom_address_residential_indicator_comboBox.SelectedItem.ToString();
                if (shipFrom_address_residential_indicator_comboBox.SelectedItem == "")
                {
                    ShipEngineUI.shipFrom_address_residential_indicator = "no";
                }

                ShipEngineUI.packages_dimensions_length = packages_dimensions_length_numericUpDown.Value.ToString();
                ShipEngineUI.packages_dimensions_width = packages_dimensions_width_numericUpDown.Value.ToString();
                ShipEngineUI.packages_dimensions_height = packages_dimensions_height_numericUpDown.Value.ToString();

                ShipEngineUI.packages_weight_value = packages_weight_value_numericUpDown.Value.ToString();


                //RATE REQUEST
                #region  Rate Request
                //POST REQUEST
                string rateRequestBody = "{\r\n\"rate_options\": {\r\n  \"carrier_ids\": [\r\n    \"" + carrier_id + "\"\r\n  ]\r\n}," +
                    "\r\n         \"shipment\": " +
                    "{\r\n        \"validate_address\": \"no_validation\"" +
                    ",\r\n        \"carrier_id\": \"" + "" + "\"" +
                    ",\r\n        \"warehouse_id\": \"" + "" + "\"" +
                    ",\r\n        \"service_code\": \"" + service_code + "\"" +
                    ",\r\n        \"external_order_id\": null," +
                    "\r\n         \"ship_date\": \"" + ship_date + "\"" +
                    ",\r\n        \"is_return_label\": " + ShipEngineUI.is_return + "," +
                    "\r\n\r\n        \"items\": []," +
                    "\r\n\r\n        \"ship_to\": {\r\n            \"name\": \"" + ShipEngineUI.shipTo_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipTo_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipTo_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipTo_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipTo_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipTo_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipTo_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipTo_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipTo_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipTo_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipTo_address_residential_indicator + "\"\r\n        }," +
                    "\r\n\r\n        \"ship_from\": {\r\n            \"name\": \"" + ShipEngineUI.shipFrom_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipFrom_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipFrom_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipFrom_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipFrom_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipFrom_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipFrom_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipFrom_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipFrom_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipFrom_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipFrom_address_residential_indicator + "\"\r\n        }," +
                    "\r\n\r\n        \"confirmation\": \"" + delivery_confirmation_CheckBox.SelectedItem.ToString() + 
                    "\",\r\n\r\n        \"advanced_options\": {" +
                    "\r\n                \"bill_to_account\": null," +
                    "\r\n                \"bill_to_country_code\": null," +
                    "\r\n                \"bill_to_party\": null," +
                    "\r\n                \"bill_to_postal_code\": null," +
                    "\r\n                \"canada_delivered_duty\": null," +
                    "\r\n                \"contains_alcohol\": \"false\"," +
                    "\r\n                \"delivered_duty_paid\": \"false\"," +
                    "\r\n                \"non_machinable\": \"false\"," +
                    "\r\n                \"saturday_delivery\": \"false\"," +
                    "\r\n                \"third-party-consignee\": \"false\"," +
                    "\r\n                \"ancillary_endorsements_option\": null," +
                    "\r\n                \"freight_class\": null," +
                    "\r\n                \"custom_field_1\": null," +
                    "\r\n                \"custom_field_2\": null," +
                    "\r\n                \"custom_field_3\": null," +
                    "\r\n                \"return_pickup_attempts\": null," +
                    "\r\n\r\n            \"dry_ice\": \"false\"," +
                    "\r\n                \"dry_ice_weight\": {" +
                    "\r\n                \"value\": \"0.00\"," +
                    "\r\n                \"unit\": \"pound\"\r\n            }," +
                    "\r\n\r\n            \"collect_on_delivery\": {" +
                    "\r\n                \"payment_type\": \"none\"," +
                    "\r\n                \"payment_amount\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": \"0.00\"\r\n                }\r\n            }\r\n        }," +
                    "\r\n\r\n        \"origin_type\": null," +
                    "\r\n        \"insurance_provider\": \"none\"," +
                    "\r\n        \"packages\": [\r\n            {" +
                    "\r\n                \"package_code\": \"" + package_code_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n                \"weight\": {\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": " + "\"pound\"" + "\r\n                }," +
                    "\r\n                \"dimensions\": {\r\n                    \"unit\": \"" + "inch" + "\"," +
                    "\r\n                    \"length\": " + ShipEngineUI.packages_dimensions_length + "," +
                    "\r\n                    \"width\": " + ShipEngineUI.packages_dimensions_width + "," +
                    "\r\n                    \"height\": " + ShipEngineUI.packages_dimensions_height + "\r\n                }," +
                    "\r\n                \"insured_value\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": 0.00\r\n                }," +
                    "\r\n                \"label_messages\": {" +
                    "\r\n                    \"reference1\": " + rateLogId + "," +
                    "\r\n                    \"reference2\": null," +
                    "\r\n                    \"reference3\": null" +
                    "\r\n                },\r\n                \"external_package_id\": 0" +
                    "\r\n            }\r\n        ]" +
                    "\r\n    }" +
                    "\r\n}";

                #endregion

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(rateRequestBody);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Close();


                //Documents path REQUEST LOG
                string docPath = @"..\..\Resources\Logs";
                File.WriteAllText(Path.Combine(docPath, "RateRequest - " + rateLogId + ".txt"), rateRequestBody);


                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                rate_response_RichTextBox.Text = parseResponse.ReadToEnd();

                string responseBodyText = rate_response_RichTextBox.Text;
                File.WriteAllText(Path.Combine(docPath, "RateResponse - " + rateLogId + ".txt"), responseBodyText);

                int getRates1 = responseBodyText.IndexOf("\"shipping_amount\"") + "\"shipping_amount\"".Length;
                int getRates2 = responseBodyText.LastIndexOf("\"insurance_amount\"");
                stream.Close();

                string getRates3 = responseBodyText.Substring(getRates1, getRates2 - getRates1);


                using (var reader = new StringReader(responseBodyText))
                {

                    string ratesResponse = "";

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        //NAME
                        if (currentLine.Contains("\"amount\"") == true)
                        {
                            //string carrier_code1 = currentLine.Replace("\"carrier_code\": \"", "");
                            //string carrier_code = carrier_code1.Replace("\",", "");
                            ratesResponse += currentLine;

                        }
                        else
                        {
                            currentLine.Replace(currentLine, "");
                        }

                        if (currentLine.Contains("\"package_type\"") == true)
                        {

                            ratesResponse += Environment.NewLine + currentLine + Environment.NewLine;

                        }
                        else
                        {
                            currentLine.Replace(currentLine, "");
                        }

                        if (currentLine.Contains("\"service_code\"") == true)
                        {

                            ratesResponse += currentLine + Environment.NewLine + Environment.NewLine;

                        }
                        else
                        {
                            currentLine.Replace(currentLine, "");
                        }

                    }

                    rate_response_RichTextBox.Text = ratesResponse;
                    rate_response_RichTextBox.Text = rate_response_RichTextBox.Text.Replace("\"amount\": 0.0", "");
                    rate_response_RichTextBox.Text = rate_response_RichTextBox.Text.Replace(" ", "");
                    rate_response_RichTextBox.Text = rate_response_RichTextBox.Text.Replace("\"", "");
                }

                stream.Close();

            }
            catch (Exception HTTPexception)
            {
                if (HTTPexception.Message.Contains("400"))
                {
                    
                    MessageBox.Show("ShipEngine returned a 400 Bad Request, please check your entries and retry.");

                }
                else
                {
                    MessageBox.Show("Unspecified Error, please try again and check your entries");
                }
            }
        }

        private void create_label_Button_Click(object sender, EventArgs e)
        {

            label_RichTextBox.Clear();
            void_label_id_TextBox.Text = string.Empty;

            try
            {
                string carrier_id1 = carrier_id_ComboBox.SelectedItem.ToString();
                carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
                string carrier_id = carrier_id1.Replace("|", "");

                string service_code1 = service_code_ComboBox.SelectedItem.ToString();
                service_code1 = service_code1.Remove(service_code1.IndexOf("|") + 1);
                string service_code = service_code1.Replace("|", "");

                //LOGID
                Random logID = new Random();
                string rateLogId = logID.Next(0, 1000000).ToString("D6");

                //SHIP DATE
                string ship_date = ship_date_TextBox.Text;


                //URI - POST
                WebRequest request = WebRequest.Create("https://api.shipengine.com/v1/labels");
                request.Method = "POST";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);


                //POST REQUEST
                string createLabelrequestBody = "{\r\n\"rate_options\": {\r\n  \"carrier_ids\": [\r\n    \"" + carrier_id + "\"\r\n  ]\r\n}," +
                    "\r\n         \"shipment\": " +
                    "{\r\n        \"validate_address\": \"no_validation\"" +
                    ",\r\n        \"carrier_id\": \"" + "" + "\"" +
                    ",\r\n        \"warehouse_id\": \"" + "" + "\"" +
                    ",\r\n        \"service_code\": \"" + service_code + "\"" +
                    ",\r\n        \"external_order_id\": null," +
                    "\r\n         \"ship_date\": \"" + ship_date + "\"" +
                    ",\r\n        \"is_return_label\": " + ShipEngineUI.is_return + "," +
                    "\r\n\r\n        \"items\": []," +
                    "\r\n\r\n        \"ship_to\": {\r\n            \"name\": \"" + ShipEngineUI.shipTo_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipTo_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipTo_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipTo_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipTo_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipTo_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipTo_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipTo_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipTo_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipTo_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipTo_address_residential_indicator + "\"\r\n        }," +
                    "\r\n\r\n        \"ship_from\": {\r\n            \"name\": \"" + ShipEngineUI.shipFrom_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipFrom_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipFrom_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipFrom_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipFrom_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipFrom_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipFrom_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipFrom_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipFrom_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipFrom_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipFrom_address_residential_indicator + "\"\r\n        }," +
                    "\r\n\r\n        \"confirmation\": \"" + delivery_confirmation_CheckBox.SelectedItem.ToString() + "\",\r\n\r\n        \"advanced_options\": {" +
                    "\r\n            \"bill_to_account\": null," +
                    "\r\n            \"bill_to_country_code\": null," +
                    "\r\n            \"bill_to_party\": null," +
                    "\r\n            \"bill_to_postal_code\": null," +
                    "\r\n            \"canada_delivered_duty\": null," +
                    "\r\n            \"contains_alcohol\": \"false\"," +
                    "\r\n            \"delivered_duty_paid\": \"false\"," +
                    "\r\n            \"non_machinable\": \"false\"," +
                    "\r\n            \"saturday_delivery\": \"false\"," +
                    "\r\n            \"third-party-consignee\": \"false\"," +
                    "\r\n            \"ancillary_endorsements_option\": null," +
                    "\r\n            \"freight_class\": null," +
                    "\r\n            \"custom_field_1\": null," +
                    "\r\n            \"custom_field_2\": null," +
                    "\r\n            \"custom_field_3\": null," +
                    "\r\n            \"return_pickup_attempts\": null," +
                    "\r\n\r\n        \"dry_ice\": \"false\"," +
                    "\r\n            \"dry_ice_weight\": {" +
                    "\r\n                \"value\": \"0.00\"," +
                    "\r\n                \"unit\": \"pound\"\r\n            }," +
                    "\r\n\r\n            \"collect_on_delivery\": {" +
                    "\r\n                \"payment_type\": \"none\"," +
                    "\r\n                \"payment_amount\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": \"0.00\"\r\n                }\r\n            }\r\n        }," +
                    "\r\n\r\n        \"origin_type\": null," +
                    "\r\n        \"insurance_provider\": \"none\"," +
                    "\r\n        \"packages\": [\r\n            {" +
                    "\r\n                \"package_code\": \"" + package_code_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n                \"weight\": {\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": " + "\"pound\"" + "\r\n                }," +
                    "\r\n                \"dimensions\": {\r\n                    \"unit\": \"" + "inch" + "\"," +
                    "\r\n                    \"length\": " + ShipEngineUI.packages_dimensions_length + "," +
                    "\r\n                    \"width\": " + ShipEngineUI.packages_dimensions_width + "," +
                    "\r\n                    \"height\": " + ShipEngineUI.packages_dimensions_height + "\r\n                }," +
                    "\r\n                \"insured_value\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": 0.00\r\n                }," +
                    "\r\n                \"label_messages\": {" +
                    "\r\n                    \"reference1\": null," +
                    "\r\n                    \"reference2\": null," +
                    "\r\n                    \"reference3\": null" +
                    "\r\n                },\r\n                \"external_package_id\": " + rateLogId +
                    "\r\n            }\r\n        ]" +
                    "\r\n    }" +
                    "\r\n}";

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(createLabelrequestBody);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                //Documents path REQUEST LOG
                string docPath = @"..\..\Resources\Logs";
                File.WriteAllText(Path.Combine(docPath, "LabelRequest - " + rateLogId + ".txt"), createLabelrequestBody);

                stream.Write(data, 0, data.Length);
                stream.Close();

                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                label_RichTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = label_RichTextBox.Text;


                    using (var reader = new StringReader(responseBodyText))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("label_id") == true)
                            {

                                string label_id1 = currentLine.Replace("\"label_id\": \"", "");
                                string label_id = label_id1.Replace("\",", "");

                                //DECLARE VARIABLE
                                ShipEngineUI.label_id = label_id.Trim();

                            }
                        }  
                    }
                

                // GET LABEL IMAGE
                //LABEL_DOWNLOAD OBJECT
                int labelDownloadOBJ1 = responseBodyText.IndexOf("\"label_download\"") + "\"label_download\"".Length;
                int labelDownloadOBJ2 = responseBodyText.LastIndexOf("\"form_download\"");
                stream.Close();

                string labelDownloadOBJ3 = responseBodyText.Substring(labelDownloadOBJ1, labelDownloadOBJ2 - labelDownloadOBJ1);
                //Needed to specify as UPS contains two Label download objects

                int imgURL1 = labelDownloadOBJ3.IndexOf("\"png\": \"") + "\"png\": \"".Length;
                int imgURL2 = labelDownloadOBJ3.LastIndexOf(".png");
                stream.Close();

                string imgURL3 = labelDownloadOBJ3.Substring(imgURL1, imgURL2 - imgURL1);

                //Save image in logging
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(imgURL3 + ".png"), @"..\..\Resources\LabelImages\Label-" + rateLogId + ".png");
                }

                labelImageBox.Load(imgURL3 + ".png");

                void_label_id_TextBox.Text = ShipEngineUI.label_id;

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (Exception crateLabelError)
            {

                if (crateLabelError.Message.Contains("400"))
                {

                    MessageBox.Show("ShipEngine returned a 400 Bad Request, please check your entries and retry.");

                }
                else
                {
                    MessageBox.Show("Unspecified Error, please try again and check your entries");
                }

            }

        }

        void LabelImage(object o, PrintPageEventArgs e)
        {

            Image image = this.labelImageBox.Image;

            Point point = new Point(100, 100);

            e.Graphics.DrawImage(image, point);

        }
        private void print_Button_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            printDialog.PrinterSettings = new PrinterSettings();

            if (DialogResult.OK == printDialog.ShowDialog(this))
            {

                PrintDocument pdoc = new PrintDocument();

                pdoc.PrintPage += new PrintPageEventHandler(LabelImage);

                pdoc.Print();

            }
        }

        private void sales_order_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            create_label_from_Order_Button.Enabled = true;

            try
            {
                //GET SALES ORDER ID
                string sales_order_id1 = sales_order_ListBox.SelectedItem.ToString();
                sales_order_id1 = sales_order_id1.Remove(sales_order_id1.IndexOf("|") + 1);
                string sales_order_id = sales_order_id1.Replace("|", "");
                
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v-beta/sales_orders/" + sales_order_id;

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET"; ;

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;

                //Get Address
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    //
                    int originAddress1 = streamResponse.IndexOf("\"ship_to\": {") + "\"ship_to\": {".Length;
                    int originAddress2 = streamResponse.LastIndexOf("\"sales_order_items\": ");

                    string originAddress = streamResponse.Substring(originAddress1, originAddress2 - originAddress1);

                    using (var reader = new StringReader(originAddress))
                    {
                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            //NAME
                            if (currentLine.Contains(" \"name\": \"") == true)
                            {

                                
                                string sales_order_Name1 = currentLine.Replace("\"name\": \"", "");
                                string sales_order_Name = sales_order_Name1.Replace("\",", "");

                                //add to textbox

                                shipTo_name_TextBox.Text = sales_order_Name;

                            }

                            //PHONE
                            if (currentLine.Contains("\"phone\": \"") == true)
                            {
                                
                                string sales_order_Phone1 = currentLine.Replace("\"phone\": \"", "");
                                string sales_order_Phone = sales_order_Phone1.Replace("\",", "");

                                //add to textbox

                                shipTo_phone_TextBox.Text = sales_order_Phone;
                            }

                            //Company
                            if (currentLine.Contains("\"company_name\": \"") == true)
                            {
                                
                                string sales_order_CompanyName1 = currentLine.Replace("\"company_name\": \"", "");
                                string sales_order_CompanyName = sales_order_CompanyName1.Replace("\",", "");

                                //add to textbox

                                shipTo_company_name_TextBox.Text = sales_order_CompanyName;
                            }

                            //AddressLine 1
                            if (currentLine.Contains("\"address_line1\": \"") == true)
                            {
                                
                                string sales_order_AddressL1 = currentLine.Replace("\"address_line1\": \"", "");
                                string sales_order_AddressL = sales_order_AddressL1.Replace("\",", "");

                                //add to textbox

                                shipTo_address_line1_TextBox.Text = sales_order_AddressL;
                            }

                            //AddressLine 2
                            if (currentLine.Contains("\"address_line2\": \"") == true)
                            {
                                
                                string sales_order_AddressL2 = currentLine.Replace("\"address_line2\": \"", "");
                                string sales_order_AddressL3 = sales_order_AddressL2.Replace("\",", "");

                                //add to textbox

                                shipTo_address_line2_TextBox.Text = sales_order_AddressL3;
                            }


                            //AddressLine 3
                            if (currentLine.Contains("\"address_line3\": \"") == true)
                            {
                                
                                string sales_order_AddressL4 = currentLine.Replace("\"address_line3\": \"", "");
                                string sales_order_AddressL5 = sales_order_AddressL4.Replace("\",", "");

                                //add to textbox

                                shipTo_address_line3_TextBox.Text = sales_order_AddressL5;
                            }

                            //City
                            if (currentLine.Contains("\"city_locality\": \"") == true)
                            {
                                
                                string sales_order_City1 = currentLine.Replace("\"city_locality\": \"", "");
                                string sales_order_City = sales_order_City1.Replace("\",", "");

                                //add to textbox

                                shipTo_city_locality_TextBox.Text = sales_order_City;
                            }

                            //State Province
                            if (currentLine.Contains("\"state_province\": \"") == true)
                            {
                                
                                string sales_order_StateProvince1 = currentLine.Replace("\"state_province\": \"", "");
                                string sales_order_StateProvince = sales_order_StateProvince1.Replace("\",", "");

                                //add to textbox

                                shipTo_state_province_TextBox.Text = sales_order_StateProvince;
                            }

                            //Postal Code
                            if (currentLine.Contains("\"postal_code\": \"") == true)
                            {
                                
                                string sales_order_PostalCode1 = currentLine.Replace("\"postal_code\": \"", "");
                                string sales_order_PostalCode = sales_order_PostalCode1.Replace("\",", "");

                                //add to textbox

                                shipTo_postal_code_TextBox.Text = sales_order_PostalCode;
                            }

                            //Country Code
                            if (currentLine.Contains("\"country_code\": \"") == true)
                            {
                                
                                string sales_order_CountryCode1 = currentLine.Replace("\"country_code\": \"", "");
                                string sales_order_CountryCode = sales_order_CountryCode1.Replace("\",", "");

                                //add to textbox

                                shipTo_country_code_TextBox.Text = sales_order_CountryCode;

                            }

                            //remove spaces
                            //List Textboxes
                            IList<T> GetAllControls<T>(Control control) where T : Control
                            {
                                var TextBoxes = new List<T>();
                                foreach (Control item in control.Controls)
                                {
                                    var ctr = item as T;
                                    if (ctr != null)
                                        TextBoxes.Add(ctr);
                                    else
                                        TextBoxes.AddRange(GetAllControls<T>(item));
                                }
                                return TextBoxes;
                            }

                            //remove spaces loop
                            var textBoxesList = GetAllControls<System.Windows.Forms.TextBox>(this);
                            foreach (System.Windows.Forms.TextBox TextBoxes in textBoxesList)
                            {
                                TextBoxes.Text = TextBoxes.Text.Replace("    ", "");
                            }
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                MessageBox.Show(HTTPexception.ToString());
            }

        }

        private void create_label_from_Order_Button_Click(object sender, EventArgs e)
        {

            sales_order_RichTextBox.Clear();

            try
            {
                //PARSE CARRIER ID Get variable
                string carrier_id1 = carrier_id_ComboBox.SelectedItem.ToString();
                carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
                string carrier_id = carrier_id1.Replace("|", "");

                //PARSE SERVICE CODE Get variable
                string service_code1 = service_code_ComboBox.SelectedItem.ToString();
                service_code1 = service_code1.Remove(service_code1.IndexOf("|") + 1);
                string service_code = service_code1.Replace("|", "");

                //PARSE SALES ORDER ID Get variable
                string sales_order_id1 = sales_order_ListBox.SelectedItem.ToString();
                sales_order_id1 = sales_order_id1.Remove(sales_order_id1.IndexOf("|") + 1);
                string sales_order_id = sales_order_id1.Replace("|", "");

                //LOGID
                Random logID = new Random();
                string rateLogId = logID.Next(0, 1000000).ToString("D6");

                //SHIP DATE
                string ship_date = ship_date_TextBox.Text;


                //URI - POST
                WebRequest request = WebRequest.Create("https://api.shipengine.com/v-beta/labels/sales_order/" + sales_order_id);
                request.Method = "POST";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);


                //POST REQUEST
                string sales_order_LabelrequestBody =
                    "{\r\n    \"label_format\": \"png\"," +
                    "\r\n    \"shipment\": {" +
                    "\r\n        \"carrier_id\": " + carrier_id + "," +
                    "\r\n        \"service_code\": " + service_code + "," +
                    "\r\n        \"ship_from\": {" +
                    "\r\n            \"company_name\": " + ShipEngineUI.shipFrom_company_name + "," +
                    "\r\n            \"name\": " + ShipEngineUI.shipFrom_name + "," +
                    "\r\n            \"phone\": " + ShipEngineUI.shipFrom_phone + "," +
                    "\r\n            \"address_line1\": " + ShipEngineUI.shipFrom_address_line1 + "," +
                    "\r\n            \"address_line2\": " + ShipEngineUI.shipFrom_address_line2 + "," +
                    "\r\n            \"city_locality\": " + ShipEngineUI.shipFrom_city_locality + "," +
                    "\r\n            \"state_province\": " + ShipEngineUI.shipFrom_state_province + "," +
                    "\r\n            \"postal_code\": " + ShipEngineUI.shipFrom_postal_code + "," +
                    "\r\n            \"country_code\": " + ShipEngineUI.shipFrom_country_code + "," +
                    "\r\n            \"address_residential_indicator\": " + ShipEngineUI.shipFrom_address_residential_indicator + "" +
                    "\r\n        }," +
                    "\r\n        \"packages\": [" +
                    "\r\n            {" +
                    "\r\n                \"package_code\": " + package_code_ComboBox.SelectedItem.ToString() + "," +
                    "\r\n                \"weight\": {" +
                    "\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": \"pound\"" +
                    "\r\n                }" +
                    "\r\n            }" +
                    "\r\n        ]" +
                    "\r\n    }" +
                    "\r\n}";

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(sales_order_LabelrequestBody);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                //Documents path REQUEST LOG
                string docPath = @"..\..\Resources\Logs";
                File.WriteAllText(Path.Combine(docPath, "LabelRequest - " + rateLogId + ".txt"), sales_order_LabelrequestBody);

                stream.Write(data, 0, data.Length);
                stream.Close();

                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                sales_order_RichTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = sales_order_RichTextBox.Text;

                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("label_id") == true)
                        {

                            string label_id1 = currentLine.Replace("\"label_id\": \"", "");
                            string label_id = label_id1.Replace("\",", "");

                            //DECLARE VARIABLE
                            ShipEngineUI.label_id = label_id.Trim();

                        }
                    }
                }


                // GET LABEL IMAGE
                //LABEL_DOWNLOAD OBJECT
                int labelDownloadOBJ1 = responseBodyText.IndexOf("\"label_download\"") + "\"label_download\"".Length;
                int labelDownloadOBJ2 = responseBodyText.LastIndexOf("\"form_download\"");
                stream.Close();

                string labelDownloadOBJ3 = responseBodyText.Substring(labelDownloadOBJ1, labelDownloadOBJ2 - labelDownloadOBJ1);
                //Needed to specify as UPS contains two Label download objects

                int imgURL1 = labelDownloadOBJ3.IndexOf("\"png\": \"") + "\"png\": \"".Length;
                int imgURL2 = labelDownloadOBJ3.LastIndexOf(".png");
                stream.Close();

                string imgURL3 = labelDownloadOBJ3.Substring(imgURL1, imgURL2 - imgURL1);

                //Save image in logging
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(imgURL3 + ".png"), @"..\..\Resources\LabelImages\Label-" + rateLogId + ".png");
                }

                labelImageBox.Load(imgURL3 + ".png");

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (Exception crateLabelError)
            {

                if (crateLabelError.Message.Contains("400"))
                {

                    MessageBox.Show("ShipEngine returned a 400 Bad Request, please check your entries and retry.");

                }
                else
                {
                    MessageBox.Show("Unspecified Error, please try again and check your entries");
                }

            }


        }

        private void labelImageBox_Click(object sender, EventArgs e)
        {
            labelImageBox.Visible = false;

            label_RichTextBox.Visible = true;


        }

        private void label_RichTextBox_Click(object sender, EventArgs e)
        {

            labelImageBox.Visible = true;

            label_RichTextBox.Visible = false;
        }

        private void void_label_id_Button_Click(object sender, EventArgs e)
        {

            string label_id_entered = void_label_id_TextBox.Text;

            try
            {

                if (label_id_entered == ShipEngineUI.label_id)
                {

                    DialogResult dialogResult = MessageBox.Show("Are you sure you would like to void " + ShipEngineUI.label_id + "?", "VOID LABEL", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {

                            //URI - POST
                            WebRequest request = WebRequest.Create("https://api.shipengine.com/v1/labels/" + ShipEngineUI.label_id + "/void");
                            request.Method = "POST";

                            //API Key
                            request.Headers.Add("API-key", ShipEngineUI.apiKey);

                            Stream stream = request.GetRequestStream();

                            stream.Close();

                            WebResponse requestResponse = request.GetResponse();
                            stream = requestResponse.GetResponseStream();

                            StreamReader parseResponse = new StreamReader(stream);
                            void_label_id_RichTextBox.Text = parseResponse.ReadToEnd();
                            string responseBodyText = void_label_id_RichTextBox.Text;

                            //RESPONSE
                            HttpWebResponse responseObjectGet = null;
                            responseObjectGet = (HttpWebResponse)request.GetResponse();
                            string streamResponse = null;

                            //Get variables to declare globally
                            using (Stream labelStream = responseObjectGet.GetResponseStream())
                            {
                                StreamReader responseRead = new StreamReader(stream);
                                streamResponse = responseRead.ReadToEnd();

                                using (var reader = new StringReader(streamResponse))
                                {

                                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                                    {

                                        if (currentLine.Contains("message") == true)
                                        {

                                            string void_label_id_Response1 = currentLine.Replace("\"message\": \"", "");
                                            string void_label_id_Response = void_label_id_Response1.Replace("\",", "");

                                            //DECLARE VARIABLE
                                            ShipEngineUI.void_label_id_Response = void_label_id_Response;

                                        }
                                    }
                                }
                            }

                            MessageBox.Show(ShipEngineUI.void_label_id_Response);
                            
                            //CLOSE STREAM
                            parseResponse.Close();
                            stream.Close();

                        }
                        catch(Exception void_label_id_response_Error)
                        {

                            MessageBox.Show(void_label_id_response_Error + Environment.NewLine + "This label could not be voided.");

                        }

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        
                        void_label_id_TextBox.Text = string.Empty;
                        MessageBox.Show("You canceled the void request.");

                    }

                }
                else if (label_id_entered != ShipEngineUI.label_id)
                {

                    MessageBox.Show("The Label ID you entered does not match the label you created. Please check your entry.");

                }

            }
            catch(Exception voidlableexception)
            {
                MessageBox.Show(voidlableexception.ToString());
            }

        }
    }
}
