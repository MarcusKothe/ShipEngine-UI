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

        private void apiKeyPrompt_Load(object sender, EventArgs e)
        {


        }
       
        private void doneButton_Click(object sender, EventArgs e)
        {

            ShipEngineUI.apiKey = apiKeyTextBox.Text;

            this.Hide();

            shippingForm shippingForm = new shippingForm();
            shippingForm.ShowDialog();

        }

    }
}
