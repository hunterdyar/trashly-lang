using System.Collections;

namespace TrashlyLang.objects;

public abstract class Object
{
	public virtual ObjectType Type => ObjectType.Null;

	
	public virtual string Inspect()
	{
		return "";
	}
}