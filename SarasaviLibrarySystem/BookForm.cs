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
    public partial class BookForm : Form
    {
        public BookForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void chkReference_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReference.Checked)
            {
                MessageBox.Show(
                "Reference books cannot be borrowed.\nThey can only be used inside the library.",
                "Reference Book",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtClassification.Text.Trim() == "")
            {
                MessageBox.Show("Enter Classification");
                txtClassification.Focus();
                return;
            }

            if (txtTitle.Text.Trim() == "")
            {
                MessageBox.Show("Enter Book Title");
                txtTitle.Focus();
                return;
            }

            if (txtAuthor.Text.Trim() == "")
            {
                MessageBox.Show("Enter Author Name");
                txtAuthor.Focus();
                return;
            }

            if (txtPublisher.Text.Trim() == "")
            {
                MessageBox.Show("Enter Publisher");
                txtPublisher.Focus();
                return;
            }

            if (numCopies.Value <= 0)
            {
                MessageBox.Show("Copies Must Be Greater Than 0");
                numCopies.Focus();
                return;
            }

            if (numCopies.Value > 10)
            {
                MessageBox.Show("Maximum 10 Copies Allowed");
                numCopies.Focus();
                return;
            }
            if (numCopies.Value > 10)
            {
                MessageBox.Show("Maximum 10 Copies Allowed");
                return;
            }

            try
            {
            SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string duplicateBook = @"
            SELECT COUNT(*)
            FROM Books
            WHERE Title=@Title
            AND Author=@Author";

            SqlCommand duplicateCmd =
                new SqlCommand(duplicateBook, con);

                duplicateCmd.Parameters.AddWithValue(
                "@Title",
                txtTitle.Text);

                duplicateCmd.Parameters.AddWithValue(
                "@Author",
                txtAuthor.Text);

                int duplicate =
                Convert.ToInt32(
                duplicateCmd.ExecuteScalar());

                if (duplicate > 0)
                {
                    MessageBox.Show(
                    "Book Already Exists");

                    con.Close();

                    return;
                }

            string query = "INSERT INTO Books(BookNumber, Classification, Title, Author, Publisher) VALUES(@BookNumber, @Classification, @Title, @Author, @Publisher)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@BookNumber", txtBookNumber.Text);
            cmd.Parameters.AddWithValue("@Classification", txtClassification.Text);
            cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
            cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
            cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text);

            cmd.ExecuteNonQuery();

            // GET LAST INSERTED BOOK ID

            string getBookID = "SELECT MAX(BookID) FROM Books";

            SqlCommand cmdBook = new SqlCommand(getBookID, con);

            int bookID = Convert.ToInt32(cmdBook.ExecuteScalar());

            // SAVE COPIES

            for (int i = 1; i <= numCopies.Value; i++)
            {
                string copyNumber = txtBookNumber.Text + "-" + i;

                string status = "Available";

                bool isReference = chkReference.Checked;

                string copyQuery = "INSERT INTO Copies(CopyNumber, BookID, Status, IsReference) VALUES(@CopyNumber, @BookID, @Status, @IsReference)";

                SqlCommand copyCmd = new SqlCommand(copyQuery, con);

                copyCmd.Parameters.AddWithValue("@CopyNumber", copyNumber);
                copyCmd.Parameters.AddWithValue("@BookID", bookID);
                copyCmd.Parameters.AddWithValue("@Status", status);
                copyCmd.Parameters.AddWithValue("@IsReference", isReference);

                copyCmd.ExecuteNonQuery();
            }

            MessageBox.Show("Book Saved Successfully");

            con.Close();

            LoadBooks();

            ClearFields();

            GenerateBookNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        public void GenerateBookNumber()
        {
            if (txtClassification.Text == "")
            {
                return;
            }
            try
            {
             SqlConnection con = DBConnection.GetConnection();

            con.Open();

                // First letter of classification
                string classification =
                txtClassification.Text.Substring(0, 1).ToUpper();

                // Count books ONLY for this classification
                string query = @"
                SELECT COUNT(*)
                FROM Books
                WHERE BookNumber LIKE @Classification";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                "@Classification",
                classification + "%");

                int count =
                Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                // Final Book Number
                txtBookNumber.Text =
                classification + count.ToString("0000");

                con.Close();
            }
            catch (Exception ex)
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
                SELECT 
                    B.BookID,
                    B.BookNumber,
                    B.Classification,
                    B.Title,
                    B.Author,
                    B.Publisher,
                    COUNT(C.CopyID) AS TotalCopies
                FROM Books B
                LEFT JOIN Copies C ON B.BookID = C.BookID
                GROUP BY 
                    B.BookID,
                    B.BookNumber,
                    B.Classification,
                    B.Title,
                    B.Author,
                    B.Publisher";

            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvCopies.DataSource = dt;

            con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearFields()
        {
            txtBookID.Clear();

            txtBookNumber.Clear();

            txtClassification.Clear();

            txtTitle.Clear();

            txtAuthor.Clear();

            txtPublisher.Clear();

            numCopies.Value = 1;

            chkReference.Checked = false;
        }

        public void LoadCopies()
        {
            try
            {
                SqlConnection con = DBConnection.GetConnection();
            con.Open();

            string query = @"
                SELECT 
                    C.CopyID,
                    B.BookNumber,
                    B.Title,
                    C.CopyNumber,
                    C.Status,

                    CASE 
                        WHEN C.IsReference = 1 THEN 'Reference Only'
                        ELSE 'Borrowable'
                    END AS CopyType

                FROM Copies C
                INNER JOIN Books B ON B.BookID = C.BookID";

            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvCopies.DataSource = dt;

            con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BookForm_Load(object sender, EventArgs e)
        {
            LoadBooks();

            GenerateBookNumber();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void txtClassification_TextChanged(object sender, EventArgs e)
        {
            if (txtClassification.Text != "")
            {
                GenerateBookNumber();
            }
        }

        private void dgvCopies_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCopies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCopies.Rows[e.RowIndex];

                txtBookID.Text =
                row.Cells["BookID"].Value.ToString();

                txtBookNumber.Text =
                row.Cells["BookNumber"].Value.ToString();

                txtClassification.Text =
                row.Cells["Classification"].Value.ToString();

                txtTitle.Text =
                row.Cells["Title"].Value.ToString();

                txtAuthor.Text =
                row.Cells["Author"].Value.ToString();

                txtPublisher.Text =
                row.Cells["Publisher"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtBookID.Text == "")
            {
                MessageBox.Show("Select Book First");
                return;
            }

            try
            {
                SqlConnection con =
                DBConnection.GetConnection();

                con.Open();

                string query = @"
        UPDATE Books
        SET
            Classification=@Classification,
            Title=@Title,
            Author=@Author,
            Publisher=@Publisher
        WHERE BookID=@BookID";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                "@BookID",
                txtBookID.Text);

                cmd.Parameters.AddWithValue(
                "@Classification",
                txtClassification.Text);

                cmd.Parameters.AddWithValue(
                "@Title",
                txtTitle.Text);

                cmd.Parameters.AddWithValue(
                "@Author",
                txtAuthor.Text);

                cmd.Parameters.AddWithValue(
                "@Publisher",
                txtPublisher.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show(
                "Book Updated Successfully");

                con.Close();

                LoadBooks();

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
