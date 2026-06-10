using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class ReturnForm : Form
    {
        public ReturnForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ReturnForm_Load(object sender, EventArgs e)
        {
            LoadBorrowedBooks();

            LoadReturns();
        }

        // =====================================
        // LOAD BORROWED BOOKS
        // =====================================

        public void LoadBorrowedBooks()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = @"
            SELECT Loans.LoanID,
                   Copies.CopyNumber
            FROM Loans
            INNER JOIN Copies
            ON Loans.CopyID = Copies.CopyID";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            cmbLoan.DataSource = dt;

            cmbLoan.DisplayMember = "CopyNumber";

            cmbLoan.ValueMember = "LoanID";

            cmbLoan.SelectedIndex = -1;

            cmbLoan.Text = "-- Select Book --";

            con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =====================================
        // LOAD RETURNS
        // =====================================

        public void LoadReturns()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = @"
            SELECT LoanID,
                   UserID,
                   CopyID,
                   LoanDate,
                   ReturnDate
            FROM Loans";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvReturns.DataSource = dt;

            con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =====================================
        // CLEAR FIELDS
        // =====================================

        public void ClearFields()
        {
            cmbLoan.SelectedIndex = -1;
        }

        // =====================================
        // RETURN BOOK
        // =====================================

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (cmbLoan.SelectedIndex == -1)
            {
                MessageBox.Show("Select Loan");
                cmbLoan.Focus();
                return;
            }
            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            int loanID = Convert.ToInt32(cmbLoan.SelectedValue);

            // =====================================
            // GET COPY ID
            // =====================================

            string getCopy =
            "SELECT CopyID FROM Loans WHERE LoanID=@LoanID";

            SqlCommand copyCmd =
            new SqlCommand(getCopy, con);

            copyCmd.Parameters.AddWithValue("@LoanID", loanID);

            int copyID =
            Convert.ToInt32(copyCmd.ExecuteScalar());

            // =====================================
            // CHECK RESERVATIONS
            // =====================================

            string reserveQuery = @"
            SELECT TOP 1
                   Reservations.ReservationID,
                   Users.Name
            FROM Reservations
            INNER JOIN Users
            ON Reservations.UserID = Users.UserID
            WHERE BookID =
            (
                SELECT BookID
                FROM Copies
                WHERE CopyID=@CopyID
            )
            ORDER BY ReservationDate";

            SqlCommand reserveCmd =
            new SqlCommand(reserveQuery, con);

            reserveCmd.Parameters.AddWithValue("@CopyID", copyID);

            SqlDataReader dr = reserveCmd.ExecuteReader();

            bool hasReservation = false;

            int reservationID = 0;

            string reservedUser = "";

            if (dr.Read())
            {
                hasReservation = true;

                reservationID =
                Convert.ToInt32(dr["ReservationID"]);

                reservedUser = dr["Name"].ToString();
            }

            dr.Close();

            // =====================================
            // UPDATE COPY STATUS
            // =====================================

            string status =
            hasReservation ? "Reserved" : "Available";

            string updateQuery =
            "UPDATE Copies SET Status=@Status WHERE CopyID=@CopyID";

            SqlCommand updateCmd =
            new SqlCommand(updateQuery, con);

            updateCmd.Parameters.AddWithValue("@Status", status);

            updateCmd.Parameters.AddWithValue("@CopyID", copyID);

            updateCmd.ExecuteNonQuery();

            // =====================================
            // INFORM RESERVED MEMBER
            // =====================================

            if (hasReservation)
            {
                MessageBox.Show(
                "Inform Reserved Member: " + reservedUser);

                // DELETE OLDEST RESERVATION

                string deleteReservation =
                "DELETE FROM Reservations WHERE ReservationID=@ReservationID";

                SqlCommand delCmd =
                new SqlCommand(deleteReservation, con);

                delCmd.Parameters.AddWithValue(
                "@ReservationID",
                reservationID);

                delCmd.ExecuteNonQuery();
            }

            // =====================================
            // DELETE LOAN
            // =====================================

            string deleteLoan =
            "DELETE FROM Loans WHERE LoanID=@LoanID";

            SqlCommand deleteCmd =
            new SqlCommand(deleteLoan, con);

            deleteCmd.Parameters.AddWithValue("@LoanID", loanID);

            deleteCmd.ExecuteNonQuery();

            MessageBox.Show("Book Returned Successfully");

            con.Close();

            LoadBorrowedBooks();

            LoadReturns();

            ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =====================================
        // CLEAR BUTTON
        // =====================================

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
