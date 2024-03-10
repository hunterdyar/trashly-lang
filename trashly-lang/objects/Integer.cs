using System.Collections;

namespace TrashlyLang.objects;

public class Integer : Object
{
	public int value;//temp!
/* 	public override int GetValueFromBytes(BitArray bits)
	{
		int val = 0;
		for (var i = 0; i < bits.Count; i++)
		{
			//this is a terrible way to do this, but I hope to replace the bitarray with direct 'memory' reads :)
			//so i am halfway in the bad-but-fun direction.
			if (bits[i])
			{
				val += i ^ 2;
			}
		}

		return val;
	}
*/
public override string Inspect()
{
	return value.ToString();
}

public static Integer FromString(string s)
	{
		int x = int.Parse(s);
		var i = new Integer();
		i.value = x;
		
		return i;
	}
}