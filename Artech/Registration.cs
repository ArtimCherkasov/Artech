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
    public partial class Registration : Form
    {
        DB dataBase = new DB();
        public Registration()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DB dataBase = new DB();

            var login = textBox1.Text;
            var pass = textBox2.Text;

            string guerystring = $"insert into register(login_user, password_user) values ('{login}' , '{pass}')";

            SqlCommand command = new SqlCommand(guerystring, dataBase.getConnection());

            dataBase.openConnection();

            if(command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Аккаунт успешно создан", "Успех!");
                Authorization aut = new Authorization();
                this.Hide();
                aut.ShowDialog();
            }

            else
            {
                MessageBox.Show("Аккаунт не зарегистрирован!");
            }
            dataBase.closeConnection();
        }

        private Boolean checkuser()
        {
            var loginUser = textBox1.Text;
            var passUser = textBox2.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string guerystring = $"select id, login_user, password_user from register where login_user = '{loginUser}' and password_user = '{passUser}'";

            SqlCommand command = new SqlCommand(guerystring, dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                MessageBox.Show("Пользователь уже существует");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
            textBox1.MaxLength = 50;
            textBox2.MaxLength = 50;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Authorization Autho = new Authorization();
            Autho.ShowDialog();
            this.Hide();
        }
    }
}
