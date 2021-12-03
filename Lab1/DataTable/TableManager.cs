using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Lab1.DataTable
{
    public class TableManager
    {
        private readonly DataGridView _table;

        public DataGridView DataGridView { get => _table; }

        public TableManager(DataGridView table)
        {
            _table = table;
        }

        public double GetValue(int columnIndex, int rowIndex)
        {
            ValidateCell(columnIndex, rowIndex);

            var value = _table.Rows[rowIndex].Cells[columnIndex].Value.ToString();

            return double.Parse(value);
        }

        private void ValidateCell(int columnIndex, int rowIndex)
        {
            if(_table.Columns.Count <= columnIndex)
            {
                throw new Exception($"Column {columnIndex} isn't exist");
            }

            if (_table.Rows.Count <= rowIndex)
            {
                throw new Exception($"Row {columnIndex} isn't exist");
            }
        }
    }
}
