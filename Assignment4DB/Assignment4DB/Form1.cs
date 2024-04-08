using Assignment4DB.Models.Entities;

namespace Assignment4DB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string email = emailTxt.Text;
            string password = passwordTxt.Text;

            // Check if email and password are empty
            if (email == "" || password == "")
            {
                MessageBox.Show("Please enter email and password");
                return;
            }else
            {
                // Check if the email and password are valid
                using (ASSN4DBContext db = new ASSN4DBContext())
                {
                    // Check if the user exists in the database
                    User user = db.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
                    if (user != null)
                    {
                        MessageBox.Show("Login Successful");
                        AccountForm accountForm = new AccountForm(user);
                        accountForm.Show();
                        this.Hide();

                    }
                    else
                    {
                        MessageBox.Show("Invalid Email or Password");
                    }
                }
            }

        }
    }
}
