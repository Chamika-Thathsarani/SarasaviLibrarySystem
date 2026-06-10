using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            LoadUsers();

            GenerateUserNumber();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Enter User Name");
                txtName.Focus();
                return;
            }

            

            if (txtNIC.Text.Trim() == "")
            {
                MessageBox.Show("Enter NIC");
                txtNIC.Focus();
                return;
            }

            if (txtNIC.Text.Length < 12)
            {
                MessageBox.Show("Invalid NIC");
                txtNIC.Focus();
                return;
            }

            if (!long.TryParse(txtNIC.Text, out _))
            {
                MessageBox.Show("Invalid NIC");
                return;
            }

            if (txtAddress.Text.Trim() == "")
            {
                MessageBox.Show("Enter Address");
                txtAddress.Focus();
                return;
            }
            
            if (cmbSex.SelectedIndex == -1)
            {
                MessageBox.Show("Select Sex");
                cmbSex.Focus();
                return;
            }

            if (cmbType.SelectedIndex == -1)
            {
                MessageBox.Show("Select User Type");
                cmbType.Focus();
                return;
            }
         
            try
            {               
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

                string checkNIC =
                "SELECT COUNT(*) FROM Users WHERE NIC=@NIC";

                SqlCommand nicCmd =
                new SqlCommand(checkNIC, con);

                nicCmd.Parameters.AddWithValue(
                "@NIC",
                txtNIC.Text);

                int nicCount =
                Convert.ToInt32(nicCmd.ExecuteScalar());

                if (nicCount > 0)
                {
                    MessageBox.Show("NIC Already Exists");

                    con.Close();

                    return;
                }

                string query = "INSERT INTO Users(UserNumber, Name, Sex, NIC, Address, UserType) VALUES(@UserNumber, @Name, @Sex, @NIC, @Address, @UserType)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserNumber", txtUserNumber.Text);
            cmd.Parameters.AddWithValue("@Name", txtName.Text);
            cmd.Parameters.AddWithValue("@Sex", cmbSex.Text);
            cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
            cmd.Parameters.AddWithValue("@UserType", cmbType.Text);

            cmd.ExecuteNonQuery();

            MessageBox.Show("User Saved Successfully");

            con.Close();

            LoadUsers();

            ClearFields();

            GenerateUserNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GenerateUserNumber()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();

                con.Open();

                string query = @"
        SELECT ISNULL(MAX(UserID),0) + 1
        FROM Users";

                SqlCommand cmd = new SqlCommand(query, con);

                int number = Convert.ToInt32(cmd.ExecuteScalar());

                txtUserNumber.Text = "U" + number.ToString("0000");

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadUsers()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = "SELECT * FROM Users";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvUsers.DataSource = dt;

            con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearFields()
        {
            txtUserID.Clear();

            txtName.Clear();

            txtNIC.Clear();

            txtAddress.Clear();

            cmbSex.SelectedIndex = -1;

            cmbType.SelectedIndex = -1;

            GenerateUserNumber();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text == "")
            {
                MessageBox.Show("Select User First");
                return;
            }

            try
            {
                SqlConnection con = DBConnection.GetConnection();

                con.Open();

                string query = @"
                    UPDATE Users
                    SET
                        Name=@Name,
                        Sex=@Sex,
                        NIC=@NIC,
                        Address=@Address,
                        UserType=@UserType
                    WHERE UserID=@UserID";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@UserID", txtUserID.Text);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Sex", cmbSex.Text);
                cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@UserType", cmbType.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("User Updated Successfully");

                con.Close();

                LoadUsers();

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

                txtUserID.Text = row.Cells["UserID"].Value.ToString();

                txtUserNumber.Text = row.Cells["UserNumber"].Value.ToString();

                txtName.Text = row.Cells["Name"].Value.ToString();

                cmbSex.Text = row.Cells["Sex"].Value.ToString();

                txtNIC.Text = row.Cells["NIC"].Value.ToString();

                txtAddress.Text = row.Cells["Address"].Value.ToString();

                cmbType.Text = row.Cells["UserType"].Value.ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text == "")
            {
                MessageBox.Show("Select User First");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are You Sure?",
                "Delete",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    SqlConnection con = DBConnection.GetConnection();

                    con.Open();

                    // CHECK LOANS

                    string loanCheck =
                    "SELECT COUNT(*) FROM Loans WHERE UserID=@UserID";

                    SqlCommand loanCmd =
                    new SqlCommand(loanCheck, con);

                    loanCmd.Parameters.AddWithValue(
                    "@UserID",
                    txtUserID.Text);

                    int loanCount =
                    Convert.ToInt32(loanCmd.ExecuteScalar());

                    if (loanCount > 0)
                    {
                        MessageBox.Show(
                        "Cannot Delete User. User Has Loan Records.");

                        con.Close();

                        return;
                    }

                    // DELETE USER

                    string query =
                    "DELETE FROM Users WHERE UserID=@UserID";

                    SqlCommand cmd =
                    new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue(
                    "@UserID",
                    txtUserID.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User Deleted Successfully");

                    con.Close();

                    LoadUsers();

                    ClearFields();

                    GenerateUserNumber();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmbSex_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}