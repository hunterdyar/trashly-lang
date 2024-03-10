namespace TrashlyLang.objects;

public class Error : Object
{
	public string Message;

	public Error(string message)
	{
		Message = message;
	}

	public override string Inspect()
	{
		return "sowwy: "+Message;
	}
}