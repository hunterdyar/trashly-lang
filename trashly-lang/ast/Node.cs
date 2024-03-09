using TrashlyLang.lexer;

namespace TrashlyLang.ast;

//node is a statement
public class Node
{
	public Token Token;

	public Node(Token token)
	{
		Token = token;
	}


	public override string ToString()
	{
		return this.Token.Type.ToString();
	}
}