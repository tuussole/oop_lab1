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
using System.Text.RegularExpressions;
using System.Threading;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private const string template = "{0}: HiddenValue:{1}, Value: {2}; Cell: {3} {4}";
        private const string changeEvent = "ChangeEvent";
        private const string cellBeginEdit = "CellBeginEdit";

        private static readonly (int Row, int Column) NoneColumn = (-1, -1);

        private (int Row, int Column) _lastChangedColumn = NoneColumn;

        private Lock _lock = new Lock();
        private Calculator _calculator;

        private bool _isInRecalculation = false;
        private bool _isCalulateSelected = false;
        private HashSet<GrammarCell> _cellsWithIdentifier = new HashSet<GrammarCell>();

        private (int Row, int Column) _selectedColumn = NoneColumn;

        public Form1()
        {
            InitializeComponent();

            var tableManager = new TableManager(this.dataGridView1);
            _calculator = new Calculator(tableManager);

            var column1 = new GrammarColumn();
            column1.Name = "A";
            this.dataGridView1.Columns.Add(column1);
            var column2 = new GrammarColumn();
            column2.Name = "B";
            this.dataGridView1.Columns.Add(column2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var res = Evaluate(textBox1.Text);           //????????
            label1.Text = res.ToString();
        }

        private async void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsIncorrectCell(e.RowIndex, e.ColumnIndex))
            {
                return;
            }
            var currentColumn = (e.RowIndex, e.ColumnIndex);

            if (_lastChangedColumn != NoneColumn && _lastChangedColumn == currentColumn)
            {
                _lastChangedColumn = NoneColumn;
                Debug.WriteLine("!!!Catch double event trigger");
                return;
            }
            _lastChangedColumn = currentColumn;

            if (_isInRecalculation)
            {
                Debug.WriteLine("In Recalculation {0} {1}", e.RowIndex, e.ColumnIndex);

                return;
            }

            if (_isCalulateSelected)
            {
                return;
            }

            var cell = GetCell(e.RowIndex, e.ColumnIndex);


            try
            {
                await _lock.EnterLock();

                Debug.WriteLine(template, changeEvent + " Before", cell.HiddenValue, cell.Value, e.RowIndex, e.ColumnIndex);

                cell.HiddenValue = new string(cell.Value.ToString());
                AfterChangeCell(cell);

                Debug.WriteLine(template, changeEvent + " After", cell.HiddenValue, cell.Value, e.RowIndex, e.ColumnIndex);
            }
            finally
            {
                _lock.LeaveLock();
            }
        }

        private async void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (IsIncorrectCell(e.RowIndex, e.ColumnIndex))
            {
                return;
            }

            var cell = GetCell(e.RowIndex, e.ColumnIndex);

            try
            {
                await _lock.EnterLock();

                Debug.WriteLine(template, cellBeginEdit + " Before", cell.HiddenValue, cell.Value, e.RowIndex, e.ColumnIndex);

                cell.Value = cell.HiddenValue;

                Debug.WriteLine(template, cellBeginEdit + " After", cell.HiddenValue, cell.Value, e.RowIndex, e.ColumnIndex);
            }
            finally
            {
                _lock.LeaveLock();
            }
        }

        private void Add(object sender, EventArgs e)
        {
            //Debug.Write("");
            var column = new GrammarColumn();
            var name = ColumnHelper.FirstColumnName;
            if (this.dataGridView1.Columns.Count > 0)
            {
                var lastColumn = this.dataGridView1.Columns[this.dataGridView1.Columns.Count - 1];
                var lastColumnName = ColumnHelper.ConvertColumnNameToChar(lastColumn.Name) + 1;
                name = (char)lastColumnName;
            }
            this.dataGridView1.Columns.Add(column);
            column.Name = name.ToString();
        }

        private void Remove(object sender, EventArgs e)
        {
            if (this.dataGridView1.Columns.Count > 0)
            {
                var columnIndex = this.dataGridView1.ColumnCount - 1;
                var lastColumn = this.dataGridView1.Columns[columnIndex];

                bool shouldRemove = true;
                bool alreadyAsked = false;

                foreach(var row in this.dataGridView1.Rows)
                {
                    var lastCell = (GrammarCell)((DataGridViewRow)row).Cells[columnIndex];

                    if (_cellsWithIdentifier.Contains(lastCell))
                    {
                        if (!alreadyAsked)
                        {
                            var result = MessageBox.Show("Last Column is used in other cell. Do you want to continue removing?", "Oops", MessageBoxButtons.YesNo);

                            shouldRemove = result == DialogResult.Yes;
                            alreadyAsked = true;

                            if (!shouldRemove)
                            {
                                break;
                            }
                        }

                        _cellsWithIdentifier.Remove(lastCell);
                    }
                }

                if (shouldRemove)
                {
                    this.dataGridView1.Columns.Remove(lastColumn);
                }
            }
        }

        private string Evaluate(string value)
        {
            var returnValue = string.Empty;
            try
            {
                returnValue = _calculator.Evaluate(value).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return returnValue;
        }

        private void RecalculateAllCells()
        {
            _isInRecalculation = true;
            foreach(var cell in _cellsWithIdentifier)
            {
                CalculateCellValue(cell);
            }
            _isInRecalculation = false;
        }

        private void CalculateCellValue(GrammarCell cell)
        {
            cell.Value = Evaluate(cell.HiddenValue);
        }

        private void AddToQueueIfContainsIdentifier(GrammarCell cell)
        {
            if(Regex.IsMatch(cell.HiddenValue, ColumnHelper.IdentifierPattern))
            {
                if (!_cellsWithIdentifier.Contains(cell))
                {
                    _cellsWithIdentifier.Add(cell);
                }
            }
            else
            {
                if (_cellsWithIdentifier.Contains(cell))
                {
                    _cellsWithIdentifier.Remove(cell);
                }
            }

        }

        private GrammarCell GetCell(int rowIndex, int columnIndex)
        {
            return (GrammarCell)this.dataGridView1.Rows[rowIndex].Cells[columnIndex];
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (IsIncorrectCell(e.RowIndex, e.ColumnIndex))
            {
                return;
            }
            var cell = GetCell(e.RowIndex, e.ColumnIndex);

            this.textBox1.Text = cell.HiddenValue;

            _selectedColumn = (e.RowIndex, e.ColumnIndex);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(IsIncorrectCell(_selectedColumn.Row, _selectedColumn.Column))
            {
                return;
            }

            var cell = GetCell(_selectedColumn.Row, _selectedColumn.Column);

            _isCalulateSelected = true;

            cell.HiddenValue = this.textBox1.Text;
            AfterChangeCell(cell);

            _isCalulateSelected = false;
        }

        public bool IsIncorrectCell(int rowIndex, int columnIndex)
        {
            var isIncorrectRow = rowIndex < 0 || rowIndex >= this.dataGridView1.RowCount;
            var isIncorrectColumn = columnIndex < 0 || columnIndex >= this.dataGridView1.ColumnCount;

            return isIncorrectRow || isIncorrectColumn;
        }

        private void AfterChangeCell(GrammarCell cell)
        {
            CalculateCellValue(cell);
            AddToQueueIfContainsIdentifier(cell);
            RecalculateAllCells();
        }
    }
}
