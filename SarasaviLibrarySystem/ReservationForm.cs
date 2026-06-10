using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class ReservationForm : Form
    {
        public ReservationForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
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

            cmbUser.SelectedIndex = -1;

            con.Close();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

        public void LoadBooks()
        {
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = @"
                SELECT DISTINCT Books.BookID, Books.Title
                FROM Books
                INNER JOIN Copies
                ON Books.BookID = Copies.BookID
                WHERE Copies.IsReference = 0";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            cmbBook.DataSource = dt;

            cmbBook.DisplayMember = "Title";

            cmbBook.ValueMember = "BookID";

            cmbBook.SelectedIndex = -1;

            con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadReservations()
        {
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string query = @"
                SELECT Reservations.ReservationID,
                       Users.Name,
                       Books.Title,
                       ReservationDate
                FROM Reservations
                INNER JOIN Users
                ON Reservations.UserID = Users.UserID
                INNER JOIN Books
                ON Reservations.BookID = Books.BookID";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvReservations.DataSource = dt;

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

            cmbBook.SelectedIndex = -1;
        }

        private void ReservationForm_Load(object sender, EventArgs e)
        {
            LoadUsers();

            LoadBooks();

            LoadReservations();

            dtReserve.Value = DateTime.Now;
        }

        private void btnReserve_Click(object sender, EventArgs e)
        {
            if (cmbUser.SelectedIndex == -1)
            {
                MessageBox.Show("Select User");
                cmbUser.Focus();
                return;
            }

            if (cmbBook.SelectedIndex == -1)
            {
                MessageBox.Show("Select Book");
                cmbBook.Focus();
                return;
            }
            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            int userID = Convert.ToInt32(cmbUser.SelectedValue);

            int bookID = Convert.ToInt32(cmbBook.SelectedValue);

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
                // CHECK USER TYPE
                // =====================================

                string typeQuery =
            "SELECT UserType FROM Users WHERE UserID=@UserID";

            SqlCommand typeCmd =
            new SqlCommand(typeQuery, con);

            typeCmd.Parameters.AddWithValue("@UserID", userID);

            string userType =
            typeCmd.ExecuteScalar().ToString();

            if (userType == "Visitor")
            {
                MessageBox.Show(
                "Visitors Cannot Reserve Books");

                con.Close();

                return;
            }

            // =====================================
            // CHECK AVAILABLE COPIES
            // =====================================

            string checkQuery = @"
                SELECT COUNT(*)
                FROM Copies
                WHERE BookID=@BookID
                AND Status='Available'
                AND IsReference=0";

            SqlCommand checkCmd =
            new SqlCommand(checkQuery, con);

            checkCmd.Parameters.AddWithValue("@BookID", bookID);

            int available =
            Convert.ToInt32(checkCmd.ExecuteScalar());

            if (available > 0)
            {
                MessageBox.Show(
                "Book Is Available. Reservation Not Needed.");

                con.Close();

                return;
            }

            // =====================================
            // CHECK DUPLICATE RESERVATION
            // =====================================

            string duplicateQuery = @"
                SELECT COUNT(*)
                FROM Reservations
                WHERE UserID=@UserID
                AND BookID=@BookID";

            SqlCommand duplicateCmd =
            new SqlCommand(duplicateQuery, con);

            duplicateCmd.Parameters.AddWithValue("@UserID", userID);

            duplicateCmd.Parameters.AddWithValue("@BookID", bookID);

            int duplicate =
            Convert.ToInt32(duplicateCmd.ExecuteScalar());

            if (duplicate > 0)
            {
                MessageBox.Show(
                "User Already Reserved This Book");

                con.Close();

                return;
            }

            // =====================================
            // CHECK MAXIMUM RESERVATIONS
            // =====================================

            string maxQuery = @"
                SELECT COUNT(*)
                FROM Reservations
                WHERE UserID=@UserID";

            SqlCommand maxCmd =
            new SqlCommand(maxQuery, con);

            maxCmd.Parameters.AddWithValue("@UserID", userID);

            int reserveCount =
            Convert.ToInt32(maxCmd.ExecuteScalar());

            if (reserveCount >= 3)
            {
                MessageBox.Show(
                "Maximum 3 Reservations Allowed");

                con.Close();

                return;
            }

            // =====================================
            // SAVE RESERVATION
            // =====================================

            string query = @"
                INSERT INTO Reservations
                (UserID, BookID, ReservationDate)
                VALUES
                (@UserID, @BookID, @ReservationDate)";

            SqlCommand cmd =
            new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserID", userID);

            cmd.Parameters.AddWithValue("@BookID", bookID);

            cmd.Parameters.AddWithValue(
            "@ReservationDate",
            dtReserve.Value);

            cmd.ExecuteNonQuery();

            MessageBox.Show(
            "Reservation Added Successfully");

            con.Close();

            LoadReservations();

            ClearFields();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void dgvReservations_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
