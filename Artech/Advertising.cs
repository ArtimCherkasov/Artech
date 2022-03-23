using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Artech
{
    public partial class Advertising : Form
    {
        public Advertising()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 FR2 = new Form2();
            this.Hide();
            FR2.ShowDialog();
        }
    }
}
