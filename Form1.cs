using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        public static string name_file = "nevek.txt";

        public static List<jatekos> jatekosok = new List<jatekos>();
        public static List<string> nevek = new List<string>();
        public static List<weapons_class> fegyverek = new List<weapons_class>();
        static jatekos player1;
        static jatekos player2;
        static jatekos player3;
        static jatekos player4;
        static jatekos player5;
        static jatekos player6;
        static jatekos player7;
        static jatekos player8;



        public class jatekos
        {
            public string username;
            public int level;
            public int life;
            public int shield;
            public int kills;
            public int xp;
            // changed to hold actual weapon objects assigned to the player
            public List<weapons_class> weapons;
            public int items;
            public int avatar;

            public jatekos(string username, int level, int life, int shield, int kills, int xp, List<weapons_class> weapons, int items, int avatar)
            {
                this.username = username;
                this.level = level;
                this.life = life;
                this.shield = shield;
                this.kills = kills;
                this.xp = xp;
                this.weapons = weapons ?? new List<weapons_class>();
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

        private void Form1_Load(object sender, EventArgs e)
        {//nevek beolvasasa
            if (File.Exists(name_file))
            {
                using (var name = new StreamReader(name_file))
                {
                    while (!name.EndOfStream)
                    {
                        string line = name.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var p in parts)
                        {
                            nevek.Add(p.Trim());
                        }
                    }
                }
            }

            //fegyverek beolvasasa
            if (File.Exists(weapon_file))
            {
                using (var weapon = new StreamReader(weapon_file))
                {
                    while (!weapon.EndOfStream)
                    {
                        string line = weapon.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 3)
                        {
                            string wname = parts[0].Trim();
                            int wlevel;
                            int wmag;
                            int.TryParse(parts[1].Trim(), out wlevel);
                            int.TryParse(parts[2].Trim(), out wmag);
                            fegyverek.Add(new weapons_class(wname, wlevel, wmag));
                        }
                        else
                        {
                            // if only name provided, use default numeric values
                            string wname = parts[0].Trim();
                            fegyverek.Add(new weapons_class(wname, 1, 0));
                        }
                    }
                }
            }

            // Create a single Random instance
            var rnd = new Random();

            // Build player1 with random stats and 1..5 weapons chosen from fegyverek
            string username = nevek.Count > 0 ? nevek[rnd.Next(nevek.Count)] : "Player";
            int level = rnd.Next(1, 201);
            int life = rnd.Next(0, 101);
            int shield = rnd.Next(0, 101);
            int kills = rnd.Next(0, 100);
            int xp = kills * 150;
            int items = rnd.Next(1, 11);
            int avatar = rnd.Next(1, 21);

            // decide how many weapons to assign (1 to 5)
            int weaponCount = rnd.Next(1, 6);

            // select up to weaponCount distinct weapons from fegyverek
            var assignedWeapons = new List<weapons_class>();
            if (fegyverek.Count > 0)
            {
                // if fewer weapons available than requested, take all
                weaponCount = Math.Min(weaponCount, fegyverek.Count);

                // choose distinct random indices
                var indices = new HashSet<int>();
                while (indices.Count < weaponCount)
                {
                    indices.Add(rnd.Next(fegyverek.Count));
                }
                foreach (var idx in indices)
                {
                    assignedWeapons.Add(fegyverek[idx]);
                }
            }

            // instantiate player1
            player1 = new jatekos(username, level, life, shield, kills, xp, assignedWeapons, items, avatar);

            // (Optional) add player1 to the general list
            jatekosok.Add(player1);

            // show player1 weapon names in label1 (comma separated)
            if (player1.weapons != null && player1.weapons.Count > 0)
            {
                label1.Text = string.Join(", ", player1.weapons.Select(w => w.weapon_name));
            }
            else
            {
                label1.Text = "Nincs fegyver";
            }

            // Map weapon names to image file names
            var weaponImageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Hand Cannon", "Deagle.jpg" },
                { "Assault Rifle", "scar.jpg" },
                { "Pump Shotgun", "pump.jpg" },
                { "Bolt-Action Sniper Rifle", "bolti.jpg" },
                { "Darth Vader's Lightsaber", "vader.jpg" },
                { "Ranger Assault Rifle", "assa.jpg" },
                { "Drum Gun", "drum.jpg" },
                { "Tactical Shotgun", "tact.jpg" },
                { "Tactical Submachine Gun", "smg.jpg" },
                { "Heisted Run 'N' Gun", "p90.jpg" },
                { "Shadow Tracker", "usp.jpg" },
                { "Hunting Rifle", "kacsavadasz.jpg" }
            };

            // prepare picturebox array in the display order
            var weaponBoxes = new PictureBox[] { ptb_weapon1, ptb_weapon2, ptb_weapon3, ptb_weapon4, ptb_weapon5 };

            // Clear existing images safely and assign weapons to pictureboxes one-to-one.
            for (int i = 0; i < weaponBoxes.Length; i++)
            {
                var pb = weaponBoxes[i];

                // dispose previous image if any
                if (pb.Image != null)
                {
                    pb.Image.Dispose();
                    pb.Image = null;
                }

                // if there is a corresponding weapon, set its image
                if (i < player1.weapons.Count)
                {
                    var weapon = player1.weapons[i];
                    string imgFile;
                    if (weaponImageMap.TryGetValue(weapon.weapon_name, out imgFile))
                    {
                        if (File.Exists(imgFile))
                        {
                            // load image from file
                            try
                            {
                                pb.Image = Image.FromFile(imgFile);
                                pb.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            catch
                            {
                                // If loading fails, leave picturebox empty
                                pb.Image = null;
                            }
                        }
                        else
                        {
                            // image file not found - leave empty (or set default)
                            pb.Image = null;
                        }
                    }
                    else
                    {
                        // no mapping for weapon name - leave empty (or set default)
                        pb.Image = null;
                    }
                }
                else
                {
                    // no weapon for this slot -> leave empty
                    pb.Image = null;
                }
            }
        }
    }
}
