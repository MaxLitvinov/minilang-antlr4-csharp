grammar MiniLang;

program : statement* EOF ;

statement
    : 'let' ID '=' expr ';'                            # VarDecl
    | ID '=' expr ';'                                  # Assign
    | 'print' '(' expr ')' ';'                         # Print
    | 'if' '(' expr ')' statement ('else' statement)?  # If
    | 'while' '(' expr ')' statement                   # While
    | '{' statement* '}'                               # Block
    ;

expr
    : '-' expr                                         # Neg
    | expr op=('*'|'/'|'%') expr                       # MulDiv
    | expr op=('+'|'-') expr                           # AddSub
    | expr op=('<'|'<='|'>'|'>='|'=='|'!=') expr       # Compare
    | '(' expr ')'                                     # Parens
    | NUMBER                                           # Num
    | STRING                                           # Str
    | ID                                               # Var
    ;

ID      : [a-zA-Z_] [a-zA-Z_0-9]* ;
NUMBER  : [0-9]+ ('.' [0-9]+)? ;
STRING  : '"' (~["\r\n])* '"' ;
COMMENT : '//' ~[\r\n]* -> skip ;
WS      : [ \t\r\n]+ -> skip ;