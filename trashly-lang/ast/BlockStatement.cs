using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class BlockStatement : Node
{
	public List<Node> Statements;
	public BlockStatement(Token token) : base(token)
	{
		Statements = new List<Node>();
	}

	public override string ToString()
	{
		string str = "{";
		foreach (var s in Statements)
		{
			str += s.ToString() + ',';
		}

		return str + "}";
	}
}