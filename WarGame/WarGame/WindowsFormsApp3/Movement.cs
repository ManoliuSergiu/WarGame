using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
namespace WindowsFormsApp3
{
	public static class Movement
	{
		public static void TargetSelect(Unit unit)
		{
			if (!unit.controlledMovement)
			{
				unit.TargetList.Clear();
				float min = 5000f;
				List<Unit> targetTeam;
				if (!unit.team)
				{
					targetTeam = Engine.BlueTeam;
				}
				else
				{
					targetTeam = Engine.RedTeam;
				}
				
				if (unit.Type==UnitType.Medic)
				{
					CheckCriticalHealth(unit);
				}
				if(!unit.Healing)
					foreach (var enemy in targetTeam)
					{
						if (enemy.Type != UnitType.Dead)
							Aux(unit, enemy, ref min);
					}
			}
			else if(!unit.controlledTarget)
			{
				float delta = 1;
				unit.target = null;
				unit.routing = false;
				if(unit.targetlocation.X>=unit.GetPoint().X-delta	&& unit.targetlocation.X <= unit.GetPoint().X + delta
					&& unit.targetlocation.Y>=unit.GetPoint().Y-delta && unit.targetlocation.Y <= unit.GetPoint().Y + delta)
				{
					unit.controlledMovement = false;
					TargetSelect(unit);
				}
				if(unit==Engine.selected)
				Form1.lab.Text = unit.controlledMovement+"";
			}
			else
			{
				if (unit.target.Type == UnitType.Dead)
				{
					unit.controlledTarget = false;
					unit.controlledMovement = false;
					TargetSelect(unit);
				}
			}
		}

		private static void CheckCriticalHealth(Unit unit)
		{
			List<Unit> targetTeam;
			if (unit.team)
				targetTeam = Engine.BlueTeam;
			else
				targetTeam = Engine.RedTeam;
			float min = 50;
			float aux;
			Unit TargetToHeal=null;
			foreach (var ally in targetTeam)
			{
				if (ally.Type != UnitType.Dead && (aux = Dist(unit, ally)) < min && (ally.HP / ally.stats.MaxHP < 0.5f)&&ally!=unit) 
				{
					min = aux;
					TargetToHeal = ally;
				}
			}
			if (min < 50)
			{
				unit.Healing = true;
				unit.target = TargetToHeal;
				unit.target.gettingHealed = true;
			}
			else
			{
				unit.Healing = false;
			}
		}

		private static void Aux(Unit unit, Unit enemy, ref float min)
		{
			float dist = Dist(unit, enemy);
			//Unit enemy2;
			if (min > dist)
			{
				min = dist;
				unit.target = enemy;
				unit.targetlocation = enemy.GetPoint();
			}
			unit.routing = false;
			if (dist < unit.stats.RANGE)
			{
				unit.TargetList.Add(enemy);
			}
			if (unit.Type != UnitType.Dragon && unit.stats.Ranged)
			{
				if (min < 30)
				{
					unit.routing = true;
				}
				else if (unit.TargetList.Count > 0)
				{
					Unit aux = SelectBestArcherTarget(unit);
					unit.target = aux;
					unit.targetlocation = aux.GetPoint();
				}
			}
		}

		public static void _Movement()
		{
			for (int i = 0; i < Engine.BlueTeam.Count; i++)
			{
				_Move(Engine.BlueTeam[i],ref i);
			}
			for (int i = 0; i < Engine.RedTeam.Count; i++)
			{
				_Move(Engine.RedTeam[i], ref i);
			}
		}

		private static void _Move(Unit a,ref int i)
		{
			
			if (a.Type != UnitType.Dead)
			{
				if (a.STM >= a.stats.STM * 2 / 3)
				{

				}
				else if (a.STM >= a.stats.STM / 3)
				{
					if (a.fatigue != 1)
					{
						a.fatigue = 1;
						a.fatigued = true;
						a.UpdateFatigue();
					}
				}
				else
				{
					if (a.fatigue != 1)
					{
						a.fatigue = 2;
						a.UpdateFatigue();
					}

				}
				MovementRoutine(a);
				if (a.GetPoint().X == -1) i--;
					
			}
		}

		private static void MovementRoutine(Unit unit)
		{
			UnitTick(unit);
			if (!unit.routing)
			{
				if (unit.target == null&& unit.target.Type == UnitType.Dead)
					TargetSelect(unit);
				float dist = Dist(unit, unit.target);
				if (!unit.Healing)
				{
					if (unit.stats.RANGE < dist || unit.controlledMovement && !unit.controlledTarget)
					{
						Move(unit);
						unit.shot = false;
					}
					else
					{
						if (unit.currentTick == 0)
						{
							Attack(unit);
						}
					}
				}
				else
				{
					float dist2 = Dist(unit, unit.target);
					if (dist2 > 5)
						Move(unit);
					else
					{
						if(unit.currentTick==0)
						{
							Heal(unit);
						}
					}
				}
			}
			else
			{
				if(Engine.tick==10)
					TargetSelect(unit);
				unit.shot = false;
				Move(unit);
			}

		}

		private static void Heal(Unit unit)
		{
			Random rand = new Random();
			int a = rand.Next(1, 21);
			if (a == 1)
			{
				
			}
			else if (a < unit.stats.CritRoll)
			{
				Form1.lab.Text = unit.target.HP+"";
				unit.target.HP += unit.target.stats.MaxHP/3;
				Form1.lab.Text = unit.target.HP + "";

			}
			else
			{
				Form1.lab.Text = unit.target.HP+"";
				unit.target.HP += unit.target.stats.MaxHP / 2.5f;
				Form1.lab.Text = unit.target.HP + "";
			}
			unit.gettingHealed = false;
			unit.target.hud.RefreshStats();
			unit.STM -= 3;
		}

		private static void UnitTick(Unit unit)
		{
			unit.currentTick++;
			if (unit.currentTick >= unit.ATKSP)
			{
				unit.currentTick = 0;
			}
		}

		private static void Attack(Unit unit)
		{
			Random rand = new Random();
			unit.shot = true;
			bool HA = (unit.height-1 > unit.target.height && unit.Type==UnitType.Archer) ? true : false;        //Height Advantage
			if (unit.stats.Ranged)
			{
				MapRenderer.ShootingQueue.Add(unit);
				if(Dist(unit,unit.target)<30)
				{
					unit.routing = true;
				}
			}
			if (unit.target == null || unit.target.Type == UnitType.Dead)
			{
				TargetSelect(unit);
				return;
			}
			if (rand.Next(100) < unit.stats.ACC * (unit.target.stats.SIZE / 6) + ((HA)? 10:0))	//HA gives 10 accuaracy points => chance of hit is increased by 10%
			{
				if(rand.Next(100)<100+unit.stats.DEX-unit.target.stats.DODGE)
				{
					float damage = unit.stats.DMG*(100-unit.target.stats.ARMOR)/100 + ((HA) ? 20 : 0); //HA gives 20 bonus pre-mitigation damage;
					if(unit.target.stats.Shield)
					{
						if (rand.Next(100) < unit.target.stats.ARMOR)
						{
							damage = 0;
						}
					}
					if (damage != 0)
					{
						int aux2;
						if ((aux2 = rand.Next(1, 21)) >= unit.stats.CritRoll)
						{
							damage *= unit.stats.CritMult;
						}
						if (aux2 == 1)
						{
							damage = 0;
						}
						int aux;
						damage = ((aux=rand.Next(1,unit.target.stats.DR))<damage)?(damage-aux):(0);
					}
					unit.target.HP -= damage;
					if (unit.target.HP <=0)
					{
						unit.killcount++;
						List<UnitHUD>[] targetHUDs;
 						if (unit.target.team)
						{
							targetHUDs = Engine.blueHUDs2;
							Engine.BlueDeaths++;
						}
						else
						{
							targetHUDs = Engine.redHUDs2;
							Engine.RedDeaths++;
						}

						targetHUDs[(int)unit.target.Type].Remove(unit.target.hud);
						unit.target.Type = UnitType.Dead;
						unit.target.hud.SetImage();
						targetHUDs[(int)unit.target.Type].Add(unit.target.hud);
						Engine.CheckWin();
					}
					unit.target.hud.RefreshStats();

					unit.miss = false;
				}
				else
				{
					if (unit.Type == UnitType.Archer)
					{
						unit.miss = true;
					}
				}
			}
			else
			{
				if (unit.Type == UnitType.Archer)
				{
					unit.miss = true;
				}
			}
			unit.hud.RefreshStats();
			unit.STM -= 3;
		}

		private static void Move(Unit unit)
		{
			float ox,oy,x, y, xa, ya;
			ox = x = unit.GetPoint().X ;
			oy = y = unit.GetPoint().Y ;
			if (unit.routing)
			{
				Rout(unit);
				xa = unit.targetlocation.X;
				ya = unit.targetlocation.Y;
			}
			else
			{
				if (unit.controlledMovement)
				{
					xa = unit.targetlocation.X;
					ya = unit.targetlocation.Y;
				}
				else
				{
					xa = unit.target.GetPoint().X;
					ya = unit.target.GetPoint().Y;
				}
			}
			double dir = Math.Atan2(ya - y,xa - x);
			x += (float)Math.Cos(dir) * unit.stats.MS;
			y += (float)Math.Sin(dir) * unit.stats.MS;

			unit.STM -= 0.007f*(unit.MS/(4/60f));
			if (unit.currentTick==15||unit.currentTick==unit.stats.ATKSP/2+15)
				unit.hud.RefreshStats();
			if (x < 0 || y < 0|| x >= MapRenderer.map.Width - 1 || y >= MapRenderer.map.Height - 1)
			{
				DisposeUnit(unit);
				if (unit.team)	Engine.BlueDeaths++;
				else			Engine.RedDeaths++;
			}

			else
			{
				if (MapRenderer.GetHeight(ox, oy) < MapRenderer.GetHeight(x, y)) unit.STM -= 1;
				unit.height = MapRenderer.GetHeight(x, y);
				unit.SetPoint(x, y);
			}
			
		}

		private static void DisposeUnit(Unit unit)
		{
			List<Unit> team;
			List<UnitHUD> HUDs;

			if(unit.team)
			{
				team = Engine.BlueTeam;
				HUDs = Engine.blueHUDs;
			}
			else
			{
				team = Engine.RedTeam;
				HUDs = Engine.redHUDs;
			}
			bool UnitPred(Unit u)
			{
				return u.GetPoint() == unit.GetPoint();
			}
			int a = team.FindIndex(UnitPred);
			team.RemoveAt(a);
			var aux = HUDs[a];
			team.RemoveAt(a);
			aux.Dispose();
			Engine.SortHUDs();
		}

		private static Unit SelectBestArcherTarget(Unit unit)
		{
			Unit TargetUS = null ;
			Unit TargetS = null ;
			float minUS=2000f;
			float minS = 2000f;
			foreach (var item in unit.TargetList)
			{
				if (item.Type != UnitType.Dead)
				{
					float dist = Dist(unit, item);
					if (!item.stats.Shield)
					{
						if (minUS > dist)
						{
							TargetUS = item;
							minUS = dist;
						}
					}
					else
					{
						if (minS > dist)
						{
							TargetS = item;
							minS = dist;
						}
					}
				}

			}
			float range = unit.stats.RANGE / 3.6f;
			if (minS < range && minUS > range||TargetUS==null)
			{
				return TargetS;
			}
			else return TargetUS;
			
		}

		private static void Rout(Unit unit)
		{
			float xa = unit.GetPoint().X;
			float xb = unit.target.GetPoint().X;
			float ya = unit.GetPoint().Y;
			float yb = unit.target.GetPoint().Y;
			unit.targetlocation = new PointF(xa + (xa - xb), ya + (ya - yb));
		}

		private static float Dist(Unit unit, Unit enemy)
		{
			float xa = unit.GetPoint().X;
			float xb = enemy.GetPoint().X;
			float ya = unit.GetPoint().Y;
			float yb = enemy.GetPoint().Y;
			return (float)Math.Sqrt((xa - xb) * (xa - xb) + (ya - yb) * (ya - yb));
		}

		public static float Dist(Unit unit, PointF point)
		{
			float xa = unit.GetPoint().X;
			float xb = point.X;
			float ya = unit.GetPoint().Y;
			float yb = point.Y;
			return (float)Math.Sqrt((xa - xb) * (xa - xb) + (ya - yb) * (ya - yb));
		}
	}
}
