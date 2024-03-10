using System.Collections;

namespace TrashlyLang.objects;

public abstract class Object
{
	public int memLocation;
	public int memSize;

	//public abstract T GetValueFromBytes(BitArray bits);

	public BitArray GetBytes()
	{
		var bytes = new BitArray(memSize);
		return bytes;
	}
	//not calling it TOString because it's ToConfusing
	public virtual string Inspect()
	{
		return "";
	}
}