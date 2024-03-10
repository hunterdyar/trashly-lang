using TrashlyLang.ast;
using TrashlyLang.objects;
using Boolean = TrashlyLang.objects.Boolean;
using Object = TrashlyLang.objects.Object;
using String = TrashlyLang.objects.String;

namespace TrashlyLang.memory;

public class Environment
{
	public Memory Memory;
	private Environment _outerEnvironment = null;

	private Dictionary<string, (int loc, ObjectType type)> _dataStore =
		new Dictionary<string, (int loc, ObjectType type)>();

	//ideally, all of my ram would be in an image. but today is not that today, functions will save their AST counterparts.
	private Dictionary<string, Object> _functionStore = new Dictionary<string, Object>();
	public Environment(Memory memory)
	{
		Memory = memory;
	}

	public void Set(string identifier, Object o)
	{
		//"hack" since our memory model does not support the functions themselves.
		if (o.Type == ObjectType.Function)
		{
			if (_functionStore.ContainsKey(identifier))
			{
				throw new Exception($"function {identifier} already exists :(");
			}
			_functionStore.Add(identifier,o);
			return;
		}
		
		if (_dataStore.ContainsKey(identifier))
		{
			throw new Exception($"oops! Can't create {identifier}, it already exists. You feool!");
		}

		var data = Encode(o);
		var size = data.Length;//=MemSize... no need! we trust the encoders.
		var loc = Memory.GetAvailableMemoryLocation(size, true);
		_dataStore.Add(identifier,(loc,o.Type));
		Memory.Write(loc,data);
	}

	public Object Get(string identifier)
	{
		//"hack" for functions
		if (_functionStore.TryGetValue(identifier, out var func))
		{
			return func;
		}
		
		if(_dataStore.TryGetValue(identifier, out var value))
		{
			switch (value.type)
            {
                case ObjectType.Null:
                    return new Null();
                case ObjectType.Int:
					var intData = Memory.Read(value.loc, GetMemorySize(value.type));
                    return Integer.Construct(intData);
                case ObjectType.Bool:
					var boolData = Memory.Read(value.loc, GetMemorySize(value.type));
                    return Boolean.Construct(boolData);
                case ObjectType.String:
					var sizeData = Memory.Read(value.loc, GetMemorySize(ObjectType.Int));
	                int stringLength = Integer.Decode(sizeData);
	                int stringStartLoc = value.loc + GetMemorySize(ObjectType.Int);
	                var stringData = Memory.Read(stringStartLoc, stringLength * GetMemorySize(ObjectType.Character));
	                return String.Construct(stringLength, stringData);
            }
		}
		else
		{
			//if we don't have a variable, we check the outer one.
			if (_outerEnvironment != null)
			{
				return _outerEnvironment.Get(identifier);
			}
		}
		return new Error($"can't get value for {identifier}");
	}

	public void FreeEnvironment()
	{
		foreach (var kvp in _dataStore)
		{
			var loc = kvp.Value.loc;
			var count = GetMemorySize(kvp.Value.type);
			Memory.Free(loc, count);
		}
		_dataStore.Clear();
		_functionStore.Clear();
	}

	//Extending The Environment
	public Environment CreateEnclosedEnvironment()
	{
		var e = new Environment(Memory);
		e._outerEnvironment = this;
		return e;
	}
	
	//Utility

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
		}else if (o is String s)
		{
			return String.Encode(s.Value);
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
			case ObjectType.Character:
				return 8;//utf-8
			default://nulls,functions
				return 0;
		}

		return 0;
	}

	public Function GetFunction(Expression ceFunction)
	{
		throw new NotImplementedException();
	}
}