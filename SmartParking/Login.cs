using System;
using System.Windows.Forms;

namespace SmartParking
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

      


        private void loginBtn_Click_1(object sender, EventArgs e)
        {
            User user = new User();
            if (user.UserName == usernameBox.Text && user.Password == passwordBox.Text)
            {
                this.Hide();
                ParkingLot engine = new ParkingLot();
                engine.ShowDialog();
                this.Close();

            }
            else
            {
                label.Text = "Wrong Username or Password";
                usernameBox.Text = passwordBox.Text = "";
            }
        }


        private void passwordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
