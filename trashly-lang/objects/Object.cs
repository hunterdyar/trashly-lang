using System.Collections;

namespace TrashlyLang.objects;

public abstract class Object
{
	public virtual ObjectType Type => ObjectType.Null;
	public static int MemSize;

	//public abstract T GetValueFromBytes(BitArray bits);
	public BitArray GetBytes()
	{
		var bytes = new BitArray(MemSize);
		return bytes;
	}
	//not calling it TOString because it's ToConfusing
	public virtual string Inspect()
	{
		return "";
	}
}