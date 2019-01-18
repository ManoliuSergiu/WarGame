using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp3
{
	public static class MapRenderer
	{
		private static PictureBox pb = Form1.pb;
		private static PictureBox um = Form1.um;
		public static Bitmap map = new Bitmap(pb.Width, pb.Height);
		public static Graphics gfx = Graphics.FromImage(map);
		private static Random random = new Random();
		public static Bitmap umap = new Bitmap(um.Width, um.Height);
		public static Graphics ugfx = Graphics.FromImage(umap);
		public static List<Unit> ShootingQueue = new List<Unit>();
		public static int[,] heightmap;
		public static Bitmap DrawMap()
		{

			gfx.Clear(Color.ForestGreen);
			map = DrawRandomMap();
			return map;
		}

		public static Bitmap DrawUnit(Unit unit)
		{
			
			if (unit.GetPoint().X < um.Width/2)
			{
				UnitDraw(unit, Color.Red, Color.Pink, Color.DarkRed);
			}
			else
			{
				UnitDraw(unit, Color.Blue,Color.LightBlue, Color.DarkBlue);
			}
			return umap;
		}

		public static void DrawUnits()
		{
			
			foreach (var unit in Engine.RedTeam)
				UnitDraw(unit, Color.Red, Color.LightPink, Color.DarkRed);
			foreach (var unit in Engine.BlueTeam)
				UnitDraw(unit, Color.Blue, Color.LightBlue, Color.DarkBlue);


			Unit aux = Engine.selected;											
			if (aux != null)
			{
				PointF[] points = new PointF[3];
				points[0] = new PointF(aux.GetPoint().X, aux.GetPoint().Y - aux.stats.SIZE * 1.1f);
				points[1] = new PointF(aux.GetPoint().X - aux.stats.SIZE * 0.7f, aux.GetPoint().Y - aux.stats.SIZE * 2.5f);
				points[2] = new PointF(aux.GetPoint().X + aux.stats.SIZE * 0.7f, aux.GetPoint().Y - aux.stats.SIZE * 2.5f);
				ugfx.FillPolygon(Brushes.Gold, points);
				ugfx.DrawPolygon(Pens.Black, points);
			}
			
		}

		private static bool UnitDraw(Unit unit,Color a, Color b, Color c)
		{
			SolidBrush auxb = new SolidBrush(a);
			SolidBrush auxb2 = new SolidBrush(b);
			SolidBrush auxb3 = new SolidBrush(c);
			if (unit.Type != UnitType.Dead)
			{
				if (!unit.fatigued)
					ugfx.FillEllipse(auxb, unit.GetPoint().X - unit.stats.SIZE / 2, unit.GetPoint().Y - unit.stats.SIZE / 2, unit.stats.SIZE, unit.stats.SIZE);
				else
					ugfx.FillEllipse(auxb2, unit.GetPoint().X - unit.stats.SIZE / 2, unit.GetPoint().Y - unit.stats.SIZE / 2, unit.stats.SIZE, unit.stats.SIZE);
				if (unit.HP > 0.5 * unit.stats.MaxHP)
					ugfx.DrawEllipse(new Pen(a,1), unit.GetPoint().X - unit.stats.SIZE / 2, unit.GetPoint().Y - unit.stats.SIZE / 2, unit.stats.SIZE, unit.stats.SIZE);
				else
					ugfx.DrawEllipse(new Pen(c,1), unit.GetPoint().X - unit.stats.SIZE / 2, unit.GetPoint().Y - unit.stats.SIZE / 2, unit.stats.SIZE, unit.stats.SIZE);
				
			}
			else
			{
				if (!unit.corpse)
				{
					gfx = Graphics.FromImage(map);
					gfx.DrawString("x", new Font("Arial", unit.stats.SIZE * 2, FontStyle.Bold), auxb3, unit.GetPoint().X - unit.stats.SIZE * 2, unit.GetPoint().Y - unit.stats.SIZE * 2);
					Engine.Map.Image = map;
					unit.corpse = true;
				}
			}
			return true;
		}

		public static int GetHeight(float X,float Y)
		{
			return heightmap[(int)(X / 4), (int)(Y / 4)];
		}
		public static void UnitMapRefresh()
		{
			DrawShots2ElectricBoogaloo();
			DrawUnits();
			
		}
		private static void DrawShots2ElectricBoogaloo()
		{
			ugfx.Clear(Color.Transparent);
			int DrawFrames = 1;
			Random rand = new Random();
			for (int i = 0; i < ShootingQueue.Count; i++)
			{
				Unit unit = ShootingQueue[i];
				if (unit.currentTick < DrawFrames&&unit.Type!=UnitType.Dead)
				{
					float a = unit.target.GetPoint().X;
					float b = unit.target.GetPoint().Y;
					if (unit.miss)
					{
						a += rand.Next(-15, +16);
						b += rand.Next(-15, +16);
					}
					ugfx.DrawLine(new Pen((unit.team?Color.LightBlue:Color.LightPink), 2), unit.GetPoint().X, unit.GetPoint().Y, a, b);
				}
				else
				{
					ShootingQueue.Remove(unit);
					i--;
				}
			}
		}
		private static Bitmap DrawRandomMap()
		{
			Bitmap a = new Bitmap(DiamondSquareGen.width, DiamondSquareGen.height/2);
			Bitmap b = new Bitmap(pb.Width,pb.Height);
			Graphics gpx = Graphics.FromImage(b);
			gpx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			heightmap = DiamondSquareGen.Start();
			for (int i = 0; i < a.Width; i++)
			{
				for (int j = 0; j < a.Height; j++)
				{
					Color color;
					if      (heightmap[i, j] < 0)   color = Color.FromArgb(255, 0, 0, 153);
					else if (heightmap[i, j] < 2)   color = Color.FromArgb(255, 51, 204, 51);
					else if (heightmap[i, j] < 4)   color = Color.FromArgb(255, 51, 188, 51);
					else if (heightmap[i, j] < 6)   color = Color.FromArgb(255, 51, 179, 41);
					else if (heightmap[i, j] < 8)   color = Color.FromArgb(255, 51, 170, 41);
					else if (heightmap[i, j] < 10)  color = Color.FromArgb(255, 51, 165, 41);
					else if (heightmap[i, j] < 12)  color = Color.FromArgb(255, 51, 161, 24);
					else if (heightmap[i, j] < 14)  color = Color.FromArgb(255, 51, 147, 24);
					else if (heightmap[i, j] < 16)  color = Color.FromArgb(255, 51, 141, 24);
					else if (heightmap[i, j] < 18)  color = Color.FromArgb(255, 51, 133, 24);
					else if (heightmap[i, j] < 20)  color = Color.FromArgb(255, 51, 129, 13);
					else if (heightmap[i, j] < 22)  color = Color.FromArgb(255, 64, 129, 13);
					else if (heightmap[i, j] < 24)  color = Color.FromArgb(255, 72, 129, 13);
					else if (heightmap[i, j] < 26)  color = Color.FromArgb(255, 83, 129, 13);
					else if (heightmap[i, j] < 28)  color = Color.FromArgb(255, 83, 122, 13);
					else if (heightmap[i, j] < 30)  color = Color.FromArgb(255, 90, 117, 13);
					else if (heightmap[i, j] < 32)  color = Color.FromArgb(255, 90, 110, 13);
					else if (heightmap[i, j] < 34)  color = Color.FromArgb(255, 101, 110, 13);
					else if (heightmap[i, j] < 36)  color = Color.FromArgb(255, 110, 110, 0);
					else if (heightmap[i, j] < 38)  color = Color.FromArgb(255, 110, 101, 0);
					else if (heightmap[i, j] < 40)  color = Color.FromArgb(255, 117, 101, 0);
					else if (heightmap[i, j] < 42 ) color = Color.FromArgb(255, 113, 88, 0);
					else if (heightmap[i, j] < 44 ) color = Color.FromArgb(255, 105, 81, 0);
					else if (heightmap[i, j] < 46 ) color = Color.FromArgb(255, 105, 68, 0);
				    else if (heightmap[i, j] < 48 ) color = Color.FromArgb(255, 102, 51, 0);
					else color = Color.FromArgb(255, 0, 0, 0);
					//color = Color.FromArgb(255, 80-heightmap[i,j], 120 + heightmap[i, j], 0);
					a.SetPixel(i,j, color);
				}
			}

			Rectangle srcRect = new Rectangle(0, 0, a.Width-1, a.Height-1);
			Rectangle dstRect = new Rectangle(0, 0, b.Width, b.Height);
			gpx.DrawImage(a, dstRect, srcRect, GraphicsUnit.Pixel);
			//gpx.DrawLine(new Pen(Brushes.Black, 3), 50, 50, 50+180, 50);
			return b;
		}
	}
}
