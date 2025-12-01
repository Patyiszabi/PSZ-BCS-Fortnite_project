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
        public static string skin_directory = "\\skinek";

        public static List<jatekos> jatekosok = new List<jatekos>();
        public static List<string> nevek = new List<string>();
        public static List<weapons_class> fegyverek = new List<weapons_class>();
        public static List<Image> skinek = new List<Image>();
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
                          
                            string wname = parts[0].Trim();
                            fegyverek.Add(new weapons_class(wname, 1, 0));
                        }
                    }
                }
            }
            //skinek beolvasasa
            string skinDir = Path.Combine(Application.StartupPath, "skinek");
            string[] skinFiles = new string[0];
            if (Directory.Exists(skinDir))
            {
                var exts = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
                skinFiles = Directory.GetFiles(skinDir)
                                     .Where(f => exts.Contains(Path.GetExtension(f).ToLowerInvariant()))
                                     .ToArray();
            }

            
            var rnd = new Random();

         
            string username = nevek.Count > 0 ? nevek[rnd.Next(nevek.Count)] : "Player";
            int level = rnd.Next(1, 201);
            int life = rnd.Next(0, 101);
            int shield = rnd.Next(0, 101);
            int kills = rnd.Next(0, 100);
            int xp = kills * 80000;
            int items = rnd.Next(1, 11);
            int avatar = -1;

            int weaponCount = rnd.Next(1, 6);

            var assignedWeapons = new List<weapons_class>();
            if (fegyverek.Count > 0)
            {
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

            player1 = new jatekos(username, level, life, shield, kills, xp, assignedWeapons, items, avatar);

            if (skinFiles.Length > 0)
            {
                int skinIndex = rnd.Next(skinFiles.Length);
                string chosenSkinFile = skinFiles[skinIndex];

                if (ptb_avatar.Image != null)
                {
                    ptb_avatar.Image.Dispose();
                    ptb_avatar.Image = null;
                }

                try
                {
                    ptb_avatar.Image = Image.FromFile(chosenSkinFile);
                    ptb_avatar.SizeMode = PictureBoxSizeMode.Zoom;
                    player1.avatar = skinIndex; 
                }
                catch
                {

                    player1.avatar = -1;
                }
            }
            else
            {
                player1.avatar = -1;
            }

            jatekosok.Add(player1);

            if (player1.weapons != null && player1.weapons.Count > 0)
            {
                label1.Text = string.Join(", ", player1.weapons.Select(w => w.weapon_name));
            }
            else
            {
                label1.Text = "Nincs fegyver";
            }

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
                { "Heisted Run 'N' Gun SMG", "p90.jpg" },
                { "Shadow Tracker", "usp.jpg" },
                { "Hunting Rifle", "kacsavadasz.jpg" }
            };

            var weaponBoxes = new PictureBox[] { ptb_weapon1, ptb_weapon2, ptb_weapon3, ptb_weapon4, ptb_weapon5 };

            for (int i = 0; i < weaponBoxes.Length; i++)
            {
                var pb = weaponBoxes[i];


                if (pb.Image != null)
                {
                    pb.Image.Dispose();
                    pb.Image = null;
                }

                if (i < player1.weapons.Count)
                {
                    var weapon = player1.weapons[i];
                    string imgFile;
                    if (weaponImageMap.TryGetValue(weapon.weapon_name, out imgFile))
                    {
                        if (File.Exists(imgFile))
                        {

                            try
                            {
                                pb.Image = Image.FromFile(imgFile);
                                pb.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            catch
                            {
                                pb.Image = null;
                            }
                        }
                        else
                        {
                            pb.Image = null;
                        }
                    }
                    else
                    {
                        pb.Image = null;
                    }
                }
                else
                {
                    pb.Image = null;
                }
            }

        }
    }
}
