---
type: documentation
title: Lambda Expression
project: PdfQL
createdAt: 2025-08-01
updatedAt: 2025-08-01
---
Lambda allows to specify a function. In PdfQL is used to specify mappings and filtering.

#### LambdaExpression syntax
```antlr
LambdaExpression 
  : '(' (parameterNames)? ')' '=>' Expression
  ;
```