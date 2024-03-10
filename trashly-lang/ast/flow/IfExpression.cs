using TrashlyLang.lexer;
using TrashlyLang.objects;

namespace TrashlyLang.ast;

public class IfExpression : Expression
{
	public bool HasAlt;
	public Expression Condition;
	public Expression Consequence;
	public Expression Alternative = null;
	public IfExpression(Token token) : base(token)
	{
	}

	public override string ToString()
	{
		var str = "if(";
		str += Condition.ToString();
		str += "){";
		str += Consequence.ToString();
		str += "}";
		if (HasAlt)
		{
			str += "else{";
			str += Alternative.ToString();
			str += "}";
		}

		return str;
	}
}