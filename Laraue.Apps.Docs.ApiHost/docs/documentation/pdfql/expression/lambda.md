---
type: documentation
title: Lambda Expression
project: PdfQL
---
Lambda allows to specify a function. In PdfQL is used to specify mappings and filtering.

#### LambdaExpression syntax
```antlr
LambdaExpression 
  : '(' (parameterNames)? ')' '=>' Expression
  ;
```