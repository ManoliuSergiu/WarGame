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
	public partial class SelectedUnitHUD : UserControl
	{
		Unit unit;
		public SelectedUnitHUD(UnitHUD unitHUD)
		{
			InitializeComponent();
			unit = unitHUD.unit;
			GetDetails();
		}
		public SelectedUnitHUD(Unit unit)
		{
			InitializeComponent();
			this.unit = unit;
			GetDetails();
		}

		private void GetDetails()
		{
			Location = new Point(1272-220, 757-220);
			nameLbl.Text = unit.Name;
			BackColor = unit.team ? Color.Blue : Color.Red;
			listBox1.Items.Add("Kills:\t\t"+unit.killcount);
			listBox1.Items.Add("Health:\t\t" + unit.HP + "/" + unit.stats.MaxHP);
			listBox1.Items.Add("Stamina:\t\t"+unit.STM+"/"+unit.stats.STM);
			listBox1.Items.Add("Damage:\t\t"+unit.stats.DMG);
			listBox1.Items.Add("Dexterity\t\t"+unit.stats.DEX);
			listBox1.Items.Add("Armor:\t\t"+unit.stats.ARMOR);
			listBox1.Items.Add("Dodge:\t\t"+unit.stats.DODGE);
		}

		public void SetStats()
		{
			listBox1.Items[0] =	"Kills:\t\t"+unit.killcount;
			listBox1.Items[1] = "Health:\t\t" + unit.HP + "/" + unit.stats.MaxHP;
			listBox1.Items[2] = "Stamina:\t\t" +Convert.ToInt32(unit.STM) + "/" + unit.stats.STM;
			listBox1.Items[3] =	"Damage:\t\t"+unit.stats.DMG;
			listBox1.Items[4] = "Dexterity\t\t" + unit.stats.DEX;
			listBox1.Items[5] = "Armor:\t\t" + unit.stats.ARMOR;
			listBox1.Items[6] =	"Dodge:\t\t"+unit.stats.DODGE;



		}

		private void nameLbl_Click(object sender, EventArgs e)
		{

		}
	}
}
