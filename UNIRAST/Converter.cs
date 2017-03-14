using System;
namespace UNIRAST
{
	public class Converter
	{
		public Converter()
		{
		}

		public static BMP UNIRAST2BMP(UNIRAST unirast)
		{
			var result = new BMP(
				(int)unirast.Pages[0].Width,
				(int)unirast.Pages[0].Height,
				unirast.Pages[0].BPP
			);

			result.Pixels = new BMP.BMPPixel[unirast.Pages[0].Height][];

			for (int y = 0; y < result.DIB.Height; y++)
			{
				result.Pixels[y] = new BMP.BMPPixel[unirast.Pages[0].Width];

				for (int x = 0; x < result.DIB.Width; x++)
				{
					result.Pixels[y][x].R = unirast.Pages[0].Pixels[y][x].R;
					result.Pixels[y][x].G = unirast.Pages[0].Pixels[y][x].G;
					result.Pixels[y][x].B = unirast.Pages[0].Pixels[y][x].B;
				}
			}

			return result;
		}

		public static UNIRAST BMP2UNIRAST(BMP bmp)
		{
			var result = new UNIRAST(bmp.DIB.Width, bmp.DIB.Height, bmp.DIB.BitSPP);

			result.Pages[0].Pixels = new UNIRAST.Page.Pixel[bmp.DIB.Height][];
			for (int y = 0; y < bmp.DIB.Height; y++)
			{
				result.Pages[0].Pixels[y] = new UNIRAST.Page.Pixel[bmp.DIB.Width];
				for (int x = 0; x < bmp.DIB.Width; x++)
				{
					result.Pages[0].Pixels[y][x].R = bmp.Pixels[y][x].R;
					result.Pages[0].Pixels[y][x].G = bmp.Pixels[y][x].G;
					result.Pages[0].Pixels[y][x].B = bmp.Pixels[y][x].B;
				}
			}

			return result;
		}

		public static UNIRAST Bitmap2UNIRAST(Bitmap bitmap)
		{
			var result = new UNIRAST(bitmap.Width, bitmap.Height, 24);

			result.Pages[0].Pixels = new UNIRAST.Page.Pixel[bitmap.Height][];
			for (int y = 0; y < bitmap.Height; y++)
			{
				result.Pages[0].Pixels[y] = new UNIRAST.Page.Pixel[bitmap.Width];
				for (int x = 0; x < bitmap.Width; x++)
				{
					result.Pages[0].Pixels[y][x].R = bitmap.Pixels[y][x].R;
					result.Pages[0].Pixels[y][x].G = bitmap.Pixels[y][x].G;
					result.Pages[0].Pixels[y][x].B = bitmap.Pixels[y][x].B;
				}
			}

			return result;
		}
	}
}
