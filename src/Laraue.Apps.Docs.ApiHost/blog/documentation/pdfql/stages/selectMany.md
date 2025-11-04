---
type: documentation
title: SelectMany stage
project: PdfQL
---
SelectMany is the operation that can get from the objects collection the sequence of requested objects.

#### SelectManyStage syntax
```antlr
SelectManyStage
  : 'selectMany' '(' Selector ')'  
  ;
```
Related tokens  
_[Selector](../keyword/selector)_

#### Usage examples
1. Select table rows
    ```csharp
    select(tables)->selectMany(tableRows) // PdfTableRow[]
    ```
2. Select table cells
    ```csharp
    select(tables)->selectMany(tableCells) // PdfTableCell[]
    ```