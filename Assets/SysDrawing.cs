using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DrawTriangles
{
	public class SysDrawing
	{
		System.Drawing.Bitmap bitmap;
		System.Drawing.Graphics graphics;
		
		System.Random rand;

		Rect dimensions;
		public SysDrawing(Vector2[] points, int Count)
		{
			rand = new System.Random();
			
			dimensions = DrawTriangles.getDim(points);
			bitmap = new Bitmap((int)Mathf.Round(DrawTriangles.ImageSize.width), (int)Mathf.Round(DrawTriangles.ImageSize.height));
            graphics = System.Drawing.Graphics.FromImage(bitmap);
			
			for(int i=0;i<Count;i++)
			{
			
				DrawTriangles.SysIterate();

				UnityEngine.Color[] randColors = DrawTriangles.randomColors(points.Length,rand);
				DrawTriangle(points, randColors);
			}
		}
		
		

		public void DrawTriangle(Vector2[] points, UnityEngine.Color[] colors)
		{
			System.Drawing.Point[] d_points = new System.Drawing.Point[points.Length];
			
			Vector2 start = new Vector2(dimensions.x-(dimensions.width*0.5f),dimensions.y-(dimensions.height*0.5f));
			for (int i = 0; i < points.Length; i++)
			{
				d_points[i] = new System.Drawing.Point((int)Mathf.Round(points[i].x-start.x), (int)Mathf.Round(points[i].y-start.y));
			}
			System.Drawing.Color[] d_colors = new System.Drawing.Color[colors.Length];
			int totR = 0;
			int totG = 0;
			int totB = 0; 
			for (int i = 0; i < colors.Length; i++)
			{
				d_colors[i] = System.Drawing.Color.FromArgb(255, (int)Mathf.Round(colors[i].r * 255), (int)Mathf.Round(colors[i].g * 255), (int)Mathf.Round(colors[i].b * 255));
				totR += d_colors[i].R;
				totG += d_colors[i].G;
				totB += d_colors[i].B;
			}
			totR = totR/colors.Length;
			totG = totG/colors.Length;
			totB = totB/colors.Length;
			System.Drawing.Color c_color = System.Drawing.Color.FromArgb(255,totR,totG,totB);
			GraphicsPath path = new GraphicsPath();
			path.AddLines(d_points);
			PathGradientBrush pthGrBrush = new PathGradientBrush(path);
			pthGrBrush.SurroundColors = d_colors;
			pthGrBrush.CenterColor = c_color;
			graphics.FillPath(pthGrBrush, path);

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
			int bytes = data.Stride * bitmap.Height;
			byte[] pixelData = new byte[bytes];
			Marshal.Copy(data.Scan0, pixelData, 0, bytes);
			bitmap.UnlockBits(data);

			int texWidth = bitmap.Width;
			int lineWidth = texWidth * 4;

			Vector2 curr = new Vector2();
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					curr.y = y + start.y;
					curr.x = x + start.x;
					int pixelDataPos = ((y * lineWidth) + (x * 4));
					UnityEngine.Color newColor = new UnityEngine.Color(pixelData[pixelDataPos]/255f,pixelData[pixelDataPos+1]/255f,pixelData[pixelDataPos+2]/255f);
					if(newColor.r > 0 || newColor.b > 0 || newColor.g > 0)
					{
						DrawTriangles.queueColorChange(curr, newColor);
					}

				}
			}
		}
	}
}
