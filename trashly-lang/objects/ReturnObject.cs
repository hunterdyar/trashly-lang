namespace TrashlyLang.objects;

public class ReturnObject : Object
{
	public Object Value;

	public ReturnObject(Object val)
	{
		Value = val;
	}

	public override string Inspect()
	{
		return Value.Inspect();
	}
	
}