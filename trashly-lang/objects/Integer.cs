using System.Collections;
using System.Runtime.CompilerServices;

namespace TrashlyLang.objects;

public class Integer : Object
{
	public override ObjectType Type => ObjectType.Int;
	public static int MemSize => 8;
	public int Value;//temp!

	public Integer(int value)
	{
		Value = value;
	}

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
	public static Integer Construct(bool[] data)
	{
		int val = Decode(data);
        return new Integer(val);
	}
	public override string Inspect()
	{
		return Value.ToString();
	}

	public static Integer FromString(string s)
	{
		int x = int.Parse(s);
		var i = new Integer(x);
		
		return i;
	}

	public static bool[] Encode(int value)
	{
		var data = new bool[MemSize];
		for (var i = data.Length - 1; i >= 0; i--)
		{
			var place = (int)Math.Pow(2, i);
			int x = 0;
			if (value >= place)
			{
				data[i] = true;
				value = value - place;
			}
			else
			{
				data[i] = false;
			}
			
		}

		return data;
	}

	public static int Decode(bool[] data)
	{
		int val = 0;
		for (var i = 0; i < data.Length; i++)
		{
			//this is a terrible way to do this, but I hope to replace the bitarray with direct 'memory' reads :)
			//so i am halfway in the bad-but-fun direction.
			if (data[i])
			{
				val += (int)Math.Pow(2,i);
			}
		}

		return val;
	}
}