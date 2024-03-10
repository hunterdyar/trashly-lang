using System.Collections;
using TrashlyLang.evaluator;
using TrashlyLang.lexer;

namespace TrashlyLang.objects;

public class Boolean : Object
{
	public bool Value;

	public static Boolean FromString(string b)
	{
		b = b.ToLower();
		var bo = new Boolean();
		//we don't support casting
		if (b == "false")
		{
			bo.Value = false;
			return bo;
		}
		else if(b == "true")
		{
			bo.Value = true;
			return bo;
		}

		throw new Exception($"Unable to parse {b} as boolean");
		return bo;
	}

	public override string Inspect()
	{
		if (Value)
		{
			return "yeauh";
		}
		else
		{
			return "naaah";
		}
	}

	public static Object FromType(TokenType tokenType)
	{
		switch (tokenType)
		{
			case TokenType.True:
				return BoolMath.TRUE;
			case TokenType.False:
				return BoolMath.FALSE;
			default:
				throw new Exception($"{tokenType} is no boolean, sir.");
				return BoolMath.FALSE;
		}
	}
}