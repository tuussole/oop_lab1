using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    public static class ColumnHelper
    {
        public const string IdentifierPattern = "[A-Z]{1}[0-9]{1}";
        public const char FirstColumnName = 'A';

        public static char ConvertColumnNameToChar(string name)
        {
            return name.FirstOrDefault();
        }
        public static int GetColumnNumber(string name)
        {
            var columnCharName = ConvertColumnNameToChar(name);

            return columnCharName - FirstColumnName;
        }

    }
}
