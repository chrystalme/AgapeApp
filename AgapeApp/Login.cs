using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgapeApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string username = "admin";
            string password = "admin@123";
            if (txt_Username.Text == username && txt_pass.Text == password)
            {
                MainForm mainform = new MainForm();
                mainform.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username and Password Incorrect", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
    }
}
