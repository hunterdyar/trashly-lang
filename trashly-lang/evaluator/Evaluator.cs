using TrashlyLang.ast;
using TrashlyLang.objects;
using Object = TrashlyLang.objects.Object;
using Boolean = TrashlyLang.objects.Boolean;

namespace TrashlyLang.evaluator;

public class Evaluator
{
	//I feel like I should just put this in the AST. It makes the most sense? I think? //Statements eval and return... void?
	//but.... I want the code seperated? AST as a thing that gets consumed or converted, not a thing that IS the program feels better to me.
	//or at least more clear for the purpose of teaching.
	//so, what? partial classes? bleh.
	public Object Eval(Node node)
	{
		if (node is IntegerLiteral il)
		{
			return Integer.FromString(il.Token.Literal);
		}else if (node is BooleanLiteral bl)
		{
			return Boolean.FromString(bl.Token.Literal);
		}else if (node is EmptyExpression)
		{
			return new Null();
		}else if (node is BlockStatement bs)
		{
			Object result = new Null();
			foreach (var e in bs.Statements)
			{
				result = Eval(e);
			}

			return result;
		}

		return new Null();
	}
}