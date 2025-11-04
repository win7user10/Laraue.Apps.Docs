---
type: documentation
title: New Expression
project: PdfQL
---
New expression allows to create new instance. In PdfQL is used to create anonymous types. 

#### NewExpression syntax
```antlr
NewExpression 
  : 'new {' (MemberAssign)?+ '}'
  ;
```