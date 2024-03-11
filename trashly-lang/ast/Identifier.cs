using TrashlyLang.lexer;
using TrashlyLang.objects;

namespace TrashlyLang.ast;

//leaf
public class Identifier : Expression
{
	public string Identity;
	public ObjectType Type;
	public Identifier(Token token) : base(token)
	{
		Identity = token.Literal;
	}
	
}