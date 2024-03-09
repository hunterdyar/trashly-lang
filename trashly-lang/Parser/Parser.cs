using System.Collections;
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
		//infix
		//todo: create "IntInfixExpression".
		infixGenerators.Add(TokenType.Add,ParseInfixExpression);
		infixGenerators.Add(TokenType.Minus,ParseInfixExpression);
		infixGenerators.Add(TokenType.Asterisk,ParseInfixExpression);
		infixGenerators.Add(TokenType.Slash,ParseInfixExpression);
		//+,-,/,*,==,!=,<,>
	}

	public int GetPrecedence(TokenType type)
	{
		if(_precedence.TryGetValue(type,out var p))
		{
			return p;
		}
		return 1;
	}
	public bool AtToken(TokenType type)
	{
		return _currentToken.Type == type;
	}

	//move to the next token
	public void Next()
	{
		//note: we start one 'behind' and need to go forward one then start.
		_currentToken = _peekToken;
		_peekToken = _lexer.NextToken();

	}

	//Assert the value of the current token, then move to next.
	public void Eat(TokenType value)
	{
		if (value != _currentToken.Type)
		{
			throw new Exception($"Expected {value}, got {_currentToken.Type.ToString()}");
		}
		Next();
	}

	//Parse to the right, eating tokens UNTIL we encounter a token with a binding power <= rbp.
	public Expression ParseExpression(int rightBondPower) 
	{
		if (AtToken(TokenType.EOF))
		{
			//welp
			throw new Exception("Unexpected End of file.");
		}
		
		Expression expr = null;
		if(!prefixGenerators.TryGetValue(_currentToken.Type, out var generator))
		{
			//uh oh, no prefix thingy
			throw new Exception("Encountered prefix operator without a prefix thing in the thing.");
			return null;
		}
		else
		{
			expr = generator(); //This parses the expression and any token before it.
		}

		//while not semicolon, because theyre OPTIONAL BABY WHOO
		while (_peekToken.Type != TokenType.Semicolon && rightBondPower < PeekPrecedence())
		{
			if(!infixGenerators.TryGetValue(_currentToken.Type, out var inGenerator))
			{
				//no infix. This must be a prefix, so we can just give it back without fluffing about with precedence.
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
		//Next();//this is probably wrong, expression functions should eat the tokens.
		if (expr == null)
		{
			throw new Exception("We done goofed");
		}
		return expr;
	}

	int PeekPrecedence()
	{
		//look up the precedence of peekToken.Type;
		return GetPrecedence(_peekToken.Type);
	}	
	//entry point. We expect this to return a single root node.
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

	public Node ParseStatement()
	{
		switch (_currentToken.Type)
		{
			case TokenType.Let:
				return ParseLetStatement();
			case TokenType.Return:
				return ParseReturnStatement();
			default:
				var exp = ParseExpression(0);
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
		 var statement = new Identifier(_currentToken);
		 Eat(TokenType.Identity);
		 Eat(TokenType.Assign);
		 //parse the right side of the =
		 
		 Next();
		 statement.Value = ParseExpression(0);
		 return statement;
	}

	public Node ParseReturnStatement()
	{
		var statement = new ReturnStatement(_currentToken);
		Eat(TokenType.Return);
		statement.ReturnValue = ParseExpression(0);
		Next();
		if (_peekToken.Type == TokenType.Semicolon)
		{
			Next();
		}

		return statement;
	}
	public Expression ParseIdentifier()
    {
     	var e = new Identifier(_currentToken);
        Eat(TokenType.Identity);
        return e;
    }

	public Expression ParseIntegerLiteral()
	{
		var e= new Integer(_currentToken); 
		Eat(TokenType.Integer);
		return e;
	}

	public Expression ParsePrefixExpression()
	{
		var expression = new PrefixExpression(_currentToken);
		expression.Operator = _currentToken.Literal;
		Next();//eat the operator, but we don't know which one so we can't declare.
		expression.right = ParseExpression(PrefixPrecedence);
		return expression;
	}

	public Expression ParseInfixExpression(Expression left)
	{
		var expression = new InfixExpression(_currentToken);
		expression.Operator = _currentToken.Literal;
		expression.left = left;
		int precedence = _precedence[_currentToken.Type];
		Next();
		expression.right = ParseExpression(precedence);
		return expression;
	}
}