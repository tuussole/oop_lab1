using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Lab1.DataTable
{
    class GrammarColumn : DataGridViewColumn
    {
        public GrammarColumn()
        {
            this.CellTemplate = new GrammarCell();
        }
    }
}
