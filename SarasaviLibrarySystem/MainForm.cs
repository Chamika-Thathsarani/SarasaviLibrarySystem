using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }



        private void btnUser_Click(object sender, EventArgs e)
        {
            UserForm uf = new UserForm();
            uf.Show();
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            BookForm bf = new BookForm();
            bf.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


        private void btnInquiry_Click(object sender, EventArgs e)
        {
            InquiryForm inf = new InquiryForm();

            inf.Show();
        }

        private void btnReservation_Click(object sender, EventArgs e)
        {
            ReservationForm rf = new ReservationForm();

            rf.Show();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            ReturnForm rf = new ReturnForm();

            rf.Show();
        }

        private void btnLoan_Click(object sender, EventArgs e)
        {
            LoanForm lf = new LoanForm();

            lf.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
