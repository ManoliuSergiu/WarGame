using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace WindowsFormsApp3
{
	public static class EnumExtention
	{
		public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);
			return type.GetField(name)
				.GetCustomAttributes(false)
				.OfType<TAttribute>()
				.SingleOrDefault();
		}
	}
	public class UnitAttr : Attribute
	{
		public UnitAttr(bool defa,bool shield,bool ranged,float range,float acc,float dmg,float atksp,float dodge,
			float dex,float size, float armor,float hp, float ms, float stm,int Dr,int cr,float cm, string name)
		{
			Def = defa;			//Default
			Shield = shield;	//Shield
			Ranged = ranged;	//Ranged
			RANGE = range;		//Range
			ACC = acc;			//Accuracy
			DMG = dmg;			//Damage
			ATKSP = atksp;		//Attack Speed
			DODGE = dodge;		//Dodge chance
			DEX = dex;			//Dexterity
			SIZE = size;		//Size
			ARMOR = armor;		//Armor
			MaxHP = hp;			//Maximum hp
			MS = ms/60;			//Movement Speed
			STM = stm;			//Stamina
			DR = Dr;			//Defence Roll
			CritRoll = cr;		//Critical Roll
			CritMult = cm;		//Critical Multiplier
			Name = name;		//Unit name
		}
		public bool Def	   { get; private set; }
		public bool Shield { get; private set; }
		public bool Ranged { get; private set; }
		public float RANGE { get; private set; }
		public float ACC   { get; private set; }
		public float DMG   { get; private set; }
		public float ATKSP { get; private set; }
		public float DODGE { get; private set; }
		public float STM   { get; private set; }
		public float DEX   { get; private set; }
		public float SIZE  { get; private set; }
		public float ARMOR { get; private set; }
		public float MaxHP { get; private set; }
		public float MS    { get; private set; }
		public int DR    { get; private set; }
		public int CritRoll { get; private set; }
		public float CritMult { get; private set; }
		public string Name { get; private set; }

	}
	public enum UnitType
	{
																							
		[UnitAttr(true,false,false,3,75,250,75,25,50,   5,85,750,4f,200,   35,18,3,"Lord")]				Lord,			
		[UnitAttr(true,false,false,3,70,200,70,30,35,   5,65,600,4.5f,200, 25,19,3,"Hero")]				Hero,								
		[UnitAttr(true,false,false,3,75,125,75,25,35,   4,45,175,4.5f,100, 25,20,3,"Swordsman")]		Swordsman,				
		[UnitAttr(true,false,false,4,65,115,100,25,30,  4,45,175,4.5f,100,25,19,2.5f,"Spearman")]		Spearman,					
		[UnitAttr(true,true ,false,3,65,125,85,15,30,   5,55,175,4f,100,   35,20,3,"SwordsmanS")]		SwordsmanS,					
		[UnitAttr(true,true ,false,4,60,115,115,20,25,  5,55,175,4f,100,  35,19,2.5f,"SpearmanS")]		SpearmanS,			
		[UnitAttr(false,false,true,180,65,75,180,40,15, 4,25,120,5f,125,20,18,2,"Archer")]				Archer,	
		[UnitAttr(true,false,true,60,45,100,95,35,15,    5,35,110,5.2f,150,25,17,1.5f,"Medic")]			Medic,
		[UnitAttr(true,false,false,6,70,150,85,200,35,  7,75,225,8.5f,200,   35,17,2,"Cavalry")]		Cav,				
		[UnitAttr(true,false,true,95,55,75,150,25,35,   7,50,200,9.2f,200,  25,19,2,"R Cavalry")]		RCav,					
		[UnitAttr(true,false,false,15,65,300,300,5,25,  12,75,600,6f,300, 55,20,3,"Giant")]				Giant,			
		[UnitAttr(true,false,false,25,55,250,360,35,15, 10,55,500,8f,300,45,20,3,"Dragon")]				Dragon,
		[UnitAttr(false, false, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0f, 0,0 ,0, 0 ,"Dead")]				Dead
	}			

	public class Unit																			
	{
		public string Name;
		public UnitHUD hud;
		public UnitAttr stats;
		public UnitType Type;
		public int killcount;
		public float HP;
		public float DMG;
		public float STM;
		public float MS;
		public float ATKSP;
		public int height;
		public bool team;
		public Unit target;
		public PointF targetlocation;
		public List<Unit> TargetList = new List<Unit>();
		public bool routing = false;
		private float X, Y;
		public int currentTick = 0;
		public bool shot;
		public bool miss;
		public int fatigue = 0;
		public bool fatigued=false;
		public bool corpse = false;
		public bool controlledMovement;
		public bool controlledTarget;
		public bool gettingHealed;
		public bool Healing;

		public Unit(int x, int y, UnitType type)
		{
			X = x;
			Y = y;
			stats = type.GetAttribute<UnitAttr>();
			HP = stats.MaxHP;
			STM = stats.STM;
			MS = stats.MS;
			ATKSP = stats.ATKSP;
			height = GetHeight();
			Type = type;
			Random rand = new Random();
			currentTick += rand.Next(1,20);

		}
		
		

	
		public int GetHeight()
		{
			if (!CheckOutOfBounds())
				return MapRenderer.heightmap[(int)(X / 4), (int)(Y / 4)];
			else
			{
				if (team)
					Engine.BlueTeam.Remove(this);
				else
					Engine.RedTeam.Remove(this);
			}
			return -1;

		}

		private bool CheckOutOfBounds()
		{
			if (X < 0 || Y < 0 || X >= MapRenderer.map.Width - 1 || Y >= MapRenderer.map.Height - 1) 
				return true;
			
			return false;
		}

		public PointF GetPoint()
		{
			if (!CheckOutOfBounds())
				return new PointF(X, Y);
			else
			{
				if (team)
				{
					Engine.BlueTeam.Remove(this);
					Engine.BlueDeaths++;
					Engine.CheckWin();
				}
				else
				{
					Engine.RedTeam.Remove(this);
					Engine.RedDeaths++;
					Engine.CheckWin();
				}
			}
			return new PointF(-1,-1);
		}
		public void SetPoint(float x,float y)
		{
			X = x;
			Y = y;
		}

		internal void UpdateFatigue()
		{
			MS -= (fatigue * stats.MS / 4);
			ATKSP += (fatigue * stats.ATKSP / 4);
			DMG -= (fatigue * stats.DMG / 8);
		}
	}
}
