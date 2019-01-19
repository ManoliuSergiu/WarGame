using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{

	public partial class Form1 : Form
	{
		public static PictureBox um;
		public static UnitType selected=UnitType.Archer;
		public static Button[] unitTypeButtons = new Button[(int)UnitType.Dead];
		public static Form1 form;
		public static Panel gr;
		public static Point scroll;
		int k = 0;
		int i = 0;
		bool extend = false;
		public float ClickRadius = 20;

		public Form1()
		{
			InitializeComponent();
			form = this;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			um = pictureBox2;
			gr = panel1;
			Engine.UnitMap = pictureBox2;
			Engine.StartUp();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Engine.ReMap();
		}

		private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (k == 1)
				{
					panel1.AutoScrollPosition = new Point(0, panel1.AutoScrollPosition.Y);
					Unit unit = new Unit(e.X, e.Y, selected);
					i++;
					Engine.UnitPlacement(e.X, e.Y, unit);
					MapRenderer.ugfx.DrawString(Convert.ToString(unit.height), new Font("Times New Roman", 10f), Brushes.Red, 20 * i, 20);
				}
				if (k == 2)
				{
					Unit aux = null;
					float minDist = ClickRadius;
					float a;
					
					foreach (var unit in Engine.BlueTeam)
					{
						if(unit.Type!=UnitType.Dead)
							if (minDist > (a = Movement.Dist(unit, e.Location)))
							{
								minDist = a;
								aux = unit;
							}
					}
					foreach (var unit in Engine.RedTeam)
					{
						if (unit.Type != UnitType.Dead)

							if (minDist > (a = Movement.Dist(unit, e.Location)))
							{
								minDist = a;
								aux = unit;
							}
					}
					if (minDist != ClickRadius)
					{
						if (Engine.selected != null)
							Engine.selected.hud.BackColor = (Engine.selected.team) ? Color.Blue : Color.Red;
						Engine.selected = aux;
						Engine.selected.hud.BackColor = Color.Gold;
					}
				}
			}
			else if(e.Button==MouseButtons.Right)
			{
				if (Engine.selected != null)
				{
					Engine.selected.controlledMovement = true;

					Unit aux = null;
					float minDist = ClickRadius;
					float a;
					if (!Engine.selected.team)
					{
						foreach (var unit in Engine.BlueTeam)
							if (unit.Type != UnitType.Dead)
								if (minDist > (a = Movement.Dist(unit, e.Location)))
								{
									minDist = a;
									aux = unit;
								}
					}
					else
					{
						foreach (var unit in Engine.RedTeam)
							if (unit.Type != UnitType.Dead)
								if (minDist > (a = Movement.Dist(unit, e.Location)))
								{
									minDist = a;
									aux = unit;
								}
					}
					if (minDist != ClickRadius)
					{
						Engine.selected.controlledTarget = true;
						Engine.selected.target = aux;
					}
					else
					{
						Engine.selected.controlledTarget = false;
						Engine.selected.targetlocation = e.Location;
					}
				}
			}
		}	

		private void button2_Click(object sender, EventArgs e)
		{
			
			if (k == 0)
			{
				CreateButtons();
				MapRenderer.ugfx.DrawLine(new Pen(Color.Black, 5),new Point(um.Width/2,0),new Point(um.Width/2,um.Height));
				um.Image = MapRenderer.umap;
				button2.Text = "Start simulation";
			}
			else
			{					   
				button1.Visible =  false;
				button2.Visible =  false;
				button17.Visible = false;
				flowLayoutPanel1.Dispose();
				Engine.RefreshTargets(Engine.RedTeam);
				Engine.RefreshTargets(Engine.BlueTeam);
				Engine.tim.Start();
			}
			k++;
		}

		private void CreateButtons()
		{

			for (int i = 0; i < unitTypeButtons.Length; i++)
			{
				unitTypeButtons[i] = new Button
				{
					Parent = flowLayoutPanel1,
					Text = Convert.ToString((UnitType)i) ,
					Size = new Size(flowLayoutPanel1.Width / 2 - 6, flowLayoutPanel1.Height / ((unitTypeButtons.Length+1) / 2 )- 6),
					Tag = i
					
				};
				unitTypeButtons[i].Click += unit_Click;
			}
		}

		private void unit_Click(object sender, EventArgs e)
		{
			Button aux = (Button)sender;
			selected = (UnitType)aux.Tag;
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			switch (trackBar1.Value)
			{
				case 0:
					Engine.tim.Interval = 50;
					break;
				case 1:
					Engine.tim.Interval = 30;
					break;
				case 2:
					Engine.tim.Interval = 17;
					break;
				case 3:
					Engine.tim.Interval = 9;
					break;
				case 4:
					Engine.tim.Interval = 2;
					break;
				default:
					break;
			}
		}

		private void button14_Click(object sender, EventArgs e)
		{
			if (!extend)
			{
				Size = new Size(Width, Height + 220);
				
			}
			else
			{ 
				Size = new Size(Width, Height - 220);
			}
			extend = !extend;

		}

		private void button15_Click(object sender, EventArgs e)
		{
			int a = 15 + 90 * ((Engine.BlueTeam.Count > Engine.RedTeam.Count) ? Engine.BlueTeam.Count : Engine.RedTeam.Count);
			if (Math.Abs(panel1.AutoScrollPosition.X) + panel1.Size.Width-15<a)
			{
				panel1.AutoScrollPosition = new Point(Math.Abs(panel1.AutoScrollPosition.X) + panel1.Size.Width-15, panel1.AutoScrollPosition.Y);
				scroll = panel1.AutoScrollPosition;
			}
		}

		private void button16_Click(object sender, EventArgs e)
		{
			if (Math.Abs(panel1.AutoScrollPosition.X) - panel1.Size.Width + 15>= 0)
			{
				panel1.AutoScrollPosition = new Point(Math.Abs(panel1.AutoScrollPosition.X) - panel1.Size.Width+15, panel1.AutoScrollPosition.Y);
				scroll = panel1.AutoScrollPosition;
			}
			else
			{
				panel1.AutoScrollPosition = new Point(0, panel1.AutoScrollPosition.Y);
				scroll = panel1.AutoScrollPosition;
			}

		}

		private void button17_Click(object sender, EventArgs e)
		{
			Random rand = new Random();
			for (int i = 0; i < 150; i++)
			{
				int a, b;
				Unit unit = new Unit(a=rand.Next(um.Size.Width /2-50, um.Size.Width / 2 + 51), b=rand.Next(1,um.Size.Height-2), (UnitType)rand.Next(11));
				Engine.UnitPlacement(a, b, unit);
			}
		}

		
	}
}
