using TrashlyLang.lexer;
using TrashlyLang.objects;
using Boolean = TrashlyLang.objects.Boolean;
using Object = TrashlyLang.objects.Object;

namespace TrashlyLang.evaluator;

public static class BoolMath
{
	public static objects.Boolean TRUE = new Boolean() { Value = true };
	public static objects.Boolean FALSE = new Boolean() { Value = false };
	
	public static objects.Object Negate(objects.Object o)
	{
		if (o is TrashlyLang.objects.Boolean b)
		{
			if (b.Value)
			{
				return FALSE;
			}
			else
			{
				return TRUE;
			}
		}

		throw new Exception($"Can't negate {o}");
	}

	public static Boolean Equals(Boolean a, Boolean b)
	{
		if (a.Value == b.Value)
		{
			return TRUE;
		}

		return FALSE;
	}
	public static objects.Boolean NativeBoolToBoolObject(bool b)
	{
		if (b)
        {
        	return TRUE;
        }
        else
        {
        	return FALSE;
        }
	}

	public static Object Compare(TokenType comp, Object left, Object right)
	{
		if (left is Integer li && right is Integer ri)
		{
			switch (comp)
			{ 
				case TokenType.LessThan:
					return NativeBoolToBoolObject(li.value < ri.value);
				case TokenType.GreaterThan:
					return NativeBoolToBoolObject(li.value > ri.value);
				case TokenType.Equals:
					return NativeBoolToBoolObject(li.value == ri.value);
				case TokenType.NotEqual:
					return NativeBoolToBoolObject(li.value != ri.value);
			}
		}else if (left is Boolean lb && right is Boolean rb)
		{
			switch (comp)
			{
				case TokenType.Equals:
					return Equals(lb, rb);
				case TokenType.NotEqual:
					return Negate(Equals(lb, rb));
			}
		}

		return new Null();
	}
}