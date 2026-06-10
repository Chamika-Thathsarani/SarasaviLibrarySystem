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
    public partial class InquiryForm : Form
    {
        public InquiryForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbSearch.SelectedIndex == -1)
            {
                MessageBox.Show("Select Search Type");
                cmbSearch.Focus();
                return;
            }

            if (txtSearch.Text.Trim() == "")
            {
                MessageBox.Show("Enter Search Text");
                txtSearch.Focus();
                return;
            }
            

            try
            {
                SqlConnection con = DBConnection.GetConnection();

            con.Open();

            string condition = "";

            // =====================================
            // SEARCH TYPE
            // =====================================

            if (cmbSearch.Text == "Book Number")
            {
                condition = "Books.BookNumber LIKE @Search";
            }
            else if (cmbSearch.Text == "Title")
            {
                condition = "Books.Title LIKE @Search";
            }
            else if (cmbSearch.Text == "Author")
            {
                condition = "Books.Author LIKE @Search";
            }

            // =====================================
            // MAIN QUERY
            // =====================================

            string query = @"
            SELECT
                Books.BookNumber,
                Books.Title,
                Books.Author,
                Books.Publisher,

                COUNT(Copies.CopyID) AS TotalCopies,

                SUM(CASE WHEN Copies.Status='Available'
                    THEN 1 ELSE 0 END) AS AvailableCopies,

                SUM(CASE WHEN Copies.Status='Borrowed'
                    THEN 1 ELSE 0 END) AS BorrowedCopies,

                SUM(CASE WHEN Copies.Status='Reserved'
                    THEN 1 ELSE 0 END) AS ReservedCopies,

                CASE
                    WHEN SUM(CASE WHEN Copies.IsReference = 1 THEN 1 ELSE 0 END) > 0
                    THEN 'Reference'
                    ELSE 'Borrowable'
                END AS ReferenceStatus

            FROM Books

            INNER JOIN Copies
            ON Books.BookID = Copies.BookID

            WHERE " + condition + @"

            GROUP BY
                Books.BookNumber,
                Books.Title,
                Books.Author,
                Books.Publisher";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue(
            "@Search",
            "%" + txtSearch.Text + "%");

            SqlDataAdapter da =
            new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dgvInquiry.DataSource = dt;

            dgvInquiry.Visible = true;

            con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            dgvInquiry.DataSource = null;

            dgvInquiry.Visible = false;

            cmbSearch.SelectedIndex = 0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSuggestions();
        }

        private void dgvInquiry_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InquiryForm_Load(object sender, EventArgs e)
        {
            cmbSearch.SelectedIndex = -1;

            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

            LoadSuggestions();

            // Hide DataGridView at startup
            dgvInquiry.Visible = false;
        }

        private void LoadSuggestions()
        {
            try
            {
                if (cmbSearch.SelectedIndex == -1)
                {
                    return;
                }

                SqlConnection con = DBConnection.GetConnection();

                con.Open();

                string column = "";

                // Decide which column to search
                if (cmbSearch.Text == "Book Number")
                {
                    column = "BookNumber";
                }
                else if (cmbSearch.Text == "Title")
                {
                    column = "Title";
                }
                else if (cmbSearch.Text == "Author")
                {
                    column = "Author";
                }

                if (column == "")
                {
                    con.Close();
                    return;
                }

                string query = "SELECT DISTINCT " + column + " FROM Books";

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader dr = cmd.ExecuteReader();

                AutoCompleteStringCollection collection =
                    new AutoCompleteStringCollection();

                while (dr.Read())
                {
                    collection.Add(dr[0].ToString());
                }

                txtSearch.AutoCompleteCustomSource = collection;

                dr.Close();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
