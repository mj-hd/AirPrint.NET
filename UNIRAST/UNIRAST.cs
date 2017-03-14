using System;
using System.IO;
using System.Text;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.Collections.Generic;

namespace UNIRAST
{
	public class UNIRAST
	{
		public class Page
		{
			public Page(int width, int height, int bitSpp)
			{
				// TODO: support more options
				this.BPP = (byte)bitSpp;
				this.Colorspace = 1;
				this.Duplex = 0;
				this.Quality = 4;
				this.Width = (uint)width;
				this.Height = (uint)height;
				this.DPI = 1;
			}

			public Page(int width, int height, int bitSpp, uint dpi)
				: this(width, height, bitSpp)
			{
				this.DPI = dpi;
			}

			public Page(EndianBinaryReader reader)
			{
				this.Read(reader);
			}

			public void Read(EndianBinaryReader reader)
			{
				this.BPP = reader.ReadByte();
				this.Colorspace = reader.ReadByte();
				this.Duplex = reader.ReadByte();
				this.Quality = reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadUInt32();
				this.Width = reader.ReadUInt32();
				this.Height = reader.ReadUInt32();
				this.DPI = reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();

				if (this.BPP != 24)
					throw new Exception("Unsupported Color Depth");

				var bpp = this.BPP / 8;
				var widthBytes = this.Width * bpp;

				this.Pixels = new Pixel[this.Height][];

				byte line = 0;
				for (int y = 0; y < this.Height; y++)
				{
					line = reader.ReadByte();
					this.Pixels[y] = new Pixel[widthBytes];

					for (int x = 0; x < this.Width; x++)
					{
						var pack = (Int16)reader.ReadByte();

						if (pack == -128)
						{
							break;
						}
						if (0 <= pack && pack <= 127)
						{
							var block = reader.ReadBytes(bpp);

							while (pack-- >= 0)
							{
								this.Pixels[y][x].R = block[0];
								this.Pixels[y][x].G = block[1];
								this.Pixels[y][x].B = block[2];

								// TODO: Support 8

								if (++x >= this.Width) break;
							}
						} else if (0 > pack && pack <= -127) {
							pack *= -1;

							while (pack-- >= 0)
							{
								var block = reader.ReadBytes(bpp);
								this.Pixels[y][x].R = block[0];
								this.Pixels[y][x].G = block[1];
								this.Pixels[y][x].B = block[2];

								// TODO: Support 8

								if (++x >= this.Width) break;
							}
						}
					}

					while(line-- >= 0)
					{
						if (++y >= this.Height) break;
						this.Pixels[y] = this.Pixels[y - 1];
					}
				}
			}

			public void Write(EndianBinaryWriter writer)
			{
				writer.Write(this.BPP);
				writer.Write(this.Colorspace);
				writer.Write(this.Duplex);
				writer.Write(this.Quality);
				writer.Write((uint)0);
				writer.Write((uint)0);
				writer.Write(this.Width);
				writer.Write(this.Height);
				writer.Write(this.DPI);
				writer.Write((uint)0);
				writer.Write((uint)0);

				for (int y = 0; y < this.Height; y++)
				{
					writer.Write((byte)0);

					for (int x = 0; x < this.Width; x++)
					{
						writer.Write((byte)0);

						writer.Write((byte)this.Pixels[y][x].R);
						writer.Write((byte)this.Pixels[y][x].G);
						writer.Write((byte)this.Pixels[y][x].B);

						// TODO: Support 8
					}
				}
			}

			public void RotateLeft()
			{
				var pixels = (Pixel[][])this.Pixels.Clone();

				var origWidth = this.Width;
				var origHeight = this.Height;

				this.Width = this.Height;
				this.Height = origWidth;

				this.Pixels = new Pixel[this.Height][];

				for (var y = 0; y < this.Height; y++)
				{
					this.Pixels[y] = new Pixel[this.Width];

					for (var x = 0; x < this.Width; x++)
					{
						this.Pixels[y][x] = pixels[x][y];
					}
				}
			}

			public void RotateRight()
			{
				var pixels = (Pixel[][])this.Pixels.Clone();

				var origWidth = this.Width;
				var origHeight = this.Height;

				this.Width = this.Height;
				this.Height = origWidth;

				this.Pixels = new Pixel[this.Height][];

				for (var y = 0; y < this.Height; y++)
				{
					this.Pixels[y] = new Pixel[this.Width];

					for (var x = 0; x < this.Width; x++)
					{
						this.Pixels[y][x] = pixels[this.Width - x - 1][y];
					}
				}
			}

			public new String ToString()
			{
				var result = new StringWriter();

				result.WriteLine("Page:");
				result.WriteLine(" BPP: {0}", this.BPP);
				result.WriteLine(" Colorspace: {0}", this.Colorspace);
				result.WriteLine(" Duplex: {0}", this.Duplex);
				result.WriteLine(" Quality: {0}", this.Quality);
				result.WriteLine(" Size: {0}x{1}", this.Width, this.Height);
				result.WriteLine(" DPI: {0}", this.DPI);

				return result.ToString();
			}

			public byte BPP;
			public byte Colorspace;
			public byte Duplex;
			public byte Quality;
			//public uint Unknown0;
			//public uint Unknown1;
			public uint Width;
			public uint Height;
			public uint DPI;
			//public uint Unknown2;
			//public uint Unknown3;

			public Pixel[][] Pixels;

			public struct Pixel
			{
				public byte R;
				public byte G;
				public byte B;
			}
		}

		public UNIRAST(int width, int height, int bitSpp)
		{
			this.PageCount = 1;
			this.Pages = new List<Page>();
			this.Pages.Add(new Page(width, height, bitSpp));
		}

		public UNIRAST(byte[] data)
		{
			this.Read(new EndianBinaryReader(EndianBitConverter.Big, new MemoryStream(data)));
		}

		public void Read(EndianBinaryReader reader)
		{
			var identifier = Encoding.UTF8.GetString(reader.ReadBytes(8), 0, 8);

			if (identifier.CompareTo("UNIRAST") != 0)
				throw new Exception("Invalid Header");

			this.PageCount = reader.ReadUInt32();

			this.Pages = new List<Page>();
			for (int i = 0; i < this.PageCount; i++)
			{
				this.Pages.Add(new Page(reader));
			}
		}

		public void Write(EndianBinaryWriter writer)
		{
			writer.Write(Encoding.UTF8.GetBytes("UNIRAST"));
			writer.Write((byte)0);

			writer.Write(this.PageCount);
			foreach(Page page in this.Pages)
			{
				page.Write(writer);
			}
		}

		public byte[] ToBytes()
		{
			var buffer = new MemoryStream();

			this.Write(new EndianBinaryWriter(EndianBitConverter.Big, buffer));

			return buffer.ToArray();
		}

		public new String ToString()
		{
			var result = new StringWriter();

			result.WriteLine("UNIRAST");
			result.WriteLine("PageCount: {0}", this.PageCount);

			foreach(Page page in this.Pages)
			{
				result.WriteLine(page.ToString());
			}

			return result.ToString();
		}

		// File Header
		public uint PageCount;
		public List<Page> Pages;
	}
}
