using Lab1.DataTable;
using Lab1.Grammar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private BindingList<string> _data = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();

            //_data.Add("test 1");
            //_data.Add("test 1");
            //_data.Add("test 1");

            //this.dataGridView1.DataSource = _data;
            var column = new GrammarColumn();
            column.Name = "A";
            this.dataGridView1.Columns.Add(column);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var res = Calculator.Evaluate(textBox1.Text);
            label1.Text = res.ToString();
        }
        const string template = "{0}: HiddenValue:{1}, Value{2}";
        const string changeEvent = "ChangeEvent";
        const string doubleClickEvent = "DoubleClickEvent";
        bool isEnter = false;

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //var cell = (GrammarCell)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //Debug.WriteLine(string.Format(template, changeEvent + " Before", cell.HiddenValue, cell.Value));

            //cell.HiddenValue = new string(cell.Value.ToString());
            //cell.Value = Calculator.Evaluate(cell.HiddenValue);

            //Debug.WriteLine(string.Format(template, changeEvent + " After", cell.HiddenValue, cell.Value));

            //se
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var cell = (GrammarCell)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            Debug.WriteLine(string.Format(template, doubleClickEvent + " Before", cell.HiddenValue, cell.Value));

            cell.Value = cell.HiddenValue;
            //cell.EditedFormattedValue = cell.HiddenValue;

            Debug.WriteLine(string.Format(template, doubleClickEvent + " After", cell.HiddenValue, cell.Value));
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //isEnter = true;
            //Debug.WriteLine("Enter " + isEnter);
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //isEnter = false;
            //Debug.WriteLine("Leave " + isEnter);


            //var cell = (GrammarCell)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //Debug.WriteLine(string.Format(template, changeEvent + " Before", cell.HiddenValue, cell.Value));

            //cell.HiddenValue = cell.EditedFormattedValue.ToString();
            //cell.Value = Calculator.Evaluate(cell.HiddenValue);

            //Debug.WriteLine(string.Format(template, changeEvent + " After", cell.HiddenValue, cell.Value));
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = (GrammarCell)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            Debug.WriteLine(string.Format(template, changeEvent + " Before", cell.HiddenValue, cell.Value));

            cell.HiddenValue = cell.Value.ToString();
            cell.Value = Calculator.Evaluate(cell.HiddenValue);

            Debug.WriteLine(string.Format(template, changeEvent + " After", cell.HiddenValue, cell.Value));

        }
    }
}
