using TrashlyLang.objects;
using Object = TrashlyLang.objects.Object;

namespace TrashlyLang.evaluator;

public static class Math
{
	public static Integer Sum(Object a, Object b)
	{
		
		if (a is Integer ai && b is Integer bi)
		{
			//todo replace with bitwise FOR FUN.
			int val = ai.Value + bi.Value;
			Integer sum = new Integer(val);
			return sum;
		}
		else
		{
			throw new Exception($"Can't mathly do + on {a} and {b}");
		}
	}
	public static Integer Subtract(Object a, Object b)

	{
		if (a is Integer ai && b is Integer bi)
		{
			//todo replace with bitwise FOR FUN.
			int val = ai.Value - bi.Value;
			Integer sum = new Integer(val);
			return sum;
		}
		else
		{
			throw new Exception($"Can't do + on {a} and {b}");
		}
	}
	public static Integer Multiply(Object a, Object b)
	{
		if (a is Integer ai && b is Integer bi)
		{
			//todo replace with bitwise FOR FUN.
			int val = ai.Value * bi.Value;
			Integer product = new Integer(val);
			return product;
		}
		else
		{
			throw new Exception($"Can't do multiply on {{a}} and {b}");
		}	
	}

	public static Object Negate(Object value)
	{
		if (value is Integer val)
		{
			//todo replace with bitwise FOR FUN.
			val.Value = -val.Value;	
			return val;
		}
		else
		{
			throw new Exception($"Can't do - on {value}.");
		}	
	}

	public static Object Divide(Object a, Object b)
	{
		if (a is Integer ai && b is Integer bi)
		{
			//todo replace with bitwise FOR FUN.
			if (bi.Value == 0)
			{
				throw new Exception(
					"Trying to divide by 0. Are you trying to break computer? cus that's how you break computer!");
			}
			int val = ai.Value / bi.Value;
			Integer product = new Integer(val);
			return product;
		}
		else
		{
			throw new Exception($"Can't do divide on {a} and {b}");
		}
	}
}