using Assignment4DB.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment4DB
{
    public partial class TransactionForm : Form
    {
        //get the account id from the account form
        public Account accountId;
        //ublic User user;

        public TransactionForm(Account accountId)
        {
            InitializeComponent();
            this.accountId = accountId;
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }

        private void TransactionForm_Load(object sender, EventArgs e)
        {
            //on form load, display the using accountid show the transaction history
            using (ASSN4DBContext db = new ASSN4DBContext())
            {
                //show transaction history in gridbox, show all transactions of particular accountid
                foreach (var transaction in db.TransactionHistories.Where(t => t.AccountId == accountId.AccountId).ToList())
                {
                    dataGridView1.Rows.Add(transaction.TransactionDate, transaction.Amount, transaction.Description);
                }
            }
        }

        //onclick on gridrow, show the details in respective textboxes
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                dateTxt.Text = row.Cells[0].Value.ToString();
                amntTxt.Text = row.Cells[1].Value.ToString();
                descTxt.Text = row.Cells[2].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Attempt to parse the date, amount, and get the description from the user inputs
            bool isDateParsed = DateTime.TryParse(dateTxt.Text, out DateTime date);
            bool isAmountParsed = decimal.TryParse(amntTxt.Text, out decimal amount);
            string description = descTxt.Text;

            // Check if both date and amount were successfully parsed
            if (!isDateParsed)
            {
                MessageBox.Show("Invalid date format.");
                return; // Exit the method if the date is not valid
            }

            if (!isAmountParsed)
            {
                MessageBox.Show("Invalid amount format.");
                return; // Exit the method if the amount is not valid
            }

            // Proceed to add the transaction to the transaction history
            using (ASSN4DBContext db = new ASSN4DBContext())
            {

                decimal currentBalance = Convert.ToDecimal(accountId.CurrentBalance);
                if (string.IsNullOrEmpty(amntTxt.Text) || string.IsNullOrEmpty(descTxt.Text))
                {
                    MessageBox.Show("Please fill all the fields");
                    return;
                }else if (amount < 0)
                {
                    MessageBox.Show("Amount cannot be negative");
                    return;
                }
                //if amount is greater than current balance
                else if (amount > currentBalance)
                {
                    MessageBox.Show("Amount cannot be greater than current balance");
                    return;
                }
                else
                {
                    // Create a new transaction history object
                    TransactionHistory transaction = new TransactionHistory
                    {
                        AccountId = accountId.AccountId,
                        TransactionDate = date,
                        Amount = amount,
                        Description = description
                    };

                    // Add the transaction to the database
                    db.TransactionHistories.Add(transaction);
                    db.SaveChanges();
                    updateGrid();
                    MessageBox.Show("Transaction Added");
                    dateTxt.Text = "";
                    amntTxt.Text = "";
                    descTxt.Text = "";
                }

            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //update the transaction history
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (string.IsNullOrEmpty(amntTxt.Text) || string.IsNullOrEmpty(descTxt.Text))
                {
                    MessageBox.Show("Please fill all the fields");
                    return;
                }
                else if (decimal.Parse(amntTxt.Text) < 0)
                {
                    MessageBox.Show("Amount cannot be negative");
                    return;
                }
                else if (decimal.Parse(amntTxt.Text) > Convert.ToDecimal(accountId.CurrentBalance))
                {
                    MessageBox.Show("Amount cannot be greater than current balance");
                    return;
                }
                else
                {
                    using (ASSN4DBContext db = new ASSN4DBContext())
                    {
                        DataGridViewRow row = dataGridView1.SelectedRows[0];
                        TransactionHistory transaction = db.TransactionHistories.Where(t => t.AccountId == accountId.AccountId && t.TransactionDate == (DateTime)row.Cells[0].Value).FirstOrDefault();
                        transaction.TransactionDate = DateTime.Parse(dateTxt.Text);
                        transaction.Amount = decimal.Parse(amntTxt.Text);
                        transaction.Description = descTxt.Text;
                        db.SaveChanges();
                        updateGrid();
                        MessageBox.Show("Transaction Updated");
                        dateTxt.Text = "";
                        amntTxt.Text = "";
                        descTxt.Text = "";
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please select a transaction to update");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //delete the transaction history
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult=MessageBox.Show("Are you sure you want to delete this transaction?","Info", MessageBoxButtons.YesNo);
                if (DialogResult == DialogResult.No)
                {
                    return;
                }
                else
                {
                    using (ASSN4DBContext db = new ASSN4DBContext())
                    {
                        DataGridViewRow row = dataGridView1.SelectedRows[0];
                        TransactionHistory transaction = db.TransactionHistories.Where(t => t.AccountId == accountId.AccountId && t.TransactionDate == (DateTime)row.Cells[0].Value).FirstOrDefault();
                        db.TransactionHistories.Remove(transaction);
                        db.SaveChanges();
                        updateGrid();
                        MessageBox.Show("Transaction Deleted");
                        dateTxt.Text = "";
                        amntTxt.Text = "";
                        descTxt.Text = "";
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please select a transaction to delete");
            }
        }

        private void updateGrid()
        {
            //update the gridbox
            using (ASSN4DBContext db = new ASSN4DBContext())
            {
                dataGridView1.Rows.Clear();
                foreach (var transaction in db.TransactionHistories.Where(t => t.AccountId == accountId.AccountId).ToList())
                {
                    dataGridView1.Rows.Add(transaction.TransactionDate, transaction.Amount, transaction.Description);
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            using (ASSN4DBContext db = new ASSN4DBContext())
            {
                // Fetch the user based on the UserId
                User user = db.Users.Where(u => u.Id == accountId.UserId).FirstOrDefault();

                if (user != null) // Ensure user is not null
                {
                    // Pass the user object to AccountForm
                    AccountForm accountForm = new AccountForm(user);
                    accountForm.Show();
                    this.Hide();
                }
                else
                {
                    return;
                }
            }


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
