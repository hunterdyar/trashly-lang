using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class ReturnStatement : Node
{
	public Expression ReturnValue;
	public ReturnStatement(Token token) : base(token)
	{
	}
}