using System;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Text;

namespace UNIRAST
{
	public class BMP
	{
		public enum CompressTypes : uint
		{
			RGB = 0,
			RLE8,
			RLE4,
			BITFIELDS,
			JPEG,
			PNG
		}

		public BMP(int width, int height, int spp)
		{
			this.Header = new BMPHeader();
			this.Header.Magic = Encoding.UTF8.GetBytes("BM");

			this.DIB = new BMPDIB();
			this.DIB.Width = width;
			this.DIB.Height = height;
			this.DIB.BitSPP = (ushort)spp;
			this.DIB.HorizontalResolution = width;
			this.DIB.VerticalResolution = height;
			this.DIB.Planes = 1;
			this.DIB.CompressType = CompressTypes.RGB;
			this.DIB.Colors = 0;
			this.DIB.ImportantColors = 0;
		}

		public BMP(byte[] data)
		{
			this.Read(new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(data)));
		}

		public BMP(int width, int height, int spp, byte[] body)
			: this(width, height, spp)
		{
			this.ReadBody(new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(body)));
		}

		public void Read(EndianBinaryReader reader)
		{
			this.Header = new BMPHeader();

			this.Header.Magic = reader.ReadBytes(2);
			if (Encoding.UTF8.GetString(this.Header.Magic, 0, 2).CompareTo("BM") != 0)
				throw new Exception("Invalid Magic Bytes");

			this.Header.FileSize = reader.ReadUInt32();
			this.Header.Creator1 = reader.ReadUInt16();
			this.Header.Creator2 = reader.ReadUInt16();
			this.Header.Offset   = reader.ReadUInt32();

			this.DIB = new BMPDIB();
			this.DIB.Size = reader.ReadUInt32();
			this.DIB.Width = reader.ReadInt32();
			this.DIB.Height = reader.ReadInt32();
			this.DIB.Planes = reader.ReadUInt16();
			this.DIB.BitSPP = reader.ReadUInt16();
			this.DIB.CompressType = (CompressTypes)reader.ReadUInt32();
			this.DIB.ByteSize = reader.ReadUInt32();
			this.DIB.HorizontalResolution = reader.ReadInt32();
			this.DIB.VerticalResolution = reader.ReadInt32();
			this.DIB.Colors = reader.ReadUInt32();
			this.DIB.ImportantColors = reader.ReadUInt32();

			ReadBody(reader);
		}

		public void ReadBody(EndianBinaryReader reader)
		{
			var colorTable = new BMPPixel[256];
			if (this.DIB.Colors > 0)
			{
				for (int i = 0; i < this.DIB.Colors; i++)
				{
					colorTable[i].B = reader.ReadByte();
					colorTable[i].G = reader.ReadByte();
					colorTable[i].R = reader.ReadByte();
					colorTable[i].Junk = reader.ReadByte();
				}
			}

			var bytes = this.DIB.BitSPP / 8;

			reader.Seek((int)this.Header.Offset, SeekOrigin.Begin);

			this.Pixels = new BMPPixel[this.DIB.Height][];
			for (int y = this.DIB.Height - 1; y >= 0; y--)
			{
				this.Pixels[y] = new BMPPixel[this.DIB.Width];
				for (int x = 0; x < this.DIB.Width; x++)
				{
					var pixel = reader.ReadBytes(bytes);

					this.Pixels[y][x] = new BMPPixel()
					{
						B = pixel[0],
						G = pixel[1],
						R = pixel[2]
					};
				}
			}
		}

		public new String ToString()
		{
			var result = new StringWriter();

			result.WriteLine("BMP");
			result.WriteLine("FileSize: {0}", this.Header.FileSize);
			result.WriteLine("Offset: {0}", this.Header.Offset);
			result.WriteLine("Size: {0}x{1}", this.DIB.Width, this.DIB.Height);
			result.WriteLine("Planes: {0}", this.DIB.Planes);
			result.WriteLine("BitSPP: {0}", this.DIB.BitSPP);
			result.WriteLine("Compress: {0}", this.DIB.CompressType.ToString());
			result.WriteLine("ByteSize: {0}", this.DIB.ByteSize);
			result.WriteLine("Resolution: {0}x{1}", this.DIB.HorizontalResolution, this.DIB.VerticalResolution);
			result.WriteLine("Colors: {0}", this.DIB.Colors);
			result.WriteLine("ImportantColors: {0}", this.DIB.ImportantColors);

			return result.ToString();
		}

		public BMPHeader Header;
		public BMPDIB DIB;
		public BMPPixel[][] Pixels;

		public struct BMPHeader
		{
			public byte[] Magic;
			public uint FileSize;
			public ushort Creator1;
			public ushort Creator2;
			public uint Offset;
		}

		public struct BMPDIB
		{
			public uint Size;
			public int Width;
			public int Height;
			public ushort Planes;
			public ushort BitSPP;
			public CompressTypes CompressType;
			public uint ByteSize;
			public int HorizontalResolution;
			public int VerticalResolution;
			public uint Colors;
			public uint ImportantColors;
		}

		public struct BMPPixel
		{
			public byte R;
			public byte G;
			public byte B;
			public byte Junk;
		}
	}
}
