---
type: documentation
title: Select stage
project: PdfQL
---
Select is the operation that can get from the single object the sequence of requested objects.

#### SelectStage syntax
```antlr
SelectStage
  : 'select' '(' Selector ')'  
  ;
```

Related tokens  
_[Selector](../keyword/selector)_

#### Usage examples
1. Select tables
    ```csharp
    select(tables) // PdfTable[]
    ```
2. Select table rows
    ```csharp
    select(tableRows) // PdfTableRow[]
    ```
3. Select table cells
    ```csharp
    select(tableCells) // PdfTableCell[]
    ```