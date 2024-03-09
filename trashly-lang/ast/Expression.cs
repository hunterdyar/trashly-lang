using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class Expression : Node
{
	
	//returns a value... when parsed.... uh
	public Expression(Token token) : base(token)
	{
	}
}