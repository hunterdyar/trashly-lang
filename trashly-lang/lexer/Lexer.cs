namespace TrashlyLang.lexer;

public class Lexer
{
	private string _input;
	private int _pos;
	private int _readPos; //peek position
	private char Character => CurrentCharacter();

	public Lexer(string input)
	{
		_input = input;
		//initialize at 0 and 1
		_pos = 0;
		_readPos = 1;
		//ReadCharacter();
	}

	private void ReadCharacter()
	{
		_pos = _readPos;
		_readPos += 1;
	}

	public char CurrentCharacter()
	{
		return _pos < _input.Length ? _input[_pos] : '\0';
	}
	public char PeekCharacter()
	{
		return _readPos < _input.Length ? _input[_readPos] : '\0';
	}

	public List<Token> GetAllLexDebug()
	{
		if (_input == "")
		{
			return new List<Token>();
		}
		
		List<Token> tokens = new List<Token>();
		
		//this pattern is a do_while, but nobody uses those.
		var t = NextToken();
		tokens.Add(t);
		while (t.Type != TokenType.EOF)
		{
			t = NextToken();
			tokens.Add(t);
		}

		return tokens;
	}
	public Token NextToken()
	{
		Token token;
		EatWhitespace();
		switch (Character)
		{
			case '=':
				if (PeekCharacter() == '=')
				{
					//we construct everything because when it breaks, we want to see where it broke.
					//I decided that the literal needs to be the data from the file as a rule
					//for debugging. even if we're _extremely_ confident that literal becomes '==' (as it should)
					string literal = Character.ToString() + PeekCharacter().ToString();
					ReadCharacter();//skip ahead to consume the ==
					token = new Token(TokenType.Equals, literal);
				}
				else
				{
					token = new Token(TokenType.Assign, Character);
				}
				break;
			case ';':
				token = new Token(TokenType.Semicolon, Character);
				break;
			case '(':
				token = new Token(TokenType.LeftParen, Character);
				break;
			case ')':
				token = new Token(TokenType.RightParen, Character);
				break;
			case ',':
				token = new Token(TokenType.Comma, Character);
				break;
			case '*':
				token = new Token(TokenType.Asterisk, Character);
				break;
			case '<':
				token = new Token(TokenType.LessThan, Character);
				break;
			case '>':
				token = new Token(TokenType.GreaterThan, Character);
				break;
			case '+':
				token = new Token(TokenType.Add, Character);
				break;
			case '-':
				token = new Token(TokenType.Minus, Character);
				break;
			case '{':
				token = new Token(TokenType.LeftBrace, Character);
				break;
			case '}':
				token = new Token(TokenType.RightBrace, Character);
				break;
			case '/':
				token = new Token(TokenType.Slash, Character);
				break;
			case '!':
				if (PeekCharacter() == '=')
				{
					string literal = Character.ToString() + PeekCharacter().ToString();
					ReadCharacter();
					token = new Token(TokenType.NotEqual, literal);
				}
				else
				{
					token = new Token(TokenType.Bang, Character);
				}
				break;
			case '\0':
				token = new Token(TokenType.EOF, Character);
				break;
			default:
				if (IsLetter(Character))
				{
					token.Literal = ReadIdentifier();
					token.Type = Token.LookupTokenType(token.Literal);
					return token;
				}else if (IsDigit(Character))
				{
					token.Type = TokenType.Integer;
					token.Literal = ReadNumber();
					return token;
				}
				else
				{
					token = new Token(TokenType.Illegal, Character);
				}

				break;
		}
		//go next!
		ReadCharacter();
		return token;
	}

	private string ReadIdentifier()
	{
		int pos = _pos;
		while (IsLetter(Character))
		{
			ReadCharacter();
			var current = _input.Substring(pos, _pos - pos);
			//end brace takes precedence over variables. so "xXreturn fiveXx" should eval to {,RET,IDENT(five),}
			//so we have to do this stupid slow thing so our bad idea can work.
			if (current == "xX" || current == "Xx" || current == "Oo" || current == "oO")
			{
				return current;
			}
		}
		return _input.Substring(pos, _pos - pos);
	}

	private string ReadNumber()
	{
		int pos = _pos;
		while (IsDigit(Character))
		{
			//this increments _pos
			ReadCharacter();
		}

		return _input.Substring(pos, _pos - pos);
	}
	private void EatWhitespace()
	{
		while (IsWhitespace(Character))
		{
			ReadCharacter();
		}
	}

	public static bool IsWhitespace(char character)
	{
		//space, newline, return, and tab.
		return character == ' ' || character == '\n' || character == '\r' || character == '\t';
	}
	
	public static bool IsLetter(char character)
	{
		//i must emphasize that the point of this code is readability, not performance <_<
		//and i am willing to die on that hill.
		return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(character);
	}
	public static bool IsDigit(char character)
	{
		return "0123456789".Contains(character);
	}
}
