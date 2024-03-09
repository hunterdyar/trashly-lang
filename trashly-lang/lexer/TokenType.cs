namespace TrashlyLang.lexer;

public enum TokenType
{
	Illegal,
	Function,
	Let,
	Assign,
	Equals,
	NotEqual,
	Semicolon,
	LeftParen,
	RightParen,
	LeftBrace,
	RightBrace,
	Return,
	Comma,
	Minus,
	Add,
	Slash,
	Asterisk,
	LessThan,
	GreaterThan,
	True,
	False,
	Bang,
	If,
	Else,
	Identity,
	Integer,
	EOF
}