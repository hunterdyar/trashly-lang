using TrashlyLang.ast;

namespace TrashlyLang.objects;

public class Function : Object
{
	public override ObjectType Type => ObjectType.Function;
	public FunctionLiteral Definition;//we will keep a copy of the function definition node.
	
	public override string Inspect()
	{
		return Definition.ToString();
	}
}