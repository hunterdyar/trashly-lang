# Trashly Lang
Trashly is a toy language designed for high visibility. It is not built for performance or usability.

The main gimmick is that memory is stored in a bitmap. It can be exported and inspected at any time. It's not fast - hilariously so - but it was built to be educational.

## Lexer
The lexer first takes the 'string' input of the program and turns it into a list of tokens.

## Parsing
The parser turns the 1-dimensional list of tokens from the lexer into a tree-structure that accurately (ish) represents the structure of the code. This tree structure is called the abstract syntax tree (ast).

It uses an algorithm called '[Pratt parsing](https://en.wikipedia.org/wiki/Operator-precedence_parser#Pratt_parsing)'.
*Before trying to understand pratt parsing, take a look at [shunting yard](https://en.wikipedia.org/wiki/Shunting_yard_algorithm).* It's a simpler-to-understand algorithm that is easy to visualize. Pratt is sort of like shunting yard but using recursion instead of stacks.

After parsing is complete, one can visualize the tree output as a .dot file, viewable in graphViz. 

## Execution
Execution works by tree-walking (recursively) along the AST. The Execution functions ('Eval') are not implemented on the AST, but only because I wanted things to be separated for readers. Instead we have a fun if/else branch!

---
### Language Design Notes
What if you could use xXGamerTagXx syntax instead of {CurlyBrace} syntax? That would be kind of silly and dumb, right? Well, made me chuckle.

- xX Xx instead of { }
- permit instead of let
- oO Oo instead of ()
- you can use {} and () if you really want
