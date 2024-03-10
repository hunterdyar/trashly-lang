using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic.CompilerServices;
using TrashlyLang.ast;
using TrashlyLang.lexer;

namespace TrashlyLang.Parser;

public class Parser
{
	//prefix lookup table
	//infix lookup table
	private Lexer _lexer;
	private Token _currentToken = new Token(TokenType.EOF, "");
	private Token _peekToken = new Token(TokenType.EOF, "");
	public List<Node> Program => _program;
	private List<Node> _program; //root statements.
	private int PrefixPrecedence;
	delegate Expression PrefixExpressionFactory();

	delegate Expression InfixExpressionFactory(Expression left);

	private Dictionary<TokenType, int> _precedence = new Dictionary<TokenType, int>();

	private Dictionary<TokenType, PrefixExpressionFactory> prefixGenerators = new Dictionary<TokenType, PrefixExpressionFactory>();
	private Dictionary<TokenType, InfixExpressionFactory> infixGenerators = new Dictionary<TokenType, InfixExpressionFactory>();
	public Parser(Lexer lexer)
	{
		_lexer = lexer;
		//order of operations init
		_precedence.Add(TokenType.Equals,2);
		_precedence.Add(TokenType.NotEqual,2);
		_precedence.Add(TokenType.LessThan,3);
		_precedence.Add(TokenType.GreaterThan,3);
		_precedence.Add(TokenType.Add,5);
		_precedence.Add(TokenType.Minus,5);
		_precedence.Add(TokenType.Slash,7);
		_precedence.Add(TokenType.Asterisk,7);
		PrefixPrecedence = 9; //-a, !true, 
		_precedence.Add(TokenType.LeftParen, 10);//call
		
		//prefix init
		//identiy,int, !,-,true,false,(,if,functioncall
		prefixGenerators.Add(TokenType.Identity,ParseIdentifier);
		prefixGenerators.Add(TokenType.Integer,ParseIntegerLiteral);
		prefixGenerators.Add(TokenType.Bang,ParsePrefixExpression);
		prefixGenerators.Add(TokenType.Minus,ParsePrefixExpression);
		prefixGenerators.Add(TokenType.True,ParseBooleanLiteral);
		prefixGenerators.Add(TokenType.False,ParseBooleanLiteral);
		prefixGenerators.Add(TokenType.LeftParen,ParseGroupedExpression);
		prefixGenerators.Add(TokenType.LeftBrace,ParseBlockExpression);
		prefixGenerators.Add(TokenType.If,ParseIfExpression);
	//	prefixGenerators.Add(TokenType.LeftBrace,ParseBlockStatement);
		//infix
		//todo: create "IntInfixExpression".
		infixGenerators.Add(TokenType.Add,ParseInfixExpression);
		infixGenerators.Add(TokenType.Minus,ParseInfixExpression);
		infixGenerators.Add(TokenType.Asterisk,ParseInfixExpression);
		infixGenerators.Add(TokenType.Slash,ParseInfixExpression);
		infixGenerators.Add(TokenType.Equals,ParseInfixExpression);
		infixGenerators.Add(TokenType.NotEqual,ParseInfixExpression);
		infixGenerators.Add(TokenType.LeftParen,ParseCallExpression);
		//+,-,/,*,==,!=,<,>
	}

	public int GetPrecedence(TokenType type)
	{
		if(_precedence.TryGetValue(type,out var p))
		{
			return p;
		}
		return 0;
	}

	public (int,int) InfixBindingPower(TokenType type)
	{
		switch (type)
		{
			case TokenType.Add:
			case TokenType.Minus:
				return (1, 2);
			case TokenType.Asterisk:
			case TokenType.Slash:
				return (3, 4);
			default: return (0,0);
		}
	}
	public bool AtToken(TokenType type)
	{
		return _currentToken.Type == type;
	}
	
	public void Parse()
	{
		//make a 'program' root node that is a list of statements.
		//i used the word 'expression' in my AST and this is wrong.
			
		Next();
		Next();
		_program = new List<Node>();
		while (_currentToken.Type != TokenType.EOF)
		{
			var statement = ParseStatement();
			_program.Add(statement);
		}
	}
	//move to the next token
	public void Next()
	{
		_currentToken = _peekToken;
		_peekToken = _lexer.NextToken();
	}

	//Assert the value of the current token, then move to next.
	public void Eat(TokenType value)
	{
		if (value != _currentToken.Type)
		{
			throw new Exception($"Expected {value}, got {_currentToken.Type.ToString()}. It's probably your fault, not mine.");
		}
		Next();
	}

	//Parse to the right, eating tokens UNTIL we encounter a token with a binding power <= rbp.
	public Expression ParseExpression(int minBindingPower=0) 
	{
		if (AtToken(TokenType.EOF))
		{
			//welp
			throw new Exception("Unexpected End of file.");
		}else if (AtToken(TokenType.Semicolon))
		{
			//blank semicolons are allowed. this is allowed;;;;;
			var empty = new EmptyExpression(_currentToken);
			Eat(TokenType.Semicolon);
			return empty;
		}

		Expression expr = null;
		if(!prefixGenerators.TryGetValue(_currentToken.Type, out var generator))
		{
			//uh oh, no prefix thingy
			throw new Exception($"Encountered prefix operator {_currentToken.Type} without a prefix thing in the thing.");
			return null;
		} 
		expr = generator(); //This parses the expression and any token before it.
		

		//while not semicolon, because theyre OPTIONAL BABY WHOO
		while (_peekToken.Type != TokenType.Semicolon)
		{
			//next we find an integer, and then....
			var peek = _peekToken;
			var bp = InfixBindingPower(peek.Type);
			var cbp = InfixBindingPower(_currentToken.Type);
				//currentToken?peekToken?
			//first peek the integer (or whatever) until we peek the other.
			//when this is >, it's all right associative
			//when this is < it's all left associative
			//the algorithm works (is supposed to work) by deciding via recursion to keep chewing forward (1+2+3+4), and stepping up out of recursion to "wrap up" what is has chewed.
			
			//we aren't getting the right values?
			if (cbp.Item1 < minBindingPower)
			{
				break;
			}
			
			//this whole section is just a fancy way to call ParseExpression again.
			//but we do it differenlty for different operators. Which ones? no clue, the dictionary lookup for function calls is a clever way to handle it.
			if(!infixGenerators.TryGetValue(_currentToken.Type, out var inGenerator))
			{
				//no infix. This must be a prefix, so we can just give it back without fluffing about with precedence.PeekPrecedence())
				//if (when?) we do postfix (5!, 3++), we do a tryget here for that.
				if (expr == null)
				{
					throw new Exception("encountered operator with no prefix OR infix thing in the thing");
				}
				return expr;
			}
			else
			{
				expr = inGenerator(expr);
			}
		}

		if (expr == null)
		{
			throw new Exception("We done goofed");
		}
		return expr;
	}
	
	public Expression ParsePrefixExpression()
	{
		var expression = new PrefixExpression(_currentToken);
		expression.Operator = _currentToken.Literal;
		Next();//eat the operator, but we don't know which one so we can't declare.
		expression.right = ParseExpression(PrefixPrecedence);
		return expression;
	}

	public Expression ParseInfixExpression(Expression left){
		var expression = new InfixExpression(_currentToken);
		expression.Operator = _currentToken.Literal;
		var opType = _currentToken.Type;
		expression.left = left;
		// current precedence.
		int precedence = _precedence[_currentToken.Type];
		precedence = InfixBindingPower(opType).Item2;
		Next();//consume the operator.
		//this is the recursive call.
		expression.right = ParseExpression(precedence);
		return expression;
	}
	public Node ParseStatement()
	{
		switch (_currentToken.Type)
		{
			case TokenType.Let:
				return ParseLetStatement();
			case TokenType.Return:
				return ParseReturnStatement();
			case TokenType.LeftBrace:
				return ParseBlockStatement();
			case TokenType.Function:
				return ParseFunctionLiteral();
			default:
				//ParseExpressionStatement.
				//ExpressionNode.Expression = this.
				var exp = ParseExpression();
				//skip semicolons! That's right, they're OPTIONAL.
				//OPTIONAL SEMICOLONS ARE THE ONLY WRONG CHOICE TO WHETHER OR NOT YOU SHOULD HAVE SEMICOLONS
				//MUAHAHAHAHA
				if (_peekToken.Type == TokenType.Semicolon)
				{
					Next();
				}

				return exp;
		}
	}
	
	public Node ParseLetStatement()
	{
		 Eat(TokenType.Let);
		 var letStatement = new Identifier(_currentToken);
		 Eat(TokenType.Identity);
		 Eat(TokenType.Assign);
		 //parse the right side of the =
		 letStatement.Value = ParseExpression();
		 //chew up the optional semicolon. yum yum.
		 if (_currentToken.Type == TokenType.Semicolon)
		{
			Eat(TokenType.Semicolon);
		}
		 return letStatement;
	}

	public Node ParseReturnStatement()
	{
		var statement = new ReturnStatement(_currentToken);
		Eat(TokenType.Return);
		statement.ReturnValue = ParseExpression();
		//Next();
		if (_currentToken.Type == TokenType.Semicolon)
		{
			Eat(TokenType.Semicolon);
		}

		return statement;
	}

	private Node ParseBlockStatement()
	{
		var block = new BlockStatement(_currentToken);
		Eat(TokenType.LeftBrace);
		while (_currentToken.Type != TokenType.RightBrace)
		{
			var stmt = ParseStatement();
			block.Statements.Add(stmt);
		}

		Eat(TokenType.RightBrace);
		return block;
	}
	
	private Expression ParseBlockExpression()
	{
		var block = new GroupExpression(_currentToken);
		Eat(TokenType.LeftBrace);
		while (_currentToken.Type != TokenType.RightBrace)
		{
			var express = ParseExpression(0);
			block.Children.Add(express);
		}

		Eat(TokenType.RightBrace);
		return block;
	}


	private Expression ParseGroupedExpression()
	{
		var group = new GroupExpression(_currentToken);
		Eat(TokenType.LeftParen);
		var expression = ParseExpression(0);
		group.Children.Add(expression);
		//for, consume commas
		
		//eat the right )
		Eat(TokenType.RightParen);
		return group;
	}
	public Expression ParseIdentifier()
    {
     	var e = new Identifier(_currentToken);
        Eat(TokenType.Identity);
        return e;
    }

	public Expression ParseIntegerLiteral()
	{
		var e= new IntegerLiteral(_currentToken); 
		Eat(TokenType.Integer);
		return e;
	}

	public Expression ParseBooleanLiteral()
	{
		if (_currentToken.Type == TokenType.True || _currentToken.Type == TokenType.False)
		{
			var b = new BooleanLiteral(_currentToken);
			Next();
			return b;
		}
		else
		{
			throw new Exception($"Can't parse. {_currentToken} is not a boolean");
		}

		return null;
	}

	public Expression ParseIfExpression()
	{
		var e = new IfExpression(_currentToken);
		Eat(TokenType.If);
		Eat(TokenType.LeftParen);
		e.Condition = ParseExpression();
		Eat(TokenType.RightParen);
		e.Consequence = ParseBlockExpression();//eats left and right brace
		
		//for else if's, basically this but in a loop? ish
		if (_currentToken.Type == TokenType.Else)
		{
			e.HasAlt = true;
			Eat(TokenType.Else);
			e.Alternative = ParseBlockExpression();
		}
		else
		{
			e.HasAlt = false;
		}
		return e;
	}

	public Node ParseFunctionLiteral()
	{
		var e = new FunctionLiteral(_currentToken);
		Eat(TokenType.Function);
		e.Identity = new Identifier(_currentToken);
		Eat(TokenType.Identity);
		e.Parameters = ParseFunctionParameters();//eat ()
		e.Body = ParseBlockStatement();// eat {}
		return e;
	}

	public List<Identifier> ParseFunctionParameters()
	{
		Eat(TokenType.LeftParen);
		List<Identifier> parameters = new List<Identifier>();
		//no parameters
		if (_currentToken.Type == TokenType.RightParen)
		{
			Eat(TokenType.RightParen);
			return parameters;
		}
		//first parameter
		var i = new Identifier(_currentToken);
		parameters.Add(i);
		Eat(TokenType.Identity);
		//further parameters and the preceding comma.
		while (_currentToken.Type == TokenType.Comma)
		{
			Eat(TokenType.Comma);
			i = new Identifier(_currentToken);
			parameters.Add(i);
			Eat(TokenType.Identity);
		}
		Eat(TokenType.RightParen);
		return parameters;
	}

	public Expression ParseCallExpression(Expression left)
	{
		var e = new CallExpression(_currentToken);
		e.Function = left;
		e.Arguments = ParseCallArguments(); //eats the ( and )
		if (_currentToken.Type == TokenType.Semicolon)
		{
			Eat(TokenType.Semicolon);
		}

		return e;
	}

	public List<Expression> ParseCallArguments()
	{
		Eat(TokenType.LeftParen);
		var args = new List<Expression>();
		if (_currentToken.Type == TokenType.RightParen)
		{
			Eat(TokenType.RightParen);
			return args;
		}
		//first expression
		args.Add(ParseExpression());
		//commas and next expressions
		while (_currentToken.Type == TokenType.Comma)
		{
			Eat(TokenType.Comma);
			args.Add(ParseExpression());
		}
		Eat(TokenType.RightParen);
		return args;
	}
}