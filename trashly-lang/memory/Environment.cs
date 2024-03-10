using TrashlyLang.objects;
using Boolean = TrashlyLang.objects.Boolean;
using Object = TrashlyLang.objects.Object;

namespace TrashlyLang.memory;

public class Environment
{
	public Memory Memory;

	private Dictionary<string, (int loc, ObjectType type)> _dataStore =
		new Dictionary<string, (int loc, ObjectType type)>();
	public Environment(Memory memory)
	{
		Memory = memory;
	}

	public void Set(string identifier, Object o)
	{
		if (_dataStore.ContainsKey(identifier))
		{
			throw new Exception($"oops! Can't create {identifier}, it already exists. You feool!");
		}
		var loc = Memory.GetAvailableMemoryLocation(GetMemorySize(o.Type),true);
		_dataStore.Add(identifier,(loc,o.Type));
		Memory.Write(loc,Encode(o));
	}

	public Object Get(string identifier)
	{
		if(_dataStore.TryGetValue(identifier, out var value))
		{
			var data = Memory.Read(value.loc, GetMemorySize(value.type));
			return MakeObject(value.type, data);
		}
		else
		{
			return new Error($"can't get value for {identifier}");
		}
	}
	
	//these can move into the Object class, i guess?
	private Object MakeObject(ObjectType ot, bool[] data)
	{
		switch (ot)
		{
			case ObjectType.Null:
				return new Null();
			case ObjectType.Int:
				return Integer.Construct(data);
			case ObjectType.Bool:
				return Boolean.Construct(data);
		}

		return new Null();
	}

	public bool[] Encode(Object o)
	{
		
		if (o is Integer i)
		{
			return Integer.Encode(i.Value);
		}else if (o is Boolean b)
		{
			if (b.Value)
			{
				return new bool[] { true };
			}
			else
			{
				return new bool[] { false };
			}
		}

		return Array.Empty<bool>();
	}

	public int GetMemorySize(ObjectType ot)
	{
		switch (ot)
		{
			case ObjectType.Bool:
				return Boolean.MemSize;//1
			case ObjectType.Int:
				return Integer.MemSize;//8
			case ObjectType.Null:
				return 0;
		}

		return 0;
	}
}