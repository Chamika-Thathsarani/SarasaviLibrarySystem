using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SarasaviLibrarySystem
{
    public partial class LoanForm : Form
    {
        public LoanForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        public void LoadUsers()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = "SELECT UserID, Name FROM Users";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            cmbUser.DataSource = dt;

            cmbUser.DisplayMember = "Name";

            cmbUser.ValueMember = "UserID";

            con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void LoadCopies()
        {
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = "SELECT CopyID, CopyNumber FROM Copies WHERE Status IN ('Available','Reserved')\r\nAND IsReference = 0";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            cmbCopy.DataSource = dt;

            cmbCopy.DisplayMember = "CopyNumber";

            cmbCopy.ValueMember = "CopyID";

            cmbCopy.SelectedIndex = -1;

            con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadLoans()
        {   
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = @"SELECT LoanID,
                            Users.Name,
                            Copies.CopyNumber,
                            LoanDate,
                            ReturnDate
                     FROM Loans
                     INNER JOIN Users
                     ON Loans.UserID = Users.UserID
                     INNER JOIN Copies
                     ON Loans.CopyID = Copies.CopyID";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvLoans.DataSource = dt;

            con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearFields()
        {
            cmbUser.SelectedIndex = -1;

            cmbCopy.SelectedIndex = -1;
        }

        private void btnLoan_Click(object sender, EventArgs e)
        {
            if (cmbUser.SelectedIndex == -1)
            {
                MessageBox.Show("Select User");
                cmbUser.Focus();
                return;
            }

            if (cmbCopy.SelectedIndex == -1)
            {
                MessageBox.Show("Select Copy");
                cmbCopy.Focus();
                return;
            }

            if (dtReturn.Value <= dtLoan.Value)
            {
                MessageBox.Show(
                "Return Date Must Be After Loan Date");

                dtReturn.Focus();

                return;
            }
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            int userID = Convert.ToInt32(cmbUser.SelectedValue);

            int copyID = Convert.ToInt32(cmbCopy.SelectedValue);

                string overdueQuery = @"
                SELECT COUNT(*)
                FROM Loans
                WHERE UserID=@UserID
                AND ReturnDate < GETDATE()";

                SqlCommand overdueCmd =
                new SqlCommand(overdueQuery, con);

                overdueCmd.Parameters.AddWithValue(
                "@UserID",
                userID);

                int overdue =
                Convert.ToInt32(
                overdueCmd.ExecuteScalar());

                if (overdue > 0)
                {
                    MessageBox.Show(
                    "User Has Overdue Books");

                    con.Close();

                    return;
                }

                // =====================================
                // CHECK IF COPY IS RESERVED
                // =====================================

            string reserveCheck =
            "SELECT Status FROM Copies WHERE CopyID=@CopyID";

            SqlCommand rc = new SqlCommand(reserveCheck, con);
            rc.Parameters.AddWithValue("@CopyID", copyID);

            string reserveStatus = rc.ExecuteScalar().ToString();

            if (reserveStatus == "Reserved")
            {
                // check reservation owner
                string userCheck = @"
                    SELECT TOP 1 UserID
                    FROM Reservations
                    WHERE BookID = (SELECT BookID FROM Copies WHERE CopyID=@CopyID)
                    ORDER BY ReservationDate";

                SqlCommand uc = new SqlCommand(userCheck, con);
                uc.Parameters.AddWithValue("@CopyID", copyID);

                int reservedUserID = Convert.ToInt32(uc.ExecuteScalar());

                if (reservedUserID != userID)
                {
                    MessageBox.Show("This book is reserved for another user");
                    con.Close();
                    return;
                }
            }

            // =====================================
            // CHECK COPY STATUS
            // =====================================

            string statusQuery =
            "SELECT Status FROM Copies WHERE CopyID=@CopyID";

            SqlCommand statusCmd =
            new SqlCommand(statusQuery, con);

            statusCmd.Parameters.AddWithValue("@CopyID", copyID);

            string status =
            statusCmd.ExecuteScalar().ToString();

                if (status == "Borrowed")
                {
                    MessageBox.Show(
                    "Copy Already Borrowed");

                    con.Close();

                    return;
                }

                // =====================================
                // CHECK USER TYPE
                // =====================================

                string userTypeQuery =
            "SELECT UserType FROM Users WHERE UserID=@UserID";

            SqlCommand typeCmd =
            new SqlCommand(userTypeQuery, con);

            typeCmd.Parameters.AddWithValue("@UserID", userID);

            string userType =
            typeCmd.ExecuteScalar().ToString();

            if (userType == "Visitor")
            {
                MessageBox.Show(
                "Registered Visitors Cannot Borrow Books");

                con.Close();

                return;
            }

                // CHECK MAXIMUM 5 BOOKS

                string checkLoan = "SELECT COUNT(*) FROM Loans WHERE UserID=@UserID";

            SqlCommand checkCmd = new SqlCommand(checkLoan, con);

            checkCmd.Parameters.AddWithValue("@UserID", userID);

            int loanCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (loanCount >= 5)
            {
                MessageBox.Show("Maximum 5 Books Allowed");

                con.Close();

                return;
            }

            // CHECK REFERENCE BOOK

            string refQuery = "SELECT IsReference FROM Copies WHERE CopyID=@CopyID";

            SqlCommand refCmd = new SqlCommand(refQuery, con);

            refCmd.Parameters.AddWithValue("@CopyID", copyID);

            bool isReference = Convert.ToBoolean(refCmd.ExecuteScalar());

            if (isReference == true)
            {
                MessageBox.Show("Reference Books Cannot Be Borrowed");

                con.Close();

                return;
            }

            // SAVE LOAN

            string query = @"INSERT INTO Loans(UserID, CopyID, LoanDate, ReturnDate)
                     VALUES(@UserID, @CopyID, @LoanDate, @ReturnDate)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserID", userID);

            cmd.Parameters.AddWithValue("@CopyID", copyID);

            cmd.Parameters.AddWithValue("@LoanDate", dtLoan.Value);

            cmd.Parameters.AddWithValue("@ReturnDate", dtReturn.Value);

            cmd.ExecuteNonQuery();

            
                // REMOVE RESERVATION AFTER LOAN

                string deleteReservation = @"
                DELETE FROM Reservations
                WHERE UserID=@UserID
                AND BookID =
                (
                    SELECT BookID
                    FROM Copies
                    WHERE CopyID=@CopyID
                )";

                SqlCommand delCmd =
                new SqlCommand(deleteReservation, con);

                delCmd.Parameters.AddWithValue(
                "@UserID",
                userID);

                delCmd.Parameters.AddWithValue(
                "@CopyID",
                copyID);

                delCmd.ExecuteNonQuery();


                // UPDATE COPY STATUS

            string updateQuery = "UPDATE Copies SET Status='Borrowed' WHERE CopyID=@CopyID";

            SqlCommand updateCmd = new SqlCommand(updateQuery, con);

            updateCmd.Parameters.AddWithValue("@CopyID", copyID);

            updateCmd.ExecuteNonQuery();

            MessageBox.Show("Loan Added Successfully");

            con.Close();

            LoadLoans();

            LoadCopies();

            ClearFields();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoanForm_Load(object sender, EventArgs e)
        {
            LoadUsers();

            LoadCopies();

            LoadLoans();

            dtLoan.Value = DateTime.Now;

            dtReturn.Value = DateTime.Now.AddDays(14);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void cmbCopy_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
