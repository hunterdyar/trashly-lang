using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class CallExpression : Expression
{ 
	public Expression Function;//identifier or function literal probably?
	public List<Expression> Arguments = new List<Expression>();

	public override string ToString()
	{
		var str = Function.ToString();
		str += "(";
		foreach (var argument in Arguments)
		{
			str += argument.ToString();
		}

		str += ")";
		return str;
	}

	public CallExpression(Token token) : base(token)
	{
		
	}
}