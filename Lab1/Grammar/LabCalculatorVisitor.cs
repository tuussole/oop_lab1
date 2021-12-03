using Antlr4.Runtime.Misc;
using Lab1.DataTable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab1.Grammar
{
    public class LabCalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        private readonly TableManager _tableManager;

        public LabCalculatorVisitor(TableManager tableManager)
        {
            _tableManager = tableManager;
        }

        public override double VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(LabCalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            //Debug.WriteLine(result);

            return result;
        }

        //IdentifierExpr
        public override double VisitIdentifierExpr(LabCalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText();

            if(!Regex.IsMatch(result, ColumnHelper.IdentifierPattern))
            {
                throw new ArgumentException($"Incorrect identifier {result}");
            }

            var columnName = result[0].ToString();
            var columnIndex = ColumnHelper.GetColumnNumber(columnName);

            var rowIndex = int.Parse(result[1].ToString());

            // As first element in array is 0
            // but first human enter element is 1
            rowIndex--;

            return _tableManager.GetValue(columnIndex, rowIndex);
        }

        public override double VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentialExpr(LabCalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            //Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(LabCalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.ADD)
            {
                //Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else //LabCalculatorLexer.SUBTRACT
            {
               // Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        //public override double VisitUnaryAdditiveExpression([NotNull] LabCalculatorParser.UnaryAdditiveExpressionContext context)
        //{
        //    //var left = WalkLeft(context);
        //    //if (context.operatorToken.Type == LabCalculatorLexer.PLUS)
        //    //{
        //    //    //Debug.WriteLine();
        //    //    return left;
        //    //}
        //    //else //LabCalculatorLexer.MINUS
        //    //{
        //    //    // Debug.WriteLine();
        //    //    return -left;
        //    //}


        //    var left = WalkLeft(context);
        //    return -left;
        //}

        public override double VisitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext context)
        {
            var text = context.GetText();

            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.MULTIPLY)
            {
               // Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else //LabCalculatorLexer.DIVIDE
            {
               // Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }

        public override double VisitExtraMultiplicativeExpr(LabCalculatorParser.ExtraMultiplicativeExprContext context)  //expr mod expr / mod(expr, expr)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.MOD)
            {
                // Debug.WriteLine("{0} * {1}", left, right);
                return left % right;
            }
            else //LabCalculatorLexer.DIVIDE
            {
                // Debug.WriteLine("{0} / {1}", left, right);
                return (double)(int)((int)left / (int)right); //????
            }
        }

        private double WalkLeft(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(0));
        }

        private double WalkRight(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(1));
        }
    }

}
