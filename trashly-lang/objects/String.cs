using System.Data.Common;

namespace TrashlyLang.objects;

public class String : Object
{
	//An string is first saved as an 8bit integer (length) followed by that many 8 bit characters (ints)
	public char[] Value;//temp.
	public override ObjectType Type => ObjectType.String;

	public int GetMemSize()
	{
		return 0;
	}

	public string AsNativeString()
	{
		return new string(Value);
	}

	public override string Inspect()
	{
		return AsNativeString();
	}

	public static String Construct(int length, bool[] data)
	{
		//decode here....
		var s = new String();
		s.Value= new char[length];
		for (int i = 0; i < length; i++)
		{
			int j = i*Character.CharMemSize;
			var slice = data[j..(j + Character.CharMemSize)];
			var c = Character.Decode(slice);
			s.Value[i] = c;
		}
		return s;
	}

	public static bool[] Encode(char[] val)
	{
		var length = Integer.Encode(val.Length);
		
		bool[] data = new bool[8 + val.Length * Character.CharMemSize];

		//first 8 bits is the number
		for (int l = 0; l < Integer.MemSize;l++)
		{
			data[l] = length[l];
		}
		//next is the sequence(cough array) of values.
		for (int i = 0; i < val.Length; i++)
		{
			for (int j = 0; j < Character.CharMemSize; j++)
			{
				int k = Integer.MemSize + i * Character.CharMemSize + j;
				char c = val[i];
				var cData = Character.Encode(c);
				data[k] = cData[j];
			}
		}

		return data;
	}

	public static String FromNativeString(string nativeVal)
	{
		String s = new String();
		s.Value = nativeVal.ToCharArray();
		return s;
	}
}