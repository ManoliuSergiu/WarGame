using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
	public partial class UnitHUD : UserControl
	{
		Bitmap bmp;
		Bitmap bmp2;
		public Unit unit;
		Graphics graphics;
		public int id;
		public UnitHUD(Unit a)
		{
			Location = new Point(-100, -100);
			InitializeComponent();
			this.unit = a;
			a.hud=this;
			id = (a.team ? Engine.BlueMax : Engine.RedMax);
			Parent = Form1.gr;
			bmp  = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			bmp2 = new Bitmap(pictureBox2.Width,pictureBox2.Height);
			graphics = Graphics.FromImage(bmp2);
			BackColor =	a.team?Color.Blue:Color.Red;
			RefreshStats();
			SetImage();
			label1.Text =unit.stats.Name + " " + id;
			unit.Name = unit.stats.Name + " " + id;
			label1.Parent = pictureBox1;
			label1.BackColor = Color.Transparent;
			//BringToFront();
		}

		public void SetImage()
		{
			#region placeholder
			Form1.gr.AutoScrollPosition = new Point(0, 0);
			RefreshStats();
			switch (unit.Type)
			{
				case UnitType.Lord:
					ChangeColor(Color.Tomato);
					break;
				case UnitType.Hero:
					ChangeColor(Color.Turquoise);
					break;
				case UnitType.Archer:
					ChangeColor(Color.AliceBlue);
					break;
				case UnitType.Medic:
					ChangeColor(Color.White,unit.team?Color.Blue:Color.Red);
					break;
				case UnitType.Swordsman:
					ChangeColor(Color.White);
					break;
				case UnitType.Spearman:
					ChangeColor(Color.Green);
					break;
				case UnitType.SwordsmanS:
					ChangeColor(Color.Green);
					break;
				case UnitType.SpearmanS:
					ChangeColor(Color.DarkGreen);
					break;
				case UnitType.Cav:
					ChangeColor(Color.LightPink);
					break;
				case UnitType.RCav:
					ChangeColor(Color.HotPink);
					break;
				case UnitType.Giant:
					ChangeColor(Color.Blue);
					break;
				case UnitType.Dragon:
					ChangeColor(Color.BlueViolet);
					break;
				case UnitType.Dead:
					label1.ForeColor = Color.White;
					ChangeColor(Color.Black);
					break;
				default:
					break;
			}

			#endregion
		}

		private void UserControl1_Load(object sender, EventArgs e)
		{

		}
		public void ChangeColor(Color c)
		{
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					bmp.SetPixel(i, j, c);

				}
			}
			pictureBox1.Image = bmp;
		}
		public void ChangeColor(Color c,Color d)
		{
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					bmp.SetPixel(i,j, c);
					if (j >= bmp.Height / 2 - 5 && j <= bmp.Height / 2 + 5 || i >= bmp.Width / 2 - 5 && i <= bmp.Width / 2 + 5)
						bmp.SetPixel(i, j, d);

				}
			}
			pictureBox1.Image = bmp;
		}
		public void RefreshStats()
		{
			int healthP = (int)(63 * (unit.HP / unit.stats.MaxHP));
			int staminaP = (int)(63 * Math.Max(0,(unit.STM / unit.stats.STM)));
			graphics.DrawLine(new Pen(Color.Red,4), new Point(healthP+2,4), new Point(65, 4));
			graphics.DrawLine(new Pen(Color.Green,4), new Point(2,4), new Point(healthP+2, 4));
			graphics.DrawLine(new Pen(Color.Goldenrod, 3), new Point(2, 9), new Point(staminaP + 2, 9));
			graphics.DrawLine(new Pen(Color.WhiteSmoke, 3), new Point(staminaP + 2, 9), new Point(65, 9));
			for (int i = 0; i < bmp2.Width; i++)
			{
				for (int j = 0; j < bmp2.Height; j++)
				{
					if (i < 2 || i > bmp2.Width - 3 || j < 2 || j > bmp2.Height - 3|| j >= 6 && j < 8)
						bmp2.SetPixel(i, j, Color.Black);
					//if(i>=6&&i<8)
				}
			}
			pictureBox2.Image = bmp2;
		}

		private void UnitHUD_MouseClick(object sender, MouseEventArgs e)
		{
			if(Engine.selected!=null)
				Engine.selected.hud.BackColor = (Engine.selected.team) ? Color.Blue : Color.Red;
			Engine.selected = unit;
			BackColor = Color.Gold;
			Engine.ChangeSelectedHud(new SelectedUnitHUD(unit));
		}
	}
}
