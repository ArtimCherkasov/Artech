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

namespace Artech
{
    public partial class Authorization : Form
    {
        DB database = new DB();

        public Authorization()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var loginUser = textBox1.Text;
            var passUser = textBox2.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string guerystring = $"select id, login_user, password_user from register where login_user = '{loginUser}' and password_user = '{passUser}'";

            SqlCommand command = new SqlCommand(guerystring, database.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                MessageBox.Show("Вы успешно зашли", "успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Advertising frm2 = new Advertising();
                this.Hide();
                frm2.ShowDialog();
                this.Show();
            }
            else
                MessageBox.Show("Ошибка");
        }

        private void Authorization_Load(object sender, EventArgs e)
        {
           
            textBox1.MaxLength = 50;
            textBox2.MaxLength = 50;
            

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Registration Reg4 = new Registration();
            Reg4.ShowDialog();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
        }
    }
}
