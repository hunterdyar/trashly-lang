namespace TrashlyLang.objects;

public class Character : Object
{
	public static int CharMemSize = 8;
	public static bool[] Encode(char letter)
	{
		int i = (int)letter;
		return Integer.Encode(i);
	}

	public static char Decode(bool[] data)
	{
		int i = Integer.Decode(data);
		return (char)i;
	}
	
}