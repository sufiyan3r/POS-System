using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            List<string[]> rows = File.ReadAllLines("Data.csv").Select(x => x.Split(',')).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add(" ");
            dt.Columns.Add("  ");
            dt.Columns.Add("   ");
            dt.Columns.Add("    ");
            rows.ForEach(x => {
                dt.Rows.Add(x);
            });
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}
