grammar LabCalculator;

/*
 * Parser Rules
 */

compileUnit : expression EOF;


expression :
 LPAREN expression RPAREN #ParenthesizedExpr
 //| operatorToken=(PLUS | MINUS) LPAREN expression RPAREN #UnaryAdditiveExpression //?
 //| SUBTRACT  LPAREN expression RPAREN #UnaryAdditiveExpression
 | expression EXPONENT expression #ExponentialExpr
 | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
 | expression operatorToken=(MOD | DIV) expression #ExtraMultiplicativeExpr
 | expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
 | NUMBER #NumberExpr
 | IDENTIFIER #IdentifierExpr
 ; 

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?; 
IDENTIFIER : [A-Z]+[0-9]+; 

INT : ('0'..'9')+;

LPAREN : '(';
RPAREN : ')';
COMMA : ','; //?
//PLUS : '+'; //?
//MINUS : '-';
EXPONENT : '^';
MOD : '%';
DIV : '$';
MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';


WS : [ \t\r\n] -> channel(HIDDEN);