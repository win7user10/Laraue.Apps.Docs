---
type: documentation
title: Filter stage
project: PdfQL
---
Filter is the operation that returns objects from a sequence that matches the passed condition.

#### FilterStage syntax
```antlr
FilterStage
  : 'filter' '(' LambdaExpression ')'  
  ;
```

Related tokens  
_[LambdaExpression](../expression/lambda)_

#### Usage examples
1. For each table cell returns only those where text is equal to 'Title'.
    ```csharp
    select(tableCells) // PdfTableCell[]
        ->filter((item) => item.Text() == 'Title') // PdfTableCell[]
    ```