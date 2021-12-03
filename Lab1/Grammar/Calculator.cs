using Antlr4.Runtime;
using Lab1.DataTable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1.Grammar
{
    public class Calculator
    {
        private readonly LabCalculatorVisitor _visitor;


        public Calculator(TableManager tableManager)
        {
            _visitor = new LabCalculatorVisitor(tableManager);
        }

        public double Evaluate(string expression)
        {
            var lexer = new LabCalculatorLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LabCalculatorParser(tokens);

            var tree = parser.compileUnit();

            return _visitor.Visit(tree);
        }

    }

}
