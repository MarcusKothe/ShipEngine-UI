using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipEngine_UI
{
    public partial class apiKeyPrompt : Form
    {
        public apiKeyPrompt()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            //SET API KEY
            ShipEngineUI.apiKey = apiKeyTextBox.Text;

            ShippingForm shippingForm = new ShippingForm();
            shippingForm.ShowDialog();

            this.Hide();

        }

        private void exit_button_Click(object sender, EventArgs e)
        {

            this.Close();

        }

    }
}
