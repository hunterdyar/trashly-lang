using SkiaSharp;
namespace TrashlyLang.memory;

public class Memory
{
	public readonly int Width;
	public readonly int Height;
	public SKBitmap MemoryImage;
	private int lastUsed = 0;
	public Memory(int w, int h)
	{
		Width = w;
		Height = h;
		MemoryImage = new SKBitmap(w, h, SKColorType.Rgb888x, SKAlphaType.Opaque);
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

	public int GetAvailableMemoryLocation(int count)
	{
		var used = lastUsed;
		lastUsed = used+count;
		return count;
	}

	public void Write(int position, bool[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			var d = data[i];
			var loc = PositionToLocation(position + i);
			MemoryImage.SetPixel(loc.x, loc.y, ColorFromBool(d));
		}
	}

	public bool[] Read(int position, int count)
	{
		var data = new bool[count];
		for (int i = 0; i < count; i++)
		{
			var loc = PositionToLocation(position + i);
			var c = MemoryImage.GetPixel(loc.x, loc.y);
			data[i] = ColorToBool(c);
		}

		return data;
	}

	private SKColor ColorFromBool(bool b)
	{
		if (b)
		{
			return SKColors.White;
		}
		else
		{
			return SKColors.Black;
		}
	}

	private bool ColorToBool(SKColor color)
	{
		if (color.Blue > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
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