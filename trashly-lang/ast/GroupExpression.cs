using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class GroupExpression : Expression
{
	public List<Expression> Children;

	public override string ToString()
	{
		string s = "(";
		foreach (var child in Children)
		{
			s += child.ToString() + ",";
		}

		s += ")";
		return s;
	}

	public GroupExpression(Token token) : base(token)
	{
		Children = new List<Expression>();
	}
}