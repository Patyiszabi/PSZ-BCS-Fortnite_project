using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSZ_BCS_Fortnite_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string weapon_file = "fegyverek.txt";
        public static string item_file = "itemek.txt";
        public static string player_file = "jatekosok.txt";

        public class jatekos
        {
            public string username;
            public int level;
            public int life;
            public int shield;
            public int kills;
            public int xp;
            public int weapons;
            public int items;
            public int avatar;

            public jatekos(string username, int level, int life, int shield, int kills, int xp, int weapons, int items, int avatar)
            {
                this.username = username;
                this.level = level;
                this.life = life;
                this.shield = shield;
                this.kills = kills;
                this.xp = xp;
                this.weapons = weapons;
                this.items = items;
                this.avatar = avatar;
            }
        }
        public class weapons_class
        {
            public string weapon_name;
            public int level;
            public int mag_size;
            public weapons_class(string weapon_name, int level, int mag_size)
            {
                this.weapon_name = weapon_name;
                this.level = level;
                this.mag_size = mag_size;
            }
        }
        public class items_class
        {
            public string item_name;
            public int quantity;
            public items_class(string item_name, int quantity)
            {
                this.item_name = item_name;
                this.quantity = quantity;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Köszönjük, hogy játékunkat használta! A ranglista elérhető");
            Application.Exit();
           
        }
    }
}
