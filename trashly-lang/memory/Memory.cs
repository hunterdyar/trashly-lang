using SkiaSharp;
namespace TrashlyLang.memory;

//Blue is data. 0 is off, anything else (255) is on.
//Green is allocation. 0 is free. >0 (255) is used.
//Red is for errors? or?
public class Memory
{
	//public Action<Memory> OnMemoryChanged;
	public readonly int Width;
	public readonly int Height;
	private int _totalBits;
	public SKBitmap MemoryImage;
	public Memory(int w, int h)
	{
		Width = w;
		Height = h;
		_totalBits = w * h;
		MemoryImage = new SKBitmap(w, h, SKColorType.Rgb888x, SKAlphaType.Opaque);
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				MemoryImage.SetPixel(i,j,SKColors.Blue);
			}
		}
	}

	public (int x, int y) PositionToLocation(int pos)
	{
		int x = pos % Width;
		int y = pos / Width;
		return (x, y);
	}

	public int LocationToPosition(int x, int y)
	{
		return x + Width * y;
	}
	
	public int GetAvailableMemoryLocation(int count, bool reserve=true)
	{
		int search = 0;
		for (int i = 0; i < _totalBits; i++)
		{
			bool available = true;
			for (int j = 0; j < count; j++)
			{
				var loc = PositionToLocation(i+j);
				var color = MemoryImage.GetPixel(loc.x,loc.y);
				if (color.Green > 0)
				{
					available = false;
					//this is unavailable.
					i += j;//We can skip i ahead by j, since they're searched already.
					break;//break this.
				}
			}

			if (available)
			{
				if (reserve)
				{
					Reserve(i, count);
				}

				return i;
			}
		}
		//out of memory error!
		throw new Exception("hmm. Guess we're out of memory!");
		return 0;
	}

	public void Write(int position, bool[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			var d = data[i];
			var loc = PositionToLocation(position + i);
			SetBit(loc.x,loc.y,d);
		}
	}

	public bool[] Read(int position, int count)
	{
		var data = new bool[count];
		for (int i = 0; i < count; i++)
		{
			var loc = PositionToLocation(position + i);
			var c = MemoryImage.GetPixel(loc.x, loc.y);
			data[i] = c.Blue > 0;
		}
		return data;
	}

	//un-free
	private void Reserve(int position, int count)
	{
		for (int i = 0; i < count; i++)
        {
        	var loc = PositionToLocation(position + i);
        	var color = MemoryImage.GetPixel(loc.x, loc.y);
        	color = color.WithGreen(1);//make available.
        	MemoryImage.SetPixel(loc.x,loc.y,color);
        }
	}
	public void Free(int position, int count)
	{
		for (int i = 0; i < count; i++)
		{
			var loc = PositionToLocation(position + i);
			var color = MemoryImage.GetPixel(loc.x, loc.y);
			color = color.WithGreen(0);//make available.
			MemoryImage.SetPixel(loc.x,loc.y,color);
		}
	}

	private void SetBit(int x, int y, bool b)
	{
		var color = MemoryImage.GetPixel(x, y);
		if (color.Green > 1)
		{
			//warning! Setting data that is not free!
			color = color.WithRed(1);
		}
		color = color.WithBlue(b ? Byte.MaxValue : Byte.MinValue);
		MemoryImage.SetPixel(x,y,color);
	}

	public async Task Export()
	{
		var memStream = new MemoryStream();
		using (SKManagedWStream stream = new SKManagedWStream(memStream))
		{
			MemoryImage.Encode(stream,SKEncodedImageFormat.Png, 10);
			await File.WriteAllBytesAsync("memory.png", memStream.ToArray());
		}
	}
}