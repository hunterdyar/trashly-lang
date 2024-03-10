using System.Linq.Expressions;
using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class FunctionLiteral : Node
{
	public Identifier Identity;
	public List<Identifier> Parameters = new List<Identifier>();
	public int ArgCount => Parameters.Count;
	public Node Body;
	
	public FunctionLiteral(Token token) : base(token)
	{
	}

	public override string ToString()
	{
		var str = Identity.ToString();
		str += "(";
		foreach (var parameter in Parameters)
		{
			str += parameter.ToString() + ",";
		}

		str += "){";
		str += Body.ToString();
		str += "}";
		return str;
	}
}