namespace TrashlyLang.lexer;

public struct Token
{
	public TokenType Type;
	public string Literal;

	public Token(TokenType type, string literal)
	{
		this.Type = type;
		Literal = literal;
	}

	public Token(TokenType type, char literal)
	{
		this.Type = type;
		this.Literal = literal.ToString();
	}

	public static TokenType LookupTokenType(string keyword)
	{
		switch (keyword)
		{
			case "funk":
				return TokenType.Function;
			case "let":
				return TokenType.Let;
			case "permit":
				return TokenType.Let;
			case "return":
				return TokenType.Return;
			case "Xx":
				return TokenType.RightBrace;
			case "xX":
				return TokenType.LeftBrace;
			case "true":
				return TokenType.True;
			case "false":
				return TokenType.False;
			case "if":
				return TokenType.If;
			case "else":
				return TokenType.Else;
			case "oO":
				return TokenType.LeftParen;
			case "Oo":
				return TokenType.RightParen;
			default:
				//must be a user-created keyword AKA identity.
				return TokenType.Identity;
		}
	}
}