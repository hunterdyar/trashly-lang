namespace TrashlyLang.objects;

public class Null : TrashlyLang.objects.Object
{
	public static int MemSize => 0;
	public override ObjectType Type => ObjectType.Null;
	public override string Inspect()
	{
		return "null";
	}
}