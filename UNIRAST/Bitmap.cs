using System;
namespace UNIRAST
{
	public class Bitmap
	{
		public Bitmap(int width, int height, byte[] data)
		{
			int index = 0;

			this.Width = width;
			this.Height = height;

			if (width * height >= data.Length) throw new Exception("Muri");

			this.Pixels = new Pixel[height][];
			for (int y = 0; y < height; y++)
			{
				this.Pixels[y] = new Pixel[width];
				for (int x = 0; x < width; x++)
				{
					this.Pixels[y][x] = new Pixel()
					{
						R = data[index],
						G = data[index + 1],
						B = data[index + 2]
					};

					index += 4;
				}
			}
		}

		public Pixel[][] Pixels;
		public int Width;
		public int Height;

		public struct Pixel
		{
			public byte R;
			public byte G;
			public byte B;
		}
	}
}
