---
type: documentation
title: Instance Method Call Expression
project: PdfQL
---
Allows calling methods on an instance. In PdfQL is used to call methods on PDF objects.

#### InstanceMethodCall syntax
```antlr
InstanceMethodCallExpression
  : MemberExpression '.' '(' (parameters)?+ ')'
  ;
```