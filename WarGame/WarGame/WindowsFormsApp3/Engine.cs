using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace WindowsFormsApp3
{
	public static class Engine
	{

		public static PictureBox UnitMap=Form1.um;
		public static List<Unit> RedTeam = new List<Unit>();
		public static List<Unit> BlueTeam = new List<Unit>();
		public static List<UnitHUD> redHUDs = new List<UnitHUD>();
		public static List<UnitHUD> blueHUDs= new List<UnitHUD>();
		public static List<UnitHUD>[] redHUDs2 = new List<UnitHUD>[1+(int)UnitType.Dead];
		public static List<UnitHUD>[] blueHUDs2 = new List<UnitHUD>[1+(int)UnitType.Dead];
		public static Unit selected;
		public static SelectedUnitHUD selectedUnitHUD;
		public static int BlueMax;
		public static int LastBlueDeath = 0;
		public static int LastRedDeath = 0;
		public static int BlueDeaths=0;
		public static int RedMax;
		public static int RedDeaths=0;
		public static Timer tim = new Timer();
		public static int tick = 0;

		public static void StartUp()
		{
			tim.Tick += Tim_Tick;
			tim.Interval = 17;
			for (int i = 0; i < redHUDs2.Length; i++)
			{
				redHUDs2[i] = new List<UnitHUD>();
				blueHUDs2[i] = new List<UnitHUD>();
			}
			ReMap();
		}

		public static void ReMap()
		{
			
			UnitMap.BackgroundImage = MapRenderer.DrawMap();
			MapRenderer.ugfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		}

		private static async void Tim_Tick(object sender, EventArgs e)
		{
			Task a=null;
			if (tick == 15 || tick == 45)
			{
				if (selectedUnitHUD != null) selectedUnitHUD.SetStats();
				SortHUDs();
			}
			else if (tick == 17 || tick == 47)
				a = Task.Run(() => ListHUDsAsync(true));
			else if (tick == 19 || tick == 49)
				a = Task.Run(() => ListHUDsAsync(false));

			FightTick();

			tick++;
			if (tick == 60)
				tick = 0;
			if (a != null) await a;
		}

		private static void ReselectTargets()
		{
			foreach (var item in redHUDs2[(int)UnitType.Medic])
			{
				Movement.TargetSelect(item.unit);
			}
			foreach (var item in blueHUDs2[(int)UnitType.Medic])
			{
				Movement.TargetSelect(item.unit);
			}
		}
		public static void ListHUDsAsync(bool team)
		{
			if (team)
			for (int i = 0; i < blueHUDs.Count; i++)
			{
				if(blueHUDs[i]!=null)
				blueHUDs[i].Invoke((MethodInvoker)delegate ()
				{
					blueHUDs[i].Location = new Point(15 + 90 * i, 110);
				});
			}
			else
			for (int i = 0; i <redHUDs.Count; i++)
			{
				if(redHUDs[i]!=null)
				redHUDs[i].Invoke((MethodInvoker)delegate ()
				{
					redHUDs[i].Location = new Point(15 + 90 * i, 10);
				});
			}
		}
		private static void RefreshStats(bool team)
		{
			if (team)
				foreach (var item in blueHUDs)
				{
					if (item.unit.Type != UnitType.Dead)
						item.RefreshStats();
				}
			else
				foreach (var item in redHUDs)
				{
					if (item.unit.Type != UnitType.Dead)
						item.RefreshStats();
				}
		}

		private static void FightTick()
		{
			Movement._Movement();
			ReselectTargets();
			Refresh();
		}																						
		public static void RefreshTargets(List<Unit> team)
		{
			foreach (var unit1 in team)
			{
				Movement.TargetSelect(unit1);
			}
			
		}

		public static void UnitPlacement(int x, int y, Unit unit)
		{
			
			if (x < UnitMap.Width / 2)
			{
				unit.team = false;
				RedTeam.Add(unit);
				RedMax++;
			}
			else
			{
				unit.team = true;
				BlueTeam.Add(unit);
				BlueMax++;
			}
			
			UnitMap.Image = MapRenderer.DrawUnit(unit);
			CreateUnitHUD(unit.team,unit);
			SortHUDs();
			ListHUDs();

		}
		public static int ListHUDs()
		{
			ListHUDs(false);
			ListHUDs(true);
			return 0;
		}
		public static void ListHUDs(bool team)
		{
			List<UnitHUD> teamhuds;
			if (team)
				teamhuds = blueHUDs;
			
			else
				teamhuds = redHUDs;

			for (int i = 0; i < teamhuds.Count; i++)
			{
				teamhuds[i].Location = new Point(15 + 90 * i, team?110:10);
			}
			

		}
		public static void SortHUDs(bool team)
		{
			List<UnitHUD> teamhuds;
			List<UnitHUD>[] ordHUDs;
			if (team)
			{
				teamhuds = blueHUDs;
				ordHUDs = blueHUDs2;
			}
			else
			{
				teamhuds = redHUDs;
				ordHUDs = redHUDs2;
			}
			teamhuds.Clear();
			int i = 0;
			foreach (var Category in ordHUDs)
			{
				foreach (var unithud in Category)
				{
					if (unithud.unit != null)
					{
						teamhuds.Add(unithud);
					}
					i++;
				}
			}
		}

		internal static void ChangeSelectedHud(SelectedUnitHUD aux)
		{
			if(selectedUnitHUD!=null) Form1.form.Controls.Remove(selectedUnitHUD);
			selectedUnitHUD = aux;
			Form1.form.Controls.Add(selectedUnitHUD);
			selectedUnitHUD.Show();
		}

		public static int SortHUDs()
		{
			SortHUDs(true);
			SortHUDs(false);
			return 0;
		}

		public static void Refresh()
		{
			MapRenderer.UnitMapRefresh();
			UnitMap.Image = MapRenderer.umap;

		}

		internal static void CreateUnitHUD(bool aux,Unit unit)
		{
			if(aux)
			{
				blueHUDs2[(int)unit.Type].Add(new UnitHUD(unit));
				Form1.gr.Controls.Add(blueHUDs2[(int)unit.Type][blueHUDs2[(int)unit.Type].Count-1]);
			}
			else
			{
				redHUDs2[(int)unit.Type].Add(new UnitHUD(unit));
				Form1.gr.Controls.Add(redHUDs2[(int)unit.Type][redHUDs2[(int)unit.Type].Count - 1]);
			}
		}

		internal static void CheckWin()
		{
			
			if (BlueDeaths == BlueMax)
			{
				EndGame(true);
			}
			else if (RedDeaths == RedMax)
			{
				EndGame(false);
			}
			
		}

		internal static void EndGame(bool team)
		{
			Label win = new Label
			{
				Text = (!team ? "Blue" : "Red") + " team won",
				Font = new Font("Arial", 23f, FontStyle.Bold),
				AutoSize = false,
				Size = Form1.um.Size,
				Location = new Point(0, 0),
				Parent = Form1.um,
				TextAlign = ContentAlignment.MiddleCenter,
				BackColor = Color.FromArgb(125, 0, 0, 0),
				ForeColor = !team?Color.DarkBlue:Color.DarkRed
			};
			win.BringToFront();
			tim.Stop();
		}
	}

}
