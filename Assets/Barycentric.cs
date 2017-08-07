using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DrawTriangles
{
	class Barycentric
	{
		public int TotalIterations;
		public int CurrentIteration;

		System.Random rand;
		Rect dimensions;

		public Barycentric(Vector2[] points, int Count)
		{
			rand = new System.Random();
			
			dimensions = DrawTriangles.getDim(points);

			for(int i=0;i<Count;i++)
			{
				DrawTriangles.BaryIterate();
				UnityEngine.Color[] randColors = DrawTriangles.randomColors(points.Length,rand);
				BaryFillTriangle(points, randColors);
			}
		}
		

		public void BaryFillTriangle(Vector2[] corners, UnityEngine.Color[] corner_eles)
		{
			int startX = (int)(dimensions.x - (dimensions.width * 0.5f));
			int endX = (int)(startX + dimensions.width);

			int startY = (int)(dimensions.y - (dimensions.height * 0.5f));
			int endY = (int)(startY + dimensions.height);
			int[] eles = new int[corner_eles.Length];
       
			for (int x = startX; x < endX; x++)
			{
				for (int y = startY; y < endY; y++)
				{
					PointInTriangle(new Vector2(x, y), corners, corner_eles);
				}
			}
		}

		public bool PointInTriangle(Vector2 loc, Vector2[] corners, UnityEngine.Color[] corner_eles)//(Point A, Point B, Point C, Point P)
		{
			double s1 = corners[2].y - corners[0].y;
			double s2 = corners[2].x - corners[0].x;
			double s3 = corners[1].y - corners[0].y;
			double s4 = loc.y - corners[0].y;

			double w1 = (corners[0].x * s1 + s4 * s2 - loc.x * s1) / (s3 * s2 - (corners[1].x - corners[0].x) * s1);
			if (w1 > 1 || w1 < 0)
				return false;
			double w2 = (s4 - w1 * s3) / s1;
			if (w2 > 1 || w2 < 0)
				return false;
			double w3 = 1 - w2 - w1;
			if (w3 > 1 || w3 < 0)
				return false;

			UnityEngine.Color newColor = new UnityEngine.Color();
			newColor.r = (float)(((w1 * corner_eles[0].r) + (w2 * corner_eles[1].r) + (w3 * corner_eles[2].r)));
			newColor.g = (float)(((w1 * corner_eles[0].g) + (w2 * corner_eles[1].g) + (w3 * corner_eles[2].g)));
			newColor.b = (float)(((w1 * corner_eles[0].b) + (w2 * corner_eles[1].b) + (w3 * corner_eles[2].b)));

			DrawTriangles.queueColorChange(loc,newColor);
			//bgVal[(y * bg.width) + x] = newColor;
			//ushort newEle = (ushort)(((w1 * corner_eles[0]) + (w2 * corner_eles[1]) + (w3 * corner_eles[2])));
			//hm.RequestsetHeight(new Vector2(x, y), newEle);

			return true;
		}
	}
}
