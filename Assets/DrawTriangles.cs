using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;

namespace DrawTriangles
{
	public class DrawTriangles : MonoBehaviour
	{


		Color[] colors;
		public static Rect ImageSize;
		// Use this for initialization
		GameObject bg_obj;
		Texture2D bg;
		bool TextureNeedsUpdate = false;
		
		private int TotalIterations;
		private static volatile int SysIterations;
		private DateTime SysStartTime;

		private static volatile int BaryIterations;
		private DateTime BaryStartTime;

		Text SysProgress;
		Text SysTime;
		Text BaryProgress;
		Text BaryTime;

		public struct colorChange
		{
			public Vector2 loc;
			public Color color;
			public colorChange(Vector2 LOC, Color COLOR)
			{
				loc = LOC;
				color = COLOR;
			}
		}
		private static volatile List<colorChange> toBeSet;
		void Start()
		{
			ImageSize = new Rect(512, 512, 1024, 1024);

			bg_obj = GameObject.Find("BG").gameObject;
			bg = new Texture2D((int)ImageSize.width, (int)ImageSize.width, TextureFormat.ARGB32, false);
			bg.filterMode = FilterMode.Point;
			bg.wrapMode = TextureWrapMode.Clamp;
			colors = bg.GetPixels();

			toBeSet = new List<colorChange>();

			SysProgress = GameObject.Find("SysProgress").GetComponent<Text>();
			SysTime = GameObject.Find("SysTime").GetComponent<Text>();
			BaryProgress = GameObject.Find("BaryProgress").GetComponent<Text>();
			BaryTime = GameObject.Find("BaryTime").GetComponent<Text>();

		}

		// Update is called once per frame
		void Update()
		{
			if (toBeSet.Count > 0)
			{
				SysProgress.text = SysIterations + "/" + TotalIterations;
				if(SysIterations < TotalIterations)
					SysTime.text = (int)((DateTime.Now-SysStartTime).TotalSeconds*100)/100f + "s";

				BaryProgress.text = BaryIterations + "/" + TotalIterations;
				if(BaryIterations < TotalIterations)
					BaryTime.text = (int)((DateTime.Now-BaryStartTime).TotalSeconds*100)/100f + "s";

				Debug.Log("Setting:" + toBeSet.Count);
				List<colorChange> tempSet;
				lock (toBeSet)
				{
					tempSet = new List<colorChange>(toBeSet);
					toBeSet.Clear();
				}
				for (int i = 0; i < tempSet.Count; i++)
				{
					setColor(tempSet[i].loc, tempSet[i].color);
				}
				UpdateText();
			}
		}


		public static void queueColorChange(Vector2 loc, Color newColor)
		{
			lock (toBeSet)
			{
				toBeSet.Add(new colorChange(loc, newColor));
			}
		}

		void setColor(Vector2 loc, Color newColor)
		{
			colors[(int)((loc.y * ImageSize.width) + loc.x)] = newColor;
		}
		
		public static Rect getDim(Vector2[] points)
		{
			Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 max = new Vector2(float.MinValue, float.MinValue);
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].x < min.x)
					min.x = points[i].x;
				if (points[i].x > max.x)
					max.x = points[i].x;
				if (points[i].y < min.y)
					min.y = points[i].y;
				if (points[i].y > max.y)
					max.y = points[i].y;
			}
			Vector2 mid = new Vector2(min.x + ((max.x - min.x) * 0.5f), min.y + ((max.y - min.y) * 0.5f));
			return new Rect(mid.x, mid.y, (max.x - min.x), (max.y - min.y));
		}
		
		public static Color[] randomColors(int howMany, System.Random rand)
		{
			Color[] colors = new Color[howMany];
			for(int i=0;i<howMany;i++)
			{
				colors[i] = new UnityEngine.Color((float)rand.NextDouble(),(float)rand.NextDouble(),(float)rand.NextDouble());
			}
			return colors;
		}

		public void UpdateText()
		{
			bg.SetPixels(colors);
			bg.Apply();
			bg_obj.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = bg;
		}

		public void StartDraw()
		{
			TotalIterations = 100;
			SysStartTime = DateTime.Now;
			new Thread(() => 
			{
				Thread.CurrentThread.IsBackground = true; 
				startSysDrawing();
			}).Start();

			BaryStartTime = DateTime.Now;
			new Thread(() => 
			{
				Thread.CurrentThread.IsBackground = true; 
				startBaryDrawing();
			}).Start();
		}

		private void startSysDrawing()
		{
			Vector2[] points = new Vector2[3];
			points[0] = new Vector2(256f, 256f);
			points[1] = new Vector2(64f, 960f);
			points[2] = new Vector2(448f, 960f);
			SysDrawing sys = new SysDrawing(points, TotalIterations);
		}

		private void startBaryDrawing()
		{
			Vector2[] points = new Vector2[3];
			points[0] = new Vector2(768f, 256f);
			points[1] = new Vector2(576f, 960f);
			points[2] = new Vector2(960f, 960f);
			Barycentric bary = new Barycentric(points, TotalIterations);
		}

		public static void SysIterate()
		{
			SysIterations++;
		}
		public static void BaryIterate()
		{
			BaryIterations++;
		}
	}
}
