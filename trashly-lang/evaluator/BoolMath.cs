using Boolean = TrashlyLang.objects.Boolean;

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
}