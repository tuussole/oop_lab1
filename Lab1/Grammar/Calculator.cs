﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1.Grammar
{
    public static class Calculator
    {
        public static double Evaluate(string expression)
        {
            var lexer = new LabCalculatorLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LabCalculatorParser(tokens);

            var tree = parser.compileUnit();

            var visitor = new LabCalculatorVisitor();

            return visitor.Visit(tree);
        }
    }

}
