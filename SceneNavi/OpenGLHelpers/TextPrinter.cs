using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.OpenGLHelpers
{
	public class TextPrinter : IDisposable
	{
		const float lastUsedMax = 300.0f;

		Font font;

		GLControl parentGlControl;
		List<CachedString> cachedStrings;

		bool started, ended;
		bool disposed;

		TextPrinter()
		{
			cachedStrings = new List<CachedString>();
		}

		public TextPrinter(Font font)
			: this()
		{
			this.font = font;
		}

		~TextPrinter()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					foreach (CachedString cached in cachedStrings) cached.Dispose();
				}

				disposed = true;
			}
		}

		public void Print(string text, Vector2d position)
		{
			Print(text, position, Color.Transparent);
		}

		public void Print(string text, Vector2d position, Color backColor)
		{
			CachedString cached = cachedStrings.FirstOrDefault(x => x.String == text && x.BackColor == backColor);

			if (cached == null)
			{
				cached = new CachedString(text, this.font, backColor);
				cachedStrings.Add(cached);
			}

			cached.RefreshClientRect(parentGlControl.ClientRectangle);
			cached.LastUsedAgo += 0.01f;
			cached.Print(position);
		}

		public void Begin(GLControl glControl)
		{
			if (started && !ended) throw new Exception("TextPrinter Begin without Flush");

			parentGlControl = glControl;

			GL.PushMatrix();

			Initialization.CreateViewportAndProjection(Initialization.ProjectionTypes.Orthographic, glControl.ClientRectangle, 0.0f, 300.0f);

			started = true;
			ended = false;
		}

		public void Flush()
		{
			if (!started) throw new Exception("TextPrinter Flush without Begin");

			foreach (CachedString cached in cachedStrings.Where(x => x.LastUsedAgo >= lastUsedMax)) cached.Dispose();
			cachedStrings.RemoveAll(x => x.LastUsedAgo >= lastUsedMax);

			GL.PopMatrix();

			started = false;
			ended = true;
		}

		class CachedString : IDisposable
		{
			const float padding = 4.0f;

			public string String { get; private set; }
			public Color BackColor { get; private set; }

			public int TextureGLID { get; private set; }
			public float LastUsedAgo { get; set; }

			Font currentFont;
			StringFormat stringFormat;
			SizeF stringSize;
			Rectangle clientRect;

			double[] texCoordData, colorData;
			ushort[] indices;

			bool disposed;

			public CachedString(string text, Font font, Color backColor)
			{
				String = text;
				BackColor = backColor;

				string strippedText = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", String.Empty);
				bool hasTags = (text.CompareTo(strippedText) != 0);

				currentFont = font;
				stringFormat = new StringFormat(StringFormat.GenericTypographic);
				if (hasTags) stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

				SizeF tempSize = strippedText.MeasureString(font, stringFormat);
				stringSize = new SizeF((float)Math.Floor(tempSize.Width + padding), (float)Math.Floor(tempSize.Height + padding));

				Bitmap stringBmp = new Bitmap((int)stringSize.Width, (int)stringSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				using (Graphics g = Graphics.FromImage(stringBmp))
				{
					g.Clear(backColor);
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
					g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

					if (hasTags)
						ParseDrawString(g);
					else
						g.DrawString(text, font, Brushes.White, padding / 2.0f, padding / 2.0f, stringFormat);
				}

				System.Drawing.Imaging.BitmapData bmpData = stringBmp.LockBits(new Rectangle(0, 0, stringBmp.Width, stringBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, stringBmp.PixelFormat);

				TextureGLID = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, TextureGLID);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, stringBmp.Width, stringBmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
				stringBmp.UnlockBits(bmpData);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

				texCoordData = new double[]
				{
					0.0f, 1.0f,
					1.0f, 1.0f,
					1.0f, 0.0f,
					0.0f, 0.0f
				};

				colorData = new double[]
				{
					1.0f, 1.0f, 1.0f, 1.0f,
					1.0f, 1.0f, 1.0f, 1.0f,
					1.0f, 1.0f, 1.0f, 1.0f,
					1.0f, 1.0f, 1.0f, 1.0f
				};

				indices = new ushort[]
				{
					0, 1, 2, 3
				};

				LastUsedAgo = 0.0f;

				disposed = false;
			}

			~CachedString()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					if (disposing)
					{
						if (GL.IsTexture(TextureGLID)) GL.DeleteTexture(TextureGLID);
					}

					disposed = true;
				}
			}

			internal void ParseDrawString(Graphics g)
			{
				Color currentColor = Color.White;
				PointF position = new PointF(padding / 2.0f, padding / 2.0f);
				for (int i = 0; i < String.Length; i++)
				{
					char ch = String[i];
					string chString = new string(new char[] { ch });
					SizeF chSize = chString.MeasureString(currentFont, stringFormat);

					if (ch == '\n')
					{
						position.X = (padding / 2.0f);
						position.Y += chSize.Height;
						continue;
					}
					else if (ch == '<')
					{
						int sepIndex = String.IndexOf(':', i + 1);
						int endIndex = String.IndexOf('>', sepIndex);
						string tag = String.Substring(i + 1, (sepIndex - i) - 1);
						string value = String.Substring(sepIndex + 1, (endIndex - sepIndex) - 1);
						switch (tag)
						{
							case "color":
								string[] colorValues = value.Split(new char[] { ',', ' ' });
								if (colorValues.Length > 1)
								{
									byte[] colors = new byte[colorValues.Length];
									for (int ci = 0; ci < colors.Length; ci++)
									{
										byte.TryParse(colorValues[ci], out colors[ci]);
									}
									currentColor = Color.FromArgb(colorValues.Length == 4 ? colors[3] : 255, colors[0], colors[1], colors[2]);
								}
								else
									currentColor = Color.White;
								break;
						}

						i = endIndex;
					}
					else
					{
						using (SolidBrush brush = new SolidBrush(currentColor))
						{
							g.DrawString(chString, currentFont, brush, position, stringFormat);
						}
						position.X += chSize.Width;
					}
				}
			}

			internal void RefreshClientRect(Rectangle newClientRect)
			{
				clientRect = newClientRect;
			}

			public void Print(Vector2d location)
			{
				LastUsedAgo = 0.0f;

				if (location.X < 0) location.X = (clientRect.Right - stringSize.Width) + location.X;
				if (location.Y < 0) location.Y = (clientRect.Bottom - stringSize.Height) + location.Y;

				double[] vertexData = new double[]
				{
					location.X + 0.0,               location.Y + stringSize.Height,
					location.X + stringSize.Width,  location.Y + stringSize.Height,
					location.X + stringSize.Width,  location.Y + 0.0,
					location.X + 0.0,               location.Y + 0.0
				};

				GL.PushAttrib(AttribMask.AllAttribBits);
				GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);

				GL.UseProgram(0);
				GL.Enable(EnableCap.Texture2D);
				GL.BindTexture(TextureTarget.Texture2D, TextureGLID);
				GL.Disable(EnableCap.Lighting);
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
				GL.Enable(EnableCap.AlphaTest);

				GL.EnableClientState(ArrayCap.TextureCoordArray);
				GL.EnableClientState(ArrayCap.ColorArray);
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.TexCoordPointer(2, TexCoordPointerType.Double, 0, ref texCoordData[0]);
				GL.ColorPointer(4, ColorPointerType.Double, 0, ref colorData[0]);
				GL.VertexPointer(2, VertexPointerType.Double, 0, ref vertexData[0]);
				GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedShort, ref indices[0]);

				GL.DisableClientState(ArrayCap.TextureCoordArray);
				GL.DisableClientState(ArrayCap.ColorArray);
				GL.DisableClientState(ArrayCap.VertexArray);

				GL.PopClientAttrib();
				GL.PopAttrib();
			}
		}
	}
}
