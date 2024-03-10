using System.Collections;

namespace TrashlyLang.objects;

public class Boolean : Object
{
	public bool Value;

	public static Boolean FromString(string b)
	{
		b = b.ToLower();
		var bo = new Boolean();
		if (b == "false" || b == "0" || b == "no" || b == "naaah")
		{
			bo.Value = false;
			return bo;
		}
		else if(b == "true" || b == "1" || b == "yes" || b == "yeauh" || b == "yeah")
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
}