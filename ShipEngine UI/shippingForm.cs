using ShipEngine_UI.Resources.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace ShipEngine_UI
{
    public partial class ShippingForm : Form
    {

        public ShippingForm()
        {
            InitializeComponent();
        }

        private void shippingForm_Load(object sender, EventArgs e)
        {

            ship_Date_DateTimePicker.MinDate = DateTime.Today;
            ship_Date_DateTimePicker.Format = DateTimePickerFormat.Custom;
            ship_Date_DateTimePicker.CustomFormat = "dd-MM-yyyy";

            bill_to_party_comboBox.SelectedIndex = 0;
            contains_alcohol_comboBox.SelectedIndex = 0;
            delivered_duty_paid_comboBox.SelectedIndex = 0;
            delivery_confirmation_ComboBox.SelectedIndex = 0;
            dry_ice_comboBox.SelectedIndex = 0;
            dry_ice_weight_comboBox.SelectedIndex = 0;
            insurance_provider_comboBox.SelectedIndex = 0;
            insured_value_currency_comboBox.SelectedIndex = 0;
            non_machinable_comboBox.SelectedIndex = 0;
            origin_type_comboBox.SelectedIndex = 0;
            saturday_delivery_comboBox.SelectedIndex = 0;
            third_party_consignee_comboBox.SelectedIndex = 0;
            shipFrom_address_residential_indicator_comboBox.SelectedIndex = 1;
            shipTo_address_residential_indicator_comboBox.SelectedIndex = 1;
            carrier_code_comboBox.SelectedIndex = 0;
            connect_carrier_ComboBox.SelectedIndex = 0;
            connect_order_source_ComboBox.SelectedIndex= 0;
            rate_request_RichTextBox.Enabled = true;

            ship_date_TextBox.Text = DateTime.Today.ToString();

            //GET CARRIER ACCOUNTS
            GetCarrierAccounts();

            //GET WAREHOUSES
            GetWarehouses();

            //GET SALES ORDERS
            GetSalesOrders();

            //GET LABEL HISTORY
            GetLabelHistory();

            //GET ONE BALANCE
            //GetCarrierBalance();

            //GET ORDER SOURCES
            if (ShipEngineUI.apiKey.Contains("TEST"))
            {
                //ShipEngine Sandbox does not initialize properly when ordersources are queried. Added Check to allow sandbox use in the application.
            }
            else
            {
                GetOrderSources();
            }
            
        }

        private void shippingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.OpenForms["apiKeyPrompt"].Close();

            for (int i = System.Windows.Forms.Application.OpenForms.Count - 1; i >= 0; i--)
            {
                System.Windows.Forms.Application.OpenForms[i].Close();
            }

        }

        #region Form Load methods

        public void GetOrderSources()
        {
            //GET ORDER SOURCES
            try
            {
                //URL SOURCE
                ShipEngineUI.urlString = "https://api.shipengine.com/v-beta/order_sources";

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

                            if (currentLine.Contains("order_source_id") == true)
                            {

                                string order_source_id1 = currentLine.Replace(" \"order_source_id\": \"", "");
                                string order_source_id = order_source_id1.Replace("\",", "");

                                //add to textbox
                                order_source_richTextBox.Text = order_source_richTextBox.Text.Trim() + "," + Environment.NewLine + order_source_id.Trim() + "| ";

                            }
                            else if (currentLine.Contains("order_source_nickname") == true)
                            {
                                string order_source_nickname1 = currentLine.Replace("\"order_source_nickname\": \"", "");
                                string order_source_nickname = order_source_nickname1.Replace("\",", "");

                                order_source_richTextBox.Text = order_source_richTextBox.Text + order_source_nickname.Trim() + " | ";
                            }
                            else if (currentLine.Contains("order_source_code") == true)
                            {
                                string order_source_code1 = currentLine.Replace("\"order_source_code\": \"", "");
                                string order_source_code = order_source_code1.Replace("\",", "");

                                order_source_richTextBox.Text = order_source_richTextBox.Text + order_source_code.Trim();
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD OrderSources
                        string[] order_source_id_list1 = order_source_richTextBox.Text.Split(',');
                        string[] order_source_id_list = order_source_id_list1.Distinct().ToArray();
                        foreach (string order_source in order_source_id_list)
                        {
                            if (order_source.Trim() == "")
                                continue;

                            //_ComboBox.Items.Add(carrier_id.Trim());
                            manage_order_sources_listBox.Items.Add(order_source.Trim());
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                this.Close();

                if (HTTPexception.Message.Contains("401"))
                {
                    
                    //MessageBox.Show("ShipEngine returned 401 Unauthorized, please check your API Key.", "AUTHENTICATION ERROR");

                    ShipEngineUI.has_error = true;

                }

            }
        }

        public void GetCarrierAccounts()
        {
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
                            update_carrier_listBox.Items.Add(carrier_id.Trim());
                            
                        }

                        //PARSE AND ADD CARRIER FOR ADD FUNDS, USPS, STAMPS, ENDICIA
                        string[] carrier_id_list2 = carrier_id_RichTextBox.Text.Split(',');
                        string[] carrier_id_list3 = carrier_id_list2.Distinct().ToArray();
                        foreach (string carrier_id_add_funds in carrier_id_list3)
                        {
                            if (carrier_id_add_funds.Trim() == "")
                                continue;

                            if (carrier_id_add_funds.Contains("usps") || carrier_id_add_funds.Contains("stamps") || carrier_id_add_funds.Contains("endicia"))
                            {
                                add_funds_account_ComboBox.Items.Add(carrier_id_add_funds.Trim());
                            }
                            else
                            {
                                add_funds_account_ComboBox.Items.Remove(carrier_id_add_funds.Trim());
                            }

                            
                        }
                    }

                    if (add_funds_account_ComboBox.Items.Count == 0)
                    {
                        add_funds_account_ComboBox.Enabled = false;
                    }
                    else
                    {
                        add_funds_account_ComboBox.SelectedIndex = 0;
                        add_funds_account_ComboBox.Enabled = true;
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                this.Close();

                if (HTTPexception.Message.Contains("401"))
                {
                    apiKeyPrompt ApiKeyForm = new apiKeyPrompt();
                    MessageBox.Show("ShipEngine returned 401 Unauthorized, please check your API Key.", "AUTHENTICATION ERROR");

                    ShipEngineUI.has_error = true;

                    ApiKeyForm.ShowDialog();
                }

            }
        }

        public void GetWarehouses()
        {
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
                                warehouse_id_RichTextBox.Text = warehouse_id_RichTextBox.Text.Trim() + "~" + Environment.NewLine + warehouse_id.Trim() + "|";

                            }
                            else if (currentLine.Contains("name") == true)
                            {
                                string warehouse_name1 = currentLine.Replace("\"name\": \"", "");
                                string warehouse_name = warehouse_name1.Replace("\",", "");

                                warehouse_id_RichTextBox.Text = warehouse_id_RichTextBox.Text + warehouse_name.Trim() + "]";
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }

                        //PARSE AND ADD CARRIER ID's
                        string[] warehouse_id_list1 = warehouse_id_RichTextBox.Text.Split('~');
                        string[] warehouse_id_list = warehouse_id_list1.Distinct().ToArray();
                        foreach (string warehouse_id in warehouse_id_list)
                        {
                            if (warehouse_id.Trim() == "")
                                continue;

                            string fix = warehouse_id.Substring(0, warehouse_id.IndexOf("]"));
                            warehouse_id_ComboBox.Items.Add(fix.Trim());

                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
                MessageBox.Show(HTTPexception.ToString());
            }
        }

        public void GetSalesOrders()
        {
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

                        //PARSE AND ADD SALES ORDERS
                        string[] sales_order_list1 = sales_order_RichTextBox.Text.Split(',');
                        string[] sales_order_list = sales_order_list1.Distinct().ToArray();
                        foreach (string sales_order in sales_order_list)
                        {
                            if (sales_order.Trim() == "")
                                continue;

                            sales_order_ListBox.Items.Add(sales_order.Trim());
                        }

                        if (sales_order_ListBox.Items.Count == 0)
                        {
                            sales_order_ListBox.Items.Add("ShipEngine found no sales orders to import at this time.");
                            sales_order_ListBox.Enabled = false;
                            notify_shipped_checkBox.Enabled = false;
                            create_label_from_Order_Button.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {
               
            }
        }

        public void GetLabelHistory()
        {
            label_id_richTextBox.Text = string.Empty;

            //GET LABELS
            try
            {
                string todaysDate = DateTime.Today.ToString();
                string last7Days = DateTime.Today.AddDays(-7).ToString();

                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/labels?created_at_start=" + last7Days + "&created_at_end" + todaysDate + "&page_size=100";
                ShipEngineUI.get_label_URL = URLstring;

                //REQUEST
                WebRequest requestObject = WebRequest.Create(ShipEngineUI.get_label_URL);
                requestObject.Method = "GET";

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;

                //Get List labels data
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("label_id") == true)
                            {

                                string label_id1 = currentLine.Replace("\"label_id\": \"", "");
                                string label_id = label_id1.Replace("\",", "");

                                //add to textbox
                                label_id_richTextBox.Text += label_id.Trim() + " | ";
                                label_id = ShipEngineUI.label_url_label_id.Trim();

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains("\"status\"") == true)
                            {

                                string voided1 = currentLine.Replace("\"", "");
                                string voided = voided1.Replace(",", "");

                                //add to textbox
                                label_id_richTextBox.Text = label_id_richTextBox.Text.Trim() + voided.Trim() + " > ";

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains("\"carrier_code\"") == true)
                            {

                                string carrier_code1 = currentLine.Replace("\"", "");
                                string carrier_code = carrier_code1.Replace(",", "");

                                //add to textbox
                                label_id_richTextBox.Text = label_id_richTextBox.Text.Trim() + " " + carrier_code.Trim() + " < ";

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains(ShipEngineUI.label_url_label_id.Trim() + ".png") == true)
                            {

                                string img_url1 = currentLine.Replace("\"png\": \"", "");
                                string img_url = img_url1.Replace("\",", "");

                                //add to textbox
                                label_id_richTextBox.Text += img_url.Trim() + Environment.NewLine + ",";
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                            if (currentLine.Contains("\"total\": 543"))
                            {

                                ShipEngineUI.get_label_URL = ShipEngineUI.get_label_URL + "&page=2";

                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }

                        }

                        //PARSE AND ADD LABELS
                        string[] label_id_list1 = label_id_richTextBox.Text.Split(',');
                        string[] label_id_list = label_id_list1.Distinct().ToArray();
                        foreach (string label_id in label_id_list)
                        {
                            if (label_id.Trim() == "")
                                continue;

                            label_history_listbox.Items.Add(label_id.Trim());

                        }

                        if (label_history_listbox.Items.Count == 0)
                        {
                            label_history_listbox.Enabled = false;
                            create_manifest_button.Enabled = false;
                            label_history_listbox.Items.Add("ShipEngine found no label history for today.");
                        }
                        else if (label_history_listbox.Items.Count != 0)
                        {
                            label_history_listbox.Enabled = true;
                            create_manifest_button.Enabled = true;
                            label_history_listbox.Items.Remove("ShipEngine found no label history for today.");
                        }

                        RemoveURL(label_history_listbox, "http");
                    }
                }
            }
            catch (Exception HTTPexception)
            {

            }

        }

        public void GetCarrierBalance()
        {
            //GET CARRIER ACCOUNTS

            //GET CARRIER ID
            string carrier_id1 = add_funds_account_ComboBox.SelectedItem.ToString();
            carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
            string carrier_id = carrier_id1.Replace("|", "");

            current_balance_label.Text = "No Balance returned";

            try
            {
                //URL SOURCE
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/carriers/" + carrier_id;

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

                            if (currentLine.Contains("balance") == true)
                            {

                                string carrier_balance1 = currentLine.Replace("\"balance\": ", "");
                                string carrier_balance = carrier_balance1.Replace(",", "");

                                //add to textbox
                                carrier_balance_richTextBox.Text = carrier_balance_richTextBox.Text.Trim() + "," + Environment.NewLine + carrier_balance.Trim() + "|";
                                current_balance_label.Text = "$" + carrier_balance.Trim();
                            }
                            else
                            {
                                currentLine.Replace(currentLine, "");
                            }
                        }
                    }
                }
            }
            catch (Exception HTTPexception)
            {

            }
        }

        //Fixes UPS Having two label download objects
        private void RemoveURL(ListBox listBox, string input_string)
        {
            input_string.StartsWith("http");

            int index = listBox.FindString(input_string);

            if (index == -1) return;

            while (index != -1)
            {

                listBox.Items.RemoveAt(index);
                index = listBox.FindString(input_string, index);

            }
        }

        #endregion

        //Creating a Manifest
        public void CreateManifest()
        {

            //LOGID
            Random Manifest_logID = new Random();
            string Manifest_log = Manifest_logID.Next(0, 1000000).ToString("D6");

            try
            {

                //URI - POST
                WebRequest create_manifest_request = WebRequest.Create("https://api.shipengine.com/v1/manifests");
                create_manifest_request.Method = "POST";

                //API Key
                create_manifest_request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                string create_manifest_requestBody = "" +
                    "{\r\n" +
                    "  \"label_ids\": [" + manifest_label_id_richTextBox.Text + "]" +
                    "\r\n}";

                ASCIIEncoding create_manifest_encoding = new ASCIIEncoding();
                byte[] create_manifest_data = create_manifest_encoding.GetBytes(create_manifest_requestBody);

                create_manifest_request.ContentType = "application/json";
                create_manifest_request.ContentLength = create_manifest_data.Length;

                Stream create_manifest_stream = create_manifest_request.GetRequestStream();

                //Documents path REQUEST LOG
                string create_manifest_docPath = @"..\..\Resources\Logs";
                //File.WriteAllText(Path.Combine(create_manifest_docPath, "CreateManifestRequest - " + Manifest_log + ".txt"), create_manifest_requestBody);

                create_manifest_stream.Write(create_manifest_data, 0, create_manifest_data.Length);
                create_manifest_stream.Close();

                WebResponse create_manifest_requestResponse = create_manifest_request.GetResponse();
                create_manifest_stream = create_manifest_requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(create_manifest_stream);
                create_manifest_richTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = create_manifest_richTextBox.Text;

                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("href") == true)
                        {

                            string manifest_pdf_url1 = currentLine.Replace("\"href\": \"", "");
                            string manifest_pdf_url = manifest_pdf_url1.Replace("\",", "");

                            //DECLARE VARIABLE
                            ShipEngineUI.manifest_pdf_url = manifest_pdf_url.Trim();

                            manifest_textBox.Text = ShipEngineUI.manifest_pdf_url.Replace("\"", "");
                        }
                    }
                }

            }
            catch (WebException create_manifest_Exception)
            {

                using (WebResponse ShipEngineErrorResponse = create_manifest_Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse1 = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse1))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR CREATING MANIFEST");

                            }
                        }
                    }
                }
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
            package_code_ComboBox.SelectedIndex = 0;
        }

        private void service_code_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void warehouse_id_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //GET WAREHOUSE ID
            string warehouse_id1 = warehouse_id_ComboBox.SelectedItem.ToString();
            warehouse_id1 = warehouse_id1.Remove(warehouse_id1.IndexOf("|") + 1);
            string warehouse_id = warehouse_id1.Replace("|", "");

            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/warehouses/" + warehouse_id;

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
            label_Tab_Control.SelectedIndex = 1;

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

                //SHIPENGINE UI VARIABLES
                #region SHIPENGINE UI VARIABLES
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

                //SHIPENGINE UI ADVANCED OPTIONS
                ShipEngineUI.advanced_options_bill_to_account = bill_to_account_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_country_code = bill_to_country_code_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_party = bill_to_party_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_bill_to_postal_code = bill_to_postal_code_textBox.Text;

                ShipEngineUI.advanced_options_contains_alcohol = contains_alcohol_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_delivered_duty_paid = delivered_duty_paid_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_dry_ice = dry_ice_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_value = dry_ice_weight_numericUpDown.Value.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_unit = dry_ice_weight_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_non_machinable = non_machinable_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_saturday_delivery = saturday_delivery_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_origin_type = origin_type_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_custom_field1 = custom_field1_textBox.Text;
                ShipEngineUI.advanced_options_custom_field2 = custom_field2_textBox.Text;
                ShipEngineUI.advanced_options_custom_field3 = custom_field3_textBox.Text;

                ShipEngineUI.insurance_provider = insurance_provider_comboBox.SelectedItem.ToString();
                ShipEngineUI.packages_insured_value_amount = insured_value_amont_numericUpDown.Value.ToString();
                ShipEngineUI.packages_insured_value_currency = insured_value_currency_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_third_party_consignee = third_party_consignee_comboBox.SelectedItem.ToString();
                #endregion

                //RATE REQUEST
                #region  Rate Request
                //POST REQUEST
                string rateRequestBody = "" +
                    "{\r\n    \"rate_options\": {" +
                    "\r\n        \"carrier_ids\": [" +
                    "\r\n            \"" + carrier_id + "\"" +
                    "\r\n        ]" +
                    "\r\n    }," +
                    "\r\n    \"shipment\": {" +
                    "\r\n        \"validate_address\": \"no_validation\"," +
                    "\r\n        \"carrier_id\": \"" + carrier_id + "\"," +
                    "\r\n        \"warehouse_id\": \"\"," +
                    "\r\n        \"service_code\": \"" + service_code + "\"," +
                    "\r\n        \"external_order_id\": null," +
                    "\r\n        \"ship_date\": \"" + ship_date + "\"," +
                    "\r\n        \"is_return_label\": false," +
                    "\r\n        \"items\": []," +
                    "\r\n        \"ship_to\": {" +
                    "\r\n            \"name\": \"" + ShipEngineUI.shipTo_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipTo_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipTo_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipTo_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipTo_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipTo_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipTo_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipTo_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipTo_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipTo_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipTo_address_residential_indicator + "\"" +
                    "\r\n        }," +
                    "\r\n        \"ship_from\": {" +
                    "\r\n            \"name\": \"" + ShipEngineUI.shipFrom_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipFrom_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipFrom_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipFrom_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipFrom_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipFrom_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipFrom_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipFrom_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipFrom_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipFrom_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipFrom_address_residential_indicator + "\"" +
                    "\r\n        }," +
                    "\r\n        \"confirmation\": \"" + delivery_confirmation_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n        \"advanced_options\": {" +
                    "\r\n            \"bill_to_account\": \"" + ShipEngineUI.advanced_options_bill_to_account + "\"," +
                    "\r\n            \"bill_to_country_code\": \"" + ShipEngineUI.advanced_options_bill_to_country_code + "\"," +
                    "\r\n            \"bill_to_party\": " + ShipEngineUI.advanced_options_bill_to_party + "," +
                    "\r\n            \"bill_to_postal_code\": \"" + ShipEngineUI.advanced_options_bill_to_postal_code + "\"," +
                    "\r\n            \"canada_delivered_duty\": null," +
                    "\r\n            \"contains_alcohol\": \"" + ShipEngineUI.advanced_options_contains_alcohol + "\"," +
                    "\r\n            \"delivered_duty_paid\": \"" + ShipEngineUI.advanced_options_delivered_duty_paid + "\"," +
                    "\r\n            \"non_machinable\": \"" + ShipEngineUI.advanced_options_non_machinable + "\"," +
                    "\r\n            \"saturday_delivery\": \"" + ShipEngineUI.advanced_options_saturday_delivery + "\"," +
                    "\r\n            \"third-party-consignee\": \"false\"," +
                    "\r\n            \"ancillary_endorsements_option\": null," +
                    "\r\n            \"freight_class\": null," +
                    "\r\n            \"custom_field_1\": \"" + ShipEngineUI.advanced_options_custom_field1 + "\"," +
                    "\r\n            \"custom_field_2\": \"" + ShipEngineUI.advanced_options_custom_field2 + "\"," +
                    "\r\n            \"custom_field_3\": \"" + ShipEngineUI.advanced_options_custom_field3 + "\"," +
                    "\r\n            \"return_pickup_attempts\": null," +
                    "\r\n            \"dry_ice\": \"" + ShipEngineUI.advanced_options_dry_ice + "\"," +
                    "\r\n            \"dry_ice_weight\": {" +
                    "\r\n                \"value\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_value + "\"," +
                    "\r\n                \"unit\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_unit + "\"" +
                    "\r\n            }," +
                    "\r\n            \"collect_on_delivery\": {" +
                    "\r\n                \"payment_type\": \"none\"," +
                    "\r\n                \"payment_amount\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": \"0.00\"" +
                    "\r\n                }" +
                    "\r\n            }" +
                    "\r\n        }," +
                    "\r\n        \"origin_type\": " + ShipEngineUI.advanced_options_origin_type + "," +
                    "\r\n        \"insurance_provider\": \"" + ShipEngineUI.insurance_provider + "\"," +
                    "\r\n        \"packages\": [" +
                    "\r\n            {" +
                    "\r\n                \"package_code\": \"" + package_code_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n                \"weight\": {" +
                    "\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": \"pound\"" +
                    "\r\n                }," +
                    "\r\n                \"dimensions\": {" +
                    "\r\n                    \"unit\": \"inch\"," +
                    "\r\n                    \"length\": " + ShipEngineUI.packages_dimensions_length + "," +
                    "\r\n                    \"width\": " + ShipEngineUI.packages_dimensions_width + "," +
                    "\r\n                    \"height\": " + ShipEngineUI.packages_dimensions_height + "" +
                    "\r\n                }," +
                    "\r\n                \"insured_value\": {" +
                    "\r\n                    \"currency\": \"" + ShipEngineUI.packages_insured_value_currency + "\"," +
                    "\r\n                    \"amount\": " + ShipEngineUI.packages_insured_value_amount +
                    "\r\n                }," +
                    "\r\n                \"label_messages\": {" +
                    "\r\n                    \"reference1\": null," +
                    "\r\n                    \"reference2\": null," +
                    "\r\n                    \"reference3\": null" +
                    "\r\n                }," +
                    "\r\n                \"external_package_id\": \"" + rateLogId + "\"" +
                    "\r\n            }" +
                    "\r\n        ]" +
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
                // File.WriteAllText(Path.Combine(docPath, "RateRequest - " + rateLogId + ".txt"), rateRequestBody);


                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                rate_response_RichTextBox.Text = parseResponse.ReadToEnd();

                string responseBodyText = rate_response_RichTextBox.Text;
                //File.WriteAllText(Path.Combine(docPath, "RateResponse - " + rateLogId + ".txt"), responseBodyText);

                stream.Close();

                //LOAD LISTBOX

                using (var reader = new StringReader(responseBodyText))
                {

                    string ratesResponse = "";

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("\"amount\"") == true)
                        {

                            ratesResponse += currentLine + "~";

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
                    rate_response_RichTextBox.Text = rate_response_RichTextBox.Text.Replace("0~", "");

                    FixRate();

                    //Fixes Fedex having multiple amount properties.
                    FedexRateFix();

                    rate_request_RichTextBox.Text = rateRequestBody;

                }

                stream.Close();

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {
                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR GETTING RATES");

                            }
                        }
                    }
                }
            }
            catch (Exception exeption)
            {
                MessageBox.Show(exeption.ToString());
            }
        }

        private void rate_response_RichTextBox_Click(object sender, EventArgs e)
        {

            rate_response_RichTextBox.Visible = false;

            rate_request_RichTextBox.Visible = true;

        }

        private void rate_request_RichTextBox_Click(object sender, EventArgs e)
        {
            rate_response_RichTextBox.Visible = true;

            rate_request_RichTextBox.Visible = false;
        }

        private void create_label_Button_Click(object sender, EventArgs e)
        {

            label_RichTextBox.Clear();
            void_label_id_TextBox.Text = string.Empty;
            label_Tab_Control.SelectedIndex = 0;
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
                string labelLogId = logID.Next(0, 1000000).ToString("D6");

                //SHIP DATE
                string ship_date = ship_date_TextBox.Text;


                //SHIPENGINE UI VARIABLES
                #region SHIPENGINE UI VARIABLES
                //Address Variables
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

                //SHIPENGINE UI ADVANCED OPTIONS
                ShipEngineUI.advanced_options_bill_to_account = bill_to_account_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_country_code = bill_to_country_code_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_party = bill_to_party_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_bill_to_postal_code = bill_to_postal_code_textBox.Text;

                ShipEngineUI.advanced_options_contains_alcohol = contains_alcohol_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_delivered_duty_paid = delivered_duty_paid_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_dry_ice = dry_ice_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_value = dry_ice_weight_numericUpDown.Value.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_unit = dry_ice_weight_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_non_machinable = non_machinable_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_saturday_delivery = saturday_delivery_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_custom_field1 = custom_field1_textBox.Text;
                ShipEngineUI.advanced_options_custom_field2 = custom_field2_textBox.Text;
                ShipEngineUI.advanced_options_custom_field3 = custom_field3_textBox.Text;

                ShipEngineUI.insurance_provider = insurance_provider_comboBox.SelectedItem.ToString();
                ShipEngineUI.packages_insured_value_amount = insured_value_amont_numericUpDown.Value.ToString();
                ShipEngineUI.packages_insured_value_currency = insured_value_currency_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_third_party_consignee = third_party_consignee_comboBox.SelectedItem.ToString();
                #endregion

                //URI - POST
                WebRequest request = WebRequest.Create("https://api.shipengine.com/v1/labels");
                request.Method = "POST";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                #region REQUEST BODY
                string createLabelrequestBody = "" +
                    "{\r\n    \"rate_options\": {" +
                    "\r\n        \"carrier_ids\": [" +
                    "\r\n            \"" + carrier_id + "\"" +
                    "\r\n        ]" +
                    "\r\n    }," +
                    "\r\n    \"shipment\": {" +
                    "\r\n        \"validate_address\": \"no_validation\"," +
                    "\r\n        \"carrier_id\": \"" + carrier_id + "\"," +
                    "\r\n        \"warehouse_id\": \"\"," +
                    "\r\n        \"service_code\": \"" + service_code + "\"," +
                    "\r\n        \"external_order_id\": null," +
                    "\r\n        \"ship_date\": \"" + ship_date + "\"," +
                    "\r\n        \"is_return_label\": false," +
                    "\r\n        \"items\": []," +
                    "\r\n        \"ship_to\": {" +
                    "\r\n            \"name\": \"" + ShipEngineUI.shipTo_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipTo_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipTo_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipTo_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipTo_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipTo_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipTo_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipTo_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipTo_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipTo_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipTo_address_residential_indicator + "\"" +
                    "\r\n        }," +
                    "\r\n        \"ship_from\": {" +
                    "\r\n            \"name\": \"" + ShipEngineUI.shipFrom_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipFrom_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipFrom_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipFrom_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipFrom_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipFrom_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipFrom_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipFrom_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipFrom_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipFrom_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + ShipEngineUI.shipFrom_address_residential_indicator + "\"" +
                    "\r\n        }," +
                    "\r\n        \"confirmation\": \"" + delivery_confirmation_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n        \"advanced_options\": {" +
                    "\r\n            \"bill_to_account\": \"" + ShipEngineUI.advanced_options_bill_to_account + "\"," +
                    "\r\n            \"bill_to_country_code\": \"" + ShipEngineUI.advanced_options_bill_to_country_code + "\"," +
                    "\r\n            \"bill_to_party\": " + ShipEngineUI.advanced_options_bill_to_party + "," +
                    "\r\n            \"bill_to_postal_code\": \"" + ShipEngineUI.advanced_options_bill_to_postal_code + "\"," +
                    "\r\n            \"canada_delivered_duty\": null," +
                    "\r\n            \"contains_alcohol\": \"" + ShipEngineUI.advanced_options_contains_alcohol + "\"," +
                    "\r\n            \"delivered_duty_paid\": \"" + ShipEngineUI.advanced_options_delivered_duty_paid + "\"," +
                    "\r\n            \"non_machinable\": \"" + ShipEngineUI.advanced_options_non_machinable + "\"," +
                    "\r\n            \"saturday_delivery\": \"" + ShipEngineUI.advanced_options_saturday_delivery + "\"," +
                    "\r\n            \"third-party-consignee\": \"" + ShipEngineUI.advanced_options_third_party_consignee + "\"," +
                    "\r\n            \"ancillary_endorsements_option\": null," +
                    "\r\n            \"freight_class\": null," +
                    "\r\n            \"custom_field_1\": \"" + ShipEngineUI.advanced_options_custom_field1 + "\"," +
                    "\r\n            \"custom_field_2\": \"" + ShipEngineUI.advanced_options_custom_field2 + "\"," +
                    "\r\n            \"custom_field_3\": \"" + ShipEngineUI.advanced_options_custom_field3 + "\"," +
                    "\r\n            \"return_pickup_attempts\": null," +
                    "\r\n            \"dry_ice\": \"" + ShipEngineUI.advanced_options_dry_ice + "\"," +
                    "\r\n            \"dry_ice_weight\": {" +
                    "\r\n                \"value\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_value + "\"," +
                    "\r\n                \"unit\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_unit + "\"" +
                    "\r\n            }," +
                    "\r\n            \"collect_on_delivery\": {" +
                    "\r\n                \"payment_type\": \"none\"," +
                    "\r\n                \"payment_amount\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": \"0.00\"" +
                    "\r\n                }" +
                    "\r\n            }" +
                    "\r\n        }," +
                    "\r\n        \"origin_type\": null," +
                    "\r\n        \"insurance_provider\": \"" + ShipEngineUI.insurance_provider + "\"," +
                    "\r\n        \"packages\": [" +
                    "\r\n            {" +
                    "\r\n                \"package_code\": \"" + package_code_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n                \"weight\": {" +
                    "\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": \"pound\"" +
                    "\r\n                }," +
                    "\r\n                \"dimensions\": {" +
                    "\r\n                    \"unit\": \"inch\"," +
                    "\r\n                    \"length\": " + ShipEngineUI.packages_dimensions_length + "," +
                    "\r\n                    \"width\": " + ShipEngineUI.packages_dimensions_width + "," +
                    "\r\n                    \"height\": " + ShipEngineUI.packages_dimensions_height + "" +
                    "\r\n                }," +
                    "\r\n                \"insured_value\": {" +
                    "\r\n                    \"currency\": \"" + ShipEngineUI.packages_insured_value_currency + "\"," +
                    "\r\n                    \"amount\": " + ShipEngineUI.packages_insured_value_amount +
                    "\r\n                }," +
                    "\r\n                \"label_messages\": {" +
                    "\r\n                    \"reference1\": null," +
                    "\r\n                    \"reference2\": null," +
                    "\r\n                    \"reference3\": null" +
                    "\r\n                }," +
                    "\r\n                \"external_package_id\": \"" + labelLogId + "\"" +
                    "\r\n            }" +
                    "\r\n        ]" +
                    "\r\n    }" +
                    "\r\n}";
                #endregion

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(createLabelrequestBody);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                //Documents path REQUEST LOG
                string docPath = @"..\..\Resources\Labels";
                //File.WriteAllText(Path.Combine(docPath, "LabelRequest - " + labelLogId + ".txt"), createLabelrequestBody);

                stream.Write(data, 0, data.Length);
                stream.Close();

                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();


                StreamReader parseResponse = new StreamReader(stream);
                label_RichTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = label_RichTextBox.Text;

                //Documents path RESPONSE LOG
                //File.WriteAllText(Path.Combine(docPath, "LabelResponse - " + labelLogId + ".txt"), responseBodyText);

                //GET LABELID
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
                    client.DownloadFileAsync(new Uri(imgURL3 + ".png"), @"..\..\Resources\Labels\Label-" + labelLogId + ".png");
                }

                labelImageBox.Load(imgURL3 + ".png");

                void_label_id_TextBox.Text = ShipEngineUI.label_id;

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

                label_history_listbox.Items.Clear();
                GetCurrentTrackingNumber();
                GetLabelHistory();
            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR CREATING LABEL");

                            }
                        }
                    }
                }
            }
            catch (Exception Exception)
            {
                MessageBox.Show(Exception.Message);
            }
        }

        public void FedexRateFix()
        {
            using (var reader = new StringReader(rate_response_RichTextBox.Text))
            {

                string fixed_rate_response = "";

                for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                {

                    if (currentLine.Contains("~"))
                    {

                        currentLine = currentLine.Substring(0, currentLine.IndexOf("~") + 1);

                        fixed_rate_response += currentLine + Environment.NewLine; 

                    }
                    else
                    {
                        fixed_rate_response += currentLine + Environment.NewLine;
                    }
                }

                rate_response_RichTextBox.Text = fixed_rate_response;
                rate_response_RichTextBox.Text = rate_response_RichTextBox.Text.Replace("~", "");
            }
        }

        void LabelImage(object o, PrintPageEventArgs e)
        {

            System.Drawing.Image image = this.labelImageBox.Image;

            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.DrawImage(image, e.PageBounds);


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

                //SHIP DATE
                string ship_date = ship_date_TextBox.Text;

                //SHIPENGINE UI VARIABLES
                #region SHIPENGINE UI VARIABLES
                //SHIP FROM VARIABLES
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

                //SHIPENGINE UI ADVANCED OPTIONS
                ShipEngineUI.advanced_options_bill_to_account = bill_to_account_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_country_code = bill_to_country_code_textBox.Text;
                ShipEngineUI.advanced_options_bill_to_party = bill_to_party_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_bill_to_postal_code = bill_to_postal_code_textBox.Text;

                ShipEngineUI.advanced_options_contains_alcohol = contains_alcohol_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_delivered_duty_paid = delivered_duty_paid_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_dry_ice = dry_ice_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_value = dry_ice_weight_numericUpDown.Value.ToString();
                ShipEngineUI.advanced_options_dry_ice_weight_unit = dry_ice_weight_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_non_machinable = non_machinable_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_saturday_delivery = saturday_delivery_comboBox.SelectedItem.ToString();
                ShipEngineUI.advanced_options_origin_type = origin_type_comboBox.SelectedItem.ToString();


                ShipEngineUI.advanced_options_custom_field1 = custom_field1_textBox.Text;
                ShipEngineUI.advanced_options_custom_field2 = custom_field2_textBox.Text;
                ShipEngineUI.advanced_options_custom_field3 = custom_field3_textBox.Text;

                ShipEngineUI.insurance_provider = insurance_provider_comboBox.SelectedItem.ToString();
                ShipEngineUI.packages_insured_value_amount = insured_value_amont_numericUpDown.Value.ToString();
                ShipEngineUI.packages_insured_value_currency = insured_value_currency_comboBox.SelectedItem.ToString();

                ShipEngineUI.advanced_options_third_party_consignee = third_party_consignee_comboBox.SelectedItem.ToString();
                #endregion

                //LOGID
                Random logID = new Random();
                string sales_order_label_LogId = logID.Next(0, 1000000).ToString("D6");

                //URI - POST
                WebRequest request = WebRequest.Create("https://api.shipengine.com/v-beta/labels/sales_order/" + sales_order_id);
                request.Method = "POST";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                #region Sales Order request
                string sales_order_LabelrequestBody = "{" +
                    "\r\n    \"label_format\": \"pdf\"," +
                    "\r\n    \"shipment\": {" +
                    "\r\n        \"carrier_id\": \"" + carrier_id + "\"," +
                    "\r\n        \"service_code\": \"" + service_code + "\"," +
                    "\r\n        \"validate_address\": \"no_validation\"," +
                    "\r\n        \"warehouse_id\": \"\"," +
                    "\r\n        \"external_order_id\": null," +
                    "\r\n        \"ship_date\": \"" + ship_date + "\"," +
                    "\r\n        \"is_return_label\": false," +
                    "\r\n        \"items\": []," +
                    "\r\n        \"ship_from\": {" +
                    "\r\n            \"name\": \"" + ShipEngineUI.shipFrom_name + "\"," +
                    "\r\n            \"phone\": \"" + ShipEngineUI.shipFrom_phone + "\"," +
                    "\r\n            \"company_name\": \"" + ShipEngineUI.shipFrom_company_name + "\"," +
                    "\r\n            \"address_line1\": \"" + ShipEngineUI.shipFrom_address_line1 + "\"," +
                    "\r\n            \"address_line2\": \"" + ShipEngineUI.shipFrom_address_line2 + "\"," +
                    "\r\n            \"address_line3\": \"" + ShipEngineUI.shipFrom_address_line3 + "\"," +
                    "\r\n            \"city_locality\": \"" + ShipEngineUI.shipFrom_city_locality + "\"," +
                    "\r\n            \"state_province\": \"" + ShipEngineUI.shipFrom_state_province + "\"," +
                    "\r\n            \"postal_code\": \"" + ShipEngineUI.shipFrom_postal_code + "\"," +
                    "\r\n            \"country_code\": \"" + ShipEngineUI.shipFrom_country_code + "\"," +
                    "\r\n            \"address_residential_indicator\": \"" + shipFrom_address_residential_indicator_comboBox.SelectedItem.ToString() + "\"" +
                    "\r\n        }," +
                    "\r\n        \"confirmation\": \"" + delivery_confirmation_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n        \"advanced_options\": {" +
                    "\r\n            \"bill_to_account\": \"" + ShipEngineUI.advanced_options_bill_to_account + "\"," +
                    "\r\n            \"bill_to_country_code\": " + ShipEngineUI.advanced_options_bill_to_country_code + "," +
                    "\r\n            \"bill_to_party\": null," +
                    "\r\n            \"bill_to_postal_code\": \"" + ShipEngineUI.advanced_options_bill_to_postal_code + "\"," +
                    "\r\n            \"canada_delivered_duty\": null," +
                    "\r\n            \"contains_alcohol\": \"" + ShipEngineUI.advanced_options_contains_alcohol + "\"," +
                    "\r\n            \"delivered_duty_paid\": \"" + ShipEngineUI.advanced_options_delivered_duty_paid + "\"," +
                    "\r\n            \"non_machinable\": \"" + ShipEngineUI.advanced_options_non_machinable + "\"," +
                    "\r\n            \"saturday_delivery\": \"" + ShipEngineUI.advanced_options_saturday_delivery + "\"," +
                    "\r\n            \"third-party-consignee\": \"" + ShipEngineUI.advanced_options_third_party_consignee + "\"," +
                    "\r\n            \"ancillary_endorsements_option\": null," +
                    "\r\n            \"freight_class\": null," +
                    "\r\n            \"custom_field_1\": \"" + ShipEngineUI.advanced_options_custom_field1 + "\"," +
                    "\r\n            \"custom_field_2\": \"" + ShipEngineUI.advanced_options_custom_field2 + "\"," +
                    "\r\n            \"custom_field_3\": \"" + ShipEngineUI.advanced_options_custom_field3 + "\"," +
                    "\r\n            \"return_pickup_attempts\": null," +
                    "\r\n            \"dry_ice\": \"" + ShipEngineUI.advanced_options_dry_ice + "\"," +
                    "\r\n            \"dry_ice_weight\": {" +
                    "\r\n                \"value\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_value + "\"," +
                    "\r\n                \"unit\": \"" + ShipEngineUI.advanced_options_dry_ice_weight_unit + "\"" +
                    "\r\n            }," +
                    "\r\n            \"collect_on_delivery\": {" +
                    "\r\n                \"payment_type\": \"none\"," +
                    "\r\n                \"payment_amount\": {" +
                    "\r\n                    \"currency\": \"usd\"," +
                    "\r\n                    \"amount\": \"0.00\"" +
                    "\r\n                }" +
                    "\r\n            }" +
                    "\r\n        }," +
                    "\r\n        \"origin_type\": " + ShipEngineUI.advanced_options_origin_type + "," +
                    "\r\n        \"insurance_provider\": \"" + ShipEngineUI.insurance_provider + "\"," +
                    "\r\n        \"packages\": [" +
                    "\r\n            {" +
                    "\r\n                \"package_code\": \"" + package_code_ComboBox.SelectedItem.ToString() + "\"," +
                    "\r\n                \"weight\": {" +
                    "\r\n                    \"value\": " + ShipEngineUI.packages_weight_value + "," +
                    "\r\n                    \"unit\": \"pound\"" +
                    "\r\n                }," +
                    "\r\n                \"dimensions\": {" +
                    "\r\n                    \"unit\": \"inch\"," +
                    "\r\n                    \"length\": " + ShipEngineUI.packages_dimensions_length + "," +
                    "\r\n                    \"width\": " + ShipEngineUI.packages_dimensions_width + "," +
                    "\r\n                    \"height\": " + ShipEngineUI.packages_dimensions_height + "" +
                    "\r\n                }," +
                    "\r\n                \"insured_value\": {" +
                    "\r\n                    \"currency\": \"" + ShipEngineUI.packages_insured_value_currency + "\"," +
                    "\r\n                    \"amount\": " + ShipEngineUI.packages_insured_value_amount +
                    "\r\n                }," +
                    "\r\n                \"label_messages\": {" +
                    "\r\n                    \"reference1\": null," +
                    "\r\n                    \"reference2\": null," +
                    "\r\n                    \"reference3\": null" +
                    "\r\n                }," +
                    "\r\n                \"external_package_id\": \"" + sales_order_label_LogId + "\"," +
                    "\r\n            }" +
                    "\r\n        ]" +
                    "\r\n    }" +
                    "\r\n}";
                #endregion

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(sales_order_LabelrequestBody);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                //Documents path REQUEST LOG
                string docPath = @"..\..\Resources\Labels";
                //File.WriteAllText(Path.Combine(docPath, "SalesOrderLabelRequest - " + sales_order_label_LogId + ".txt"), sales_order_LabelrequestBody);

                stream.Write(data, 0, data.Length);
                stream.Close();

                WebResponse requestResponse = request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                sales_order_label_RichTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = sales_order_label_RichTextBox.Text;

                //File.WriteAllText(Path.Combine(docPath, "SalesOrderLabelResponse - " + sales_order_label_LogId + ".txt"), responseBodyText);

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

                            void_label_id_TextBox.Text = ShipEngineUI.label_id;

                        }

                        if (currentLine.Contains("tracking_number") == true)
                        {

                            string tracking_number1 = currentLine.Replace("\"tracking_number\": \"", "");
                            string tracking_number = tracking_number1.Replace("\",", "");

                            ShipEngineUI.Tracking_number = tracking_number.Trim();

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
                    //client.DownloadFileAsync(new Uri(imgURL3 + ".png"), @"..\..\Resources\Labels\Label-" + sales_order_label_LogId + ".png");
                }

                label_RichTextBox.Text = sales_order_label_RichTextBox.Text;
                labelImageBox.Load(imgURL3 + ".png");

                if (notify_shipped_checkBox.Checked == true)
                {
                    try
                    {

                        //URI - POST
                        WebRequest notify_shipped_request = WebRequest.Create("https://api.shipengine.com/v-beta/sales_orders/" + sales_order_id.Trim() + "/notify_shipped");
                        notify_shipped_request.Method = "POST";

                        //API Key
                        notify_shipped_request.Headers.Add("API-key", ShipEngineUI.apiKey);

                        //POST REQUEST
                        string notify_shipped_requestBody = "" +
                            "{\r\n  \"tracking_number\": \"" + ShipEngineUI.Tracking_number + "\"," +
                            "\r\n  \"carrier_code\": \"" + carrier_id + "\"" +
                            "\r\n}";

                        ASCIIEncoding notify_shipped_encoding = new ASCIIEncoding();
                        byte[] notify_shipped_data = notify_shipped_encoding.GetBytes(notify_shipped_requestBody);

                        notify_shipped_request.ContentType = "application/json";
                        notify_shipped_request.ContentLength = notify_shipped_data.Length;

                        Stream notify_shipped_stream = notify_shipped_request.GetRequestStream();

                        //Documents path REQUEST LOG
                        string notify_shipped_docPath = @"..\..\Resources\Logs";
                        //File.WriteAllText(Path.Combine(notify_shipped_docPath, "SalesOrderNotifyRequest - " + sales_order_label_LogId + ".txt"), notify_shipped_requestBody);

                        notify_shipped_stream.Write(notify_shipped_data, 0, notify_shipped_data.Length);
                        notify_shipped_stream.Close();

                        WebResponse notify_shipped_requestResponse = notify_shipped_request.GetResponse();
                        notify_shipped_stream = notify_shipped_requestResponse.GetResponseStream();

                    }
                    catch (WebException Exception)
                    {

                        using (WebResponse ShipEngineErrorResponse = Exception.Response)
                        {
                            HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                            Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                            using (Stream parseResponse1 = ShipEngineErrorResponse.GetResponseStream())

                            using (var reader = new StreamReader(parseResponse1))
                            {

                                for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                                {

                                    if (currentLine.Contains("message") == true)
                                    {

                                        string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                        string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                        MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR NOTIFYING MARKETPLACE");

                                    }
                                }
                            }
                        }
                    }
                }

                label_history_listbox.Items.Clear();
                GetCurrentTrackingNumber();
                GetLabelHistory();

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR CREATING LABEL FOR ORDER");

                            }
                        }
                    }
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
                string label_id = void_label_id_TextBox.Text;

                DialogResult dialogResult = MessageBox.Show("Are you sure you would like to void " + label_id + "?", "VOID LABEL", MessageBoxButtons.YesNo);

                manifest_label_id_richTextBox.Text = manifest_label_id_richTextBox.Text.Replace("\"" + void_label_id_TextBox.Text + "\",", "");

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {

                        //URI - POST
                        WebRequest request = WebRequest.Create("https://api.shipengine.com/v1/labels/" + label_id.Trim() + "/void");
                        request.Method = "PUT";

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

                            using (var reader = new StringReader(responseBodyText))
                            {

                                for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                                {

                                    if (currentLine.Contains("message") == true)
                                    {

                                        string void_label_id_Response1 = currentLine.Replace("\"message\": \"", "");
                                        string void_label_id_Response = void_label_id_Response1.Replace("\"", "");

                                        //DECLARE VARIABLE
                                        ShipEngineUI.void_label_id_Response = void_label_id_Response.Trim();

                                    }
                                }
                            }
                        }

                        MessageBox.Show(ShipEngineUI.void_label_id_Response, "VOID LABEL");

                        //CLOSE STREAM
                        parseResponse.Close();
                        stream.Close();

                        label_history_listbox.Items.Clear();
                        GetLabelHistory();

                    }
                    catch (Exception void_label_id_response_Error)
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
            catch (Exception voidlableexception)
            {
                MessageBox.Show(voidlableexception.ToString());
            }

        }

        #region visual click event methods

        private void advanced_options_groupBox_Click(object sender, EventArgs e)
        {

            shipTogroupBox.Visible = true;
            advanced_options_groupBox.Visible = false;

        }

        private void shipTogroupBox_Click(object sender, EventArgs e)
        {

            shipTogroupBox.Visible = false;
            advanced_options_groupBox.Visible = true;


        }

        private void shipFromgroupBox_Click(object sender, EventArgs e)
        {

            shipFromgroupBox.Visible = false;
            advanced_options_groupBox1.Visible = true;


        }

        private void advanced_options_groupBox1_Click(object sender, EventArgs e)
        {

            shipFromgroupBox.Visible = true;

            advanced_options_groupBox1.Visible = false;

        }

        private void package_code_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void label_history_listbox_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                //GET SELECTED LabelID ID
                string label_id1 = label_history_listbox.SelectedItem.ToString();
                label_id1 = label_id1.Remove(label_id1.IndexOf("|") + 1);
                string label_id = label_id1.Replace("|", "");

                //GET LABEL URL
                string label_url1 = label_history_listbox.SelectedItem.ToString();
                //label_url1 = label_url1.Remove(label_url1.LastIndexOf("<") + 1);
                string label_url = label_url1.Substring(label_url1.LastIndexOf('<') + 1);

                void_label_id_TextBox.Text = label_id.Trim();
                labelImageBox.Load(label_url);

                if (label_history_listbox.Text.Contains("completed"))
                {
                    //Select labels to void
                    if (!manifest_label_id_richTextBox.Text.Contains(label_id.Trim()))
                    {
                        manifest_label_id_richTextBox.Text += "\"" + label_id.Trim() + "\"" + ",";
                    }
                    else if (manifest_label_id_richTextBox.Text.Contains(label_id.Trim()))
                    {
                        string currentSelection = "\"" + label_id.Trim() + "\",";

                        manifest_label_id_richTextBox.Text = manifest_label_id_richTextBox.Text.Replace(currentSelection, "");
                    }
                }
                else
                {

                }

                //GET LABELS
                try
                {

                    //URL SOURCE
                    string URLstring = "https://api.shipengine.com/v1/labels/" + label_id.Trim();

                    //REQUEST
                    WebRequest requestObject = WebRequest.Create(URLstring);
                    requestObject.Method = "GET";

                    //SE AUTH
                    requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                    //RESPONSE
                    HttpWebResponse responseObjectGet = null;
                    responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                    string streamResponse = null;

                    //Get List labels data
                    using (Stream stream = responseObjectGet.GetResponseStream())
                    {
                        StreamReader responseRead = new StreamReader(stream);
                        streamResponse = responseRead.ReadToEnd();

                        label_RichTextBox.Text = streamResponse;

                        using (var reader = new StringReader(streamResponse))
                        {

                            for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                            {



                            }
                        }
                    }

                    GetCurrentTrackingNumber();
                }
                catch (Exception HTTPexception)
                {

                }
            }
            catch (Exception HTTPexception)
            {

            }

        }

        private void create_manifest_button_Click(object sender, EventArgs e)
        {

            CreateManifest();

        }

        private void clear_manifest_textBox_button_Click(object sender, EventArgs e)
        {
            manifest_label_id_richTextBox.Text = string.Empty;
        }

        public void GetCurrentTrackingNumber()
        {
            string input = label_RichTextBox.Text;

            using (var reader = new StringReader(input))
            {

                for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                {

                    if (currentLine.Contains("tracking_number") == true)
                    {

                        string tracking_number1 = currentLine.Replace("\"tracking_number\": \"", "");
                        string tracking_number = tracking_number1.Replace("\",", "");

                        ShipEngineUI.Tracking_number = tracking_number.Trim();

                        tracking_number_textBox.Text = "Tracking Number: " + ShipEngineUI.Tracking_number;
                        track_shipment_TextBox.Text = ShipEngineUI.Tracking_number;

                    }

                }
            }
        }

        private void label_RichTextBox_TextChanged(object sender, EventArgs e)
        {

            GetCurrentTrackingNumber();

        }

        public void FixRate()
        {
        }

        private void track_Shipment_Button_Click(object sender, EventArgs e)
        {

            tracking_RichTextBox.Text = "";

            try
            {
                //URL SOURCE
                string URLstring = "https://api.shipengine.com/v1/tracking?carrier_code=" + carrier_code_comboBox.SelectedItem.ToString() + "&tracking_number=" + track_shipment_TextBox.Text;

                //REQUEST
                WebRequest requestObject = WebRequest.Create(URLstring);
                requestObject.Method = "GET";

                //SE AUTH
                requestObject.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
                string streamResponse = null;

                //Get List labels data
                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    tracking_RichTextBox.Text = streamResponse;

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {



                        }
                    }
                }
            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse1 = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse1))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR TRACKING PACKAGE");

                            }
                        }
                    }
                }
            }
        }

        private void update_carrier_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_carrier_account = update_carrier_listBox.SelectedItem.ToString();

            if (selected_carrier_account.Contains("fedex"))
            {
                update_carrier_richTextBox.Enabled = true;
                update_carrier_richTextBox.Text = SE_Carrier_Connection.connections_fedex_settings;
            }
            else if (selected_carrier_account.Contains("ups"))
            {
                update_carrier_richTextBox.Enabled = true;
                update_carrier_richTextBox.Text = SE_Carrier_Connection.connections_ups_settings;
            }
            else if (selected_carrier_account.Contains("usps"))
            {
                update_carrier_richTextBox.Enabled = true;
                update_carrier_richTextBox.Text = SE_Carrier_Connection.connections_stamps_com;
            }
            else if (selected_carrier_account.Contains("stamps"))
            {
                update_carrier_richTextBox.Enabled = true;
                update_carrier_richTextBox.Text = SE_Carrier_Connection.connections_stamps_com;
            }
            else if (selected_carrier_account.Contains("endicia"))
            {
                update_carrier_richTextBox.Enabled = true;
                update_carrier_richTextBox.Text = SE_Carrier_Connection.connections_endicia;
            }
            else if (!selected_carrier_account.Contains("ups") || selected_carrier_account.Contains("fedex") || selected_carrier_account.Contains("usps")
                   || selected_carrier_account.Contains("stamps") || selected_carrier_account.Contains("endicia"))
            {
                update_carrier_richTextBox.Text = string.Empty;
                update_carrier_richTextBox.Enabled = false;
            }


        }

        private void update_carrier_button_Click(object sender, EventArgs e)
        {

            try
            {
                string carrier_id1 = update_carrier_listBox.SelectedItem.ToString();
                carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
                string carrier_id = carrier_id1.Replace("|", "");

                string carrier_name1 = update_carrier_listBox.SelectedItem.ToString();
                carrier_name1 = carrier_name1.Substring(carrier_name1.IndexOf("|"));
                string carrier_name = carrier_name1.Replace("|", "");

                //URI - PUT
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/connections/carriers/" + carrier_name + "/" + carrier_id + "/settings".Trim();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                request.Method = "PUT";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //PUT REQUEST
                string update_carrier_request = update_carrier_richTextBox.Text;

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(update_carrier_request);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse requestResponse = (HttpWebResponse)request.GetResponse();
                stream = requestResponse.GetResponseStream();

                if (requestResponse.StatusCode == HttpStatusCode.NoContent)
                {

                    MessageBox.Show("Carrier settings have been updated.", "UPDATE CARRIER ACCOUNT");

                }

                StreamReader parseResponse = new StreamReader(stream);
                update_carrier_response_richTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = update_carrier_response_richTextBox.Text;

                //GET RESPONSE
                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {


                    }
                }

                stream.Close();

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (WebException Exception)
            {
                //refresh if failed
                current_carrier_settings_button_Click(sender, e);

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR UPDATING SETTINGS");

                            }
                        }
                    }
                }
            }

        }

        private void current_carrier_settings_button_Click(object sender, EventArgs e)
        {

            string carrier_id1 = update_carrier_listBox.SelectedItem.ToString();
            carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
            string carrier_id = carrier_id1.Replace("|", "");

            string carrier_name1 = update_carrier_listBox.SelectedItem.ToString();
            carrier_name1 = carrier_name1.Substring(carrier_name1.IndexOf("|"));
            string carrier_name = carrier_name1.Replace("|", "");

            //GET CARRIER SETTINGS
            try
            {
                //URL SOURCE
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/connections/carriers/" + carrier_name + "/" + carrier_id + "/settings".Trim();

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


                        }

                    }

                    update_carrier_richTextBox.Text = streamResponse;

                }

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR GETTING SETTINGS");

                            }
                        }
                    }
                }
            }
        }

        private void delete_carrier_button_Click(object sender, EventArgs e)
        {

            string carrier_id1 = update_carrier_listBox.SelectedItem.ToString();
            carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
            string carrier_id = carrier_id1.Replace("|", "");

            string carrier_name1 = update_carrier_listBox.SelectedItem.ToString();
            carrier_name1 = carrier_name1.Substring(carrier_name1.IndexOf("|"));
            string carrier_name = carrier_name1.Replace("|", "");

            DialogResult dialogResult = MessageBox.Show("Are you sure you would like to delete " + carrier_id + "?" + "\n" + "This action cannot be undone.", "DELETE CARRIER ACCOUNT", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {

                try
                {

                    //URI - POST
                    ShipEngineUI.urlString = "https://api.shipengine.com/v1/connections/carriers/" + carrier_name + "/" + carrier_id.Trim();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                    request.Method = "DELETE";

                    //API Key
                    request.Headers.Add("API-key", ShipEngineUI.apiKey);

                    //POST REQUEST
                    string update_carrier_request = update_carrier_richTextBox.Text;

                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(update_carrier_request);

                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;

                    Stream stream = request.GetRequestStream();

                    stream.Write(data, 0, data.Length);
                    stream.Close();

                    HttpWebResponse requestResponse = (HttpWebResponse)request.GetResponse();
                    stream = requestResponse.GetResponseStream();

                    if (requestResponse.StatusCode == HttpStatusCode.NoContent)
                    {

                        MessageBox.Show("Carrier account has been deleted.", "DELETE CARRIER ACCOUNT");

                    }

                    StreamReader parseResponse = new StreamReader(stream);
                    update_carrier_response_richTextBox.Text = parseResponse.ReadToEnd();
                    string responseBodyText = update_carrier_response_richTextBox.Text;

                    //GET RESPONSE
                    using (var reader = new StringReader(responseBodyText))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {


                        }
                    }

                    stream.Close();

                    //CLOSE STREAM
                    parseResponse.Close();
                    stream.Close();

                }
                catch (WebException Exception)
                {

                    using (WebResponse ShipEngineErrorResponse = Exception.Response)
                    {
                        HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                        Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                        using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                        using (var reader = new StreamReader(parseResponse))
                        {

                            for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                            {

                                if (currentLine.Contains("message") == true)
                                {

                                    string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                    string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                    MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR DELETING" + carrier_name.ToUpper() + "ACCOUNT");

                                }
                            }
                        }
                    }
                }
            }
            else if (dialogResult == DialogResult.No)
            {

                MessageBox.Show("You canceled the delete request for " + carrier_id + ".", "DELETE CARRIER ACCOUNT");

            }
        }

        private void connect_carrier_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selected_carrier_account = connect_carrier_ComboBox.SelectedItem.ToString();

            if (selected_carrier_account.Contains("fedex"))
            {
                connect_carrier_richTextBox.Enabled = true;
                connect_carrier_richTextBox.Text = SE_Carrier_Connection.connections_fedex;
            }
            else if (selected_carrier_account.Contains("ups"))
            {
                connect_carrier_richTextBox.Enabled = true;
                connect_carrier_richTextBox.Text = SE_Carrier_Connection.connections_ups;
            }
            else if (selected_carrier_account.Contains("usps"))
            {
                connect_carrier_richTextBox.Enabled = true;
                connect_carrier_richTextBox.Text = SE_Carrier_Connection.connections_stamps_com;
            }
            else if (selected_carrier_account.Contains("stamps"))
            {
                connect_carrier_richTextBox.Enabled = true;
                connect_carrier_richTextBox.Text = SE_Carrier_Connection.connections_stamps_com;
            }
            else if (selected_carrier_account.Contains("endicia"))
            {
                connect_carrier_richTextBox.Enabled = true;
                connect_carrier_richTextBox.Text = SE_Carrier_Connection.connections_endicia;
            }
            else if (!selected_carrier_account.Contains("ups") || selected_carrier_account.Contains("fedex") || selected_carrier_account.Contains("usps")
                   || selected_carrier_account.Contains("stamps") || selected_carrier_account.Contains("endicia"))
            {
                connect_carrier_richTextBox.Text = string.Empty;
                connect_carrier_richTextBox.Enabled = false;
            }


        }

        private void connect_carrier_button_Click(object sender, EventArgs e)
        {

            try
            {

                string carrier_name1 = connect_carrier_ComboBox.SelectedItem.ToString();
                carrier_name1 = carrier_name1.Substring(carrier_name1.IndexOf("|"));
                string carrier_name = carrier_name1.Replace("|", "");

                //URI - POST
                ShipEngineUI.urlString = "https://api.shipengine.com/v1/connections/carriers/" + carrier_name;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                request.Method = "PUT";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                string connect_carrier_request = connect_carrier_richTextBox.Text;

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(connect_carrier_request);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse requestResponse = (HttpWebResponse)request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                update_carrier_response_richTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = update_carrier_response_richTextBox.Text;

                //GET RESPONSE
                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("carrier-id"))
                        {

                            string carrier_id1 = currentLine.Replace("\"carrier-id\": \"", "");
                            string carrier_id = carrier_id1.Replace("\",", "");

                            MessageBox.Show("You have successfully added your" + carrier_name + "account." + "\n" + carrier_id);

                            //refresh carrier selection
                            GetCarrierAccounts();

                        }

                    }
                }

                stream.Close();

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (WebException Exception)
            {
                
                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR ADDING CARRIER ACCOUNT");

                            }
                        }
                    }
                }
            }
        }

        private void connect_order_source_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selected_order_source = connect_order_source_ComboBox.SelectedItem.ToString();

            if (selected_order_source.Contains("amazon"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_amazon;
            }
            else if (selected_order_source.Contains("brightpearl"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_brightpearl;
            }
            else if (selected_order_source.Contains("cratejoy"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_cratejoy;
            }
            else if (selected_order_source.Contains("channeladvisor"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_channeladvisor;
            }
            else if (selected_order_source.Contains("magento"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_magento;
            }
            else if (selected_order_source.Contains("volusion"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_volusion;
            }
            else if (selected_order_source.Contains("walmart"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_walmart;
            }
            else if (selected_order_source.Contains("woocommerce"))
            {
                connect_order_source_richTextBox.Enabled = true;
                connect_order_source_richTextBox.Text = SE_Order_Source_Connection.Order_Source_Connection_woocommerce;
            }

            //else if (selected_order_source.Contains(""))
            //{
            //    connect_order_source_richTextBox.Enabled = true;
            //    connect_order_source_richTextBox.Text = SE_Order_Source_Connection.;
            //}

        }

        //No route matched with those values - research error. -UPDATE- OAuth order sources are not connectable at this time :/
        private void connect_order_source_button_Click(object sender, EventArgs e)
        {

            try
            {
                string ordersource_name = connect_order_source_ComboBox.SelectedItem.ToString().Trim();

                //URI - POST
                ShipEngineUI.urlString = "https://api.shipengine.com/v-beta/connections/order_sources/" + ordersource_name;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                request.Method = "POST";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                string connect_order_source_request = connect_order_source_richTextBox.Text;

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(connect_order_source_request);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse requestResponse = (HttpWebResponse)request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                connect_order_source_response_richTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = connect_order_source_response_richTextBox.Text;

                //GET RESPONSE
                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("order_source_id"))
                        {

                            string order_source_id1 = currentLine.Replace("\"order_source_id\": \"", "");
                            string order_source_id = order_source_id1.Replace("\",", "");

                            MessageBox.Show("You have successfully added your" + ordersource_name + "account." + "\n" + order_source_id);

                            //refresh order sources
                            GetOrderSources();

                        }

                    }
                }

                stream.Close();

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR CONNECTING ORDER SOURCE");

                            }
                        }
                    }
                }
            }

        }

        private void import_orders_button_Click(object sender, EventArgs e)
        {

            try
            {

                //GET order_source_id

                string order_source_id1 = manage_order_sources_listBox.SelectedItem.ToString();
                order_source_id1 = order_source_id1.Remove(order_source_id1.IndexOf("|") + 1);
                string order_source_id = order_source_id1.Replace("|", "");

                //GET order_source_name
                string order_source_name1 = manage_order_sources_listBox.SelectedItem.ToString();
                order_source_name1 = order_source_name1.Substring(order_source_name1.LastIndexOf("|") + 1);
                string order_source_name = order_source_name1.Replace("|", "");


                //URI - POST
                ShipEngineUI.urlString = "https://api.shipengine.com/v-beta/order_sources/" + order_source_id + "/refresh";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                request.Method = "PUT";

                //API Key
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                string order_source_import_request = ShipEngineUI.urlString;

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(order_source_import_request);

                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream stream = request.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse requestResponse = (HttpWebResponse)request.GetResponse();
                stream = requestResponse.GetResponseStream();

                StreamReader parseResponse = new StreamReader(stream);
                order_source_refresh_richTextBox.Text = parseResponse.ReadToEnd();
                string responseBodyText = order_source_refresh_richTextBox.Text;

                //GET RESPONSE
                using (var reader = new StringReader(responseBodyText))
                {

                    for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                    {

                        if (currentLine.Contains("status"))
                        {

                            string order_source_status1 = currentLine.Replace("\"status\": \"", "");
                            string order_source_status = order_source_status1.Replace("\",", "");

                            MessageBox.Show("Your" + order_source_name.ToUpper() + " store is " + order_source_status.Trim(), "IMPORT ORDERS");

                        }

                    }
                }

                stream.Close();

                //CLOSE STREAM
                parseResponse.Close();
                stream.Close();

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR IMPORTING ORDERS");

                            }
                        }
                    }
                }
            }

        }

        //No route matched with those values - research error. -UPDATE- This endpoint has been depreciated. 
        private void test_order_source_buttom_Click(object sender, EventArgs e)
        {

            try
            {

                //GET order_source_id

                string order_source_id1 = manage_order_sources_listBox.SelectedItem.ToString();
                order_source_id1 = order_source_id1.Remove(order_source_id1.IndexOf("|") + 1);
                string order_source_id = order_source_id1.Replace("|", "");

                //GET order_source_name
                string order_source_name1 = manage_order_sources_listBox.SelectedItem.ToString();
                order_source_name1 = order_source_name1.Substring(order_source_name1.LastIndexOf("|") + 1);
                string order_source_name = order_source_name1.Replace("|", "");


                //URI - POST
                ShipEngineUI.urlString = "https://api.shipengine.com/v-beta/connections/order_sources/" + order_source_name.Trim() + "/" + order_source_id + "/test";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ShipEngineUI.urlString);

                request.Method = "GET";

                //API Key
                //ADD API KEY TO HEADER
                request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //RESPONSE
                HttpWebResponse responseObjectGet = null;
                responseObjectGet = (HttpWebResponse)request.GetResponse();
                string streamResponse = null;

                if (responseObjectGet.StatusCode == HttpStatusCode.NoContent)
                {

                    MessageBox.Show("Connection to Order Source was successful", "TEST ORDER SOURCE");

                }
                else if (responseObjectGet.StatusCode != HttpStatusCode.NoContent)
                {

                    MessageBox.Show("Connection to Order Source was not successful " + responseObjectGet.StatusCode.ToString(), "TEST ORDER SOURCE FAILED");

                }

                using (Stream stream = responseObjectGet.GetResponseStream())
                {
                    StreamReader responseRead = new StreamReader(stream);
                    streamResponse = responseRead.ReadToEnd();

                    using (var reader = new StringReader(streamResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                        }
                    }
                    stream.Close();

                    //CLOSE STREAM
                   
                    stream.Close();
                }
               
            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "TEST ORDER SOURCE FAILED");

                            }
                        }
                    }
                }
            }
        }

        private void ordersource_activation_button_Click(object sender, EventArgs e)
        {



        }

        private void refresh_orders_button_Click(object sender, EventArgs e)
        {
            sales_order_ListBox.Items.Clear();

            GetSalesOrders();

        }

        private void add_funds_account_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            GetCarrierBalance();

            purchase_postage_numericUpDown_ValueChanged(sender, e);

        }

        private void purchase_postage_button_Click(object sender, EventArgs e)
        {
            //GET CARRIER ID
            string carrier_id1 = add_funds_account_ComboBox.SelectedItem.ToString();
            carrier_id1 = carrier_id1.Remove(carrier_id1.IndexOf("|") + 1);
            string carrier_id = carrier_id1.Replace("|", "");

            try
            {
                
                //URI - POST
                WebRequest purchase_postage_request = WebRequest.Create("https://api.shipengine.com/v1/carriers/" + carrier_id + "/add_funds");
                purchase_postage_request.Method = "PUT";

                //API Key
                purchase_postage_request.Headers.Add("API-key", ShipEngineUI.apiKey);

                //POST REQUEST
                string purchase_postage_requestBody = "" +
                    "{\r\n" +
                    "  \"currency\": \"usd\"," +
                    "\r\n  \"amount\": " + purchase_postage_numericUpDown.Value +
                    "\r\n}";

                ASCIIEncoding purchase_postage_encoding = new ASCIIEncoding();
                byte[] purchase_postage_data = purchase_postage_encoding.GetBytes(purchase_postage_requestBody);

                purchase_postage_request.ContentType = "application/json";
                purchase_postage_request.ContentLength = purchase_postage_data.Length;

                Stream purchase_postage_stream = purchase_postage_request.GetRequestStream();

                //Documents path REQUEST LOG
                string purchase_postage_docPath = @"..\..\Resources\Logs";


                purchase_postage_stream.Write(purchase_postage_data, 0, purchase_postage_data.Length);
                purchase_postage_stream.Close();

                WebResponse purchase_postage_request_response = purchase_postage_request.GetResponse();
                purchase_postage_stream = purchase_postage_request_response.GetResponseStream();

            }
            catch (WebException Exception)
            {

                using (WebResponse ShipEngineErrorResponse = Exception.Response)
                {
                    HttpWebResponse ShipEngineResponse = (HttpWebResponse)ShipEngineErrorResponse;
                    Console.WriteLine("Error code: {0}", ShipEngineResponse.StatusCode);
                    using (Stream parseResponse1 = ShipEngineErrorResponse.GetResponseStream())

                    using (var reader = new StreamReader(parseResponse1))
                    {

                        for (string currentLine = reader.ReadLine(); currentLine != null; currentLine = reader.ReadLine())
                        {

                            if (currentLine.Contains("message") == true)
                            {

                                string ShipEngineErrorBody1 = currentLine.Replace("\"message\": \"", "");
                                string ShipEngineErrorBody = ShipEngineErrorBody1.Replace("\",", "");

                                MessageBox.Show(ShipEngineErrorBody.Trim(), "ERROR PURCHASING POSTAGE");

                            }
                        }
                    }
                }
            }

            GetCarrierBalance();
        }

        private void purchase_postage_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                string current_balance = current_balance_label.Text.Replace("$", "");

                float current_balance_amount = float.Parse(current_balance) + float.Parse(purchase_postage_numericUpDown.Value.ToString());

                resulting_balance_label.Text = "$" + current_balance_amount.ToString();
            }
            catch
            {

            }
        }
    }
}