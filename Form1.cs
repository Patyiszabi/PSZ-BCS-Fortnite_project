using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PSZ_BCS_Fortnite_project
{
    public partial class Form1 : Form
    {
        // fájlok
        private readonly string weaponFile = "fegyverek.txt";
        private readonly string itemFile = "Itemek.txt";
        private readonly string nameFile = "nevek.txt";

        // mappák
        private readonly string skinFolder = "skinek";
        private readonly string weaponsImgFolder = "Fegyverek";
        private readonly string itemsImgFolder = "Itemek";

        private Random rnd = new Random();

        private List<Player> players = new List<Player>();
        private List<WeaponDef> weaponPool = new List<WeaponDef>();
        private List<string> itemPool = new List<string>();
        private List<string> namePool = new List<string>();

        //Fegyver dictionary
        private readonly Dictionary<string, string> WeaponImages = new Dictionary<string, string>
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

        //Item dictionary
        private readonly Dictionary<string, string> ItemImages = new Dictionary<string, string>
        {
            { "Bush", "bush.jpg" },
            { "Grappler", "grap.jpg" },
            { "Harpoon Gun", "harp.jpg" },
            { "Crash Pad", "jump.jpg" },
            { "Junk Rift", "junkr.jpg" },
            { "Buried Treasure", "map.jpg" },
            { "Boogie Bomb", "party.jpg" },
            { "Rift-To-Go", "rift.jpg" },
            { "Shadow Bomb", "shadow.jpg" },
            { "Shockwave Grenade", "shock.jpg" },
            { "Storm Flip", "storm.jpg" }

        };

        public Form1()
        {
            InitializeComponent();
        }

        #region Domain
        private class Player
        {
            public string Name;
            public int Level;
            public int Life;
            public int Shield;
            public int Kills;
            public int XP => Kills * 80000;
            public List<PlayerWeapon> Weapons = new List<PlayerWeapon>();
            public List<PlayerItem> Items = new List<PlayerItem>();
            public string AvatarPath;
        }

        private class WeaponDef
        {
            public string Name;
            public string Rarity;
            public int MaxMag;
            public WeaponDef(string name, string rarity, int maxMag)
            {
                Name = name; Rarity = rarity; MaxMag = maxMag;
            }
        }

        private class PlayerWeapon
        {
            public WeaponDef Def;
            public int Ammo;
            public PlayerWeapon(WeaponDef def, int ammo)
            {
                Def = def; Ammo = ammo;
            }
        }

        private class PlayerItem
        {
            public string Name;
            public int Quantity;
            public PlayerItem(string name, int qty)
            {
                Name = name; Quantity = qty;
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            // események
            listbox_players.SelectedIndexChanged += Listbox_players_SelectedIndexChanged;
            lbl_game_title.Click += Lbl_game_title_Click;
            ptb_avatar.Click += Picture_Click;
            var weaponBoxes = new PictureBox[] { ptb_weapon1, ptb_weapon2, ptb_weapon3, ptb_weapon4, ptb_weapon5 };
            foreach (var pb in weaponBoxes) pb.Click += Picture_Click;
            var itemBoxes = new PictureBox[] { ptb_item1, ptb_item2, ptb_item3, ptb_item4, ptb_item5, ptb_item6, ptb_item7, ptb_item8 };
            foreach (var pb in itemBoxes) pb.Click += Picture_Click;

            // fájlok
            LoadNames();
            LoadWeapons();
            LoadItems();

            // skinek
            string skinDir = Path.Combine(Application.StartupPath, skinFolder);
            string[] skins = Directory.Exists(skinDir) ? Directory.GetFiles(skinDir).Where(IsImage).ToArray() : new string[0];

            GeneratePlayers(8, skins);

            listbox_players.Items.Clear();
            foreach (var p in players) listbox_players.Items.Add(p.Name);

            label1.Text = $"Elérhető nevek:{namePool.Count}\nElérhető fegyverek: {weaponPool.Count}\nElérhető itemek: {itemPool.Count}";
        }

        #region Fájlok
        private void LoadNames()
        {
            namePool.Clear();
            string p = Path.Combine(Application.StartupPath, nameFile);
            if (!File.Exists(p)) return;
            foreach (var line in File.ReadAllLines(p))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                foreach (var part in line.Split(',')) { var n = part.Trim(); if (!string.IsNullOrEmpty(n)) namePool.Add(n); }
            }
        }

        private void LoadWeapons()
        {
            weaponPool.Clear();
            string p = Path.Combine(Application.StartupPath, weaponFile);
            if (!File.Exists(p)) return;
            foreach (var line in File.ReadAllLines(p))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                string name = parts[0].Trim();
                string rarity = parts.Length > 1 ? parts[1].Trim() : "Common";
                int maxMag = parts.Length > 2 ? int.TryParse(parts[2].Trim(), out int m) ? m : 0 : 0;
                weaponPool.Add(new WeaponDef(name, rarity, maxMag));
            }
        }

        private void LoadItems()
        {
            itemPool.Clear();
            string p = Path.Combine(Application.StartupPath, itemFile);
            if (!File.Exists(p)) return;
            foreach (var line in File.ReadAllLines(p))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                foreach (var part in line.Split(',')) { var n = part.Trim(); if (!string.IsNullOrEmpty(n)) itemPool.Add(n); }
            }
        }
        #endregion

        #region Generálás
        private void GeneratePlayers(int count, string[] skinFiles)
        {
            players.Clear();

            // Elérhető nevek listája
            var availableNames = new List<string>(namePool);
            if (availableNames.Count == 0)
            {
                availableNames = new List<string> { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7", "Player8", "Guest" };
            }

            for (int i = 0; i < count; i++)
            {
                // Név kiválasztása
                string name;
                if (availableNames.Count > 0)
                {
                    int idx = rnd.Next(availableNames.Count);
                    name = availableNames[idx];
                    availableNames.RemoveAt(idx);
                }
                else name = $"Player_{i + 1}";

                var p = new Player
                {
                    Name = name,
                    Level = rnd.Next(1, 201),
                    Life = rnd.Next(0, 101),
                    Shield = rnd.Next(0, 101),
                    Kills = rnd.Next(1, 101)
                };

                //Fegyverek kiosztasa
                int maxWeaponCount = Math.Min(5, weaponPool.Count);
                int playerWeaponCount = rnd.Next(1, maxWeaponCount + 1);
                var weaponIndices = new HashSet<int>();
                while (weaponIndices.Count < playerWeaponCount) weaponIndices.Add(rnd.Next(weaponPool.Count));
                foreach (var idx in weaponIndices)
                {
                    var w = weaponPool[idx];
                    int ammo = w.MaxMag > 0 ? rnd.Next(1, w.MaxMag + 1) : 0;
                    p.Weapons.Add(new PlayerWeapon(w, ammo));
                }

                // Itemek kiosztasa
                int maxItemCount = Math.Min(8, itemPool.Count);
                int playerItemCount = rnd.Next(1, maxItemCount + 1);
                var itemIndices = new HashSet<int>();
                while (itemIndices.Count < playerItemCount) itemIndices.Add(rnd.Next(itemPool.Count));
                foreach (var idx in itemIndices)
                {
                    string itemName = itemPool[idx];
                    int qty = rnd.Next(1, 6); // 1..5 darab
                    p.Items.Add(new PlayerItem(itemName, qty));
                }

                //Skin kiosztas
                if (skinFiles.Length > 0)
                {
                    int sIdx = rnd.Next(skinFiles.Length);
                    p.AvatarPath = skinFiles[sIdx];
                }

                players.Add(p);
            }
        }

        #endregion

        #region UI
        private void Listbox_players_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listbox_players.SelectedIndex < 0) return;
            ShowPlayer(players[listbox_players.SelectedIndex]);
        }

        private void ShowPlayer(Player p)
        {
            // avatar
            SetPicture(ptb_avatar, p.AvatarPath);

            // fegyverek
            var wBoxes = new PictureBox[] { ptb_weapon1, ptb_weapon2, ptb_weapon3, ptb_weapon4, ptb_weapon5 };
            for (int i = 0; i < wBoxes.Length; i++)
            {
                var pb = wBoxes[i];
                if (i < p.Weapons.Count)
                {
                    var pw = p.Weapons[i];
                    string img = WeaponImages.ContainsKey(pw.Def.Name) ? Path.Combine(Application.StartupPath, weaponsImgFolder, WeaponImages[pw.Def.Name]) : null;
                    SetPicture(pb, img);
                    pb.Tag = pw;
                }
                else { SetPicture(pb, null); pb.Tag = null; }
            }

            // itemek
            var iBoxes = new PictureBox[] { ptb_item1, ptb_item2, ptb_item3, ptb_item4, ptb_item5, ptb_item6, ptb_item7, ptb_item8 };
            for (int i = 0; i < iBoxes.Length; i++)
            {
                var pb = iBoxes[i];
                if (i < p.Items.Count)
                {
                    var pi = p.Items[i];
                    string img = ItemImages.ContainsKey(pi.Name) ? Path.Combine(Application.StartupPath, itemsImgFolder, ItemImages[pi.Name]) : null;
                    SetPicture(pb, img);
                    pb.Tag = pi;
                }
                else { SetPicture(pb, null); pb.Tag = null; }
            }

            label1.Text = $"Név: {p.Name}\nSzint: {p.Level}\nÉleterő: {p.Life}\nPajzs: {p.Shield}\nKills: {p.Kills}\nXP: {p.XP}";
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            if (!(sender is PictureBox pb)) return;

            if (pb.Tag is PlayerWeapon pw)
            {
                MessageBox.Show($"Fegyver: {pw.Def.Name}\nRitkaság: {pw.Def.Rarity}\nMax tár: {pw.Def.MaxMag}\nLőszer: {pw.Ammo}", "Fegyver info");
                return;
            }

            if (pb.Tag is PlayerItem pi)
            {
                MessageBox.Show($"Item: {pi.Name}\nMennyiség: {pi.Quantity}", "Item info");
                return;
            }

            if (pb == ptb_avatar && listbox_players.SelectedIndex >= 0)
            {
                var p = players[listbox_players.SelectedIndex];
                MessageBox.Show($"Név: {p.Name}\nSzint: {p.Level}\nÉleterő: {p.Life}\nPajzs: {p.Shield}\nKills: {p.Kills} \nXP: {p.XP}", "Játékos info");
            }
        }
        #endregion

        #region Ranglista
        private void Lbl_game_title_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Add meg a ranglista szűrési kulcsát (életerő / kills / xp):", "Ranglista szűrés", "xp");
            if (string.IsNullOrWhiteSpace(input)) return;
            string key = input.Trim().ToLowerInvariant();

            IEnumerable<Player> ordered;
            if (key == "életerő")
            {
                ordered = players.OrderByDescending(p => p.Life);
            }
            else if (key == "kills")
            {
                ordered = players.OrderByDescending(p => p.Kills);
            }
            else if (key == "xp")
            {
                ordered = players.OrderByDescending(p => p.XP);
            }
            else
            {
                ordered = null;
            }
            if (ordered == null) { MessageBox.Show("Érvénytelen kulcs."); return; }

            var sb = new System.Text.StringBuilder();
            int pos = 1;
            foreach (var p in ordered)
            {
                sb.AppendLine($"{pos}. {p.Name} - Életerő:{p.Life} Kills:{p.Kills} XP:{p.XP}");
                pos++;
            }
            MessageBox.Show(sb.ToString(), "Ranglista");
        }
        #endregion

        #region Utils
        private void SetPicture(PictureBox pb, string path)
        {
            if (pb.Image != null) { pb.Image.Dispose(); pb.Image = null; }
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                pb.Image = Image.FromFile(path);
                pb.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private static bool IsImage(string file) => new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" }.Contains(Path.GetExtension(file).ToLower());

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Köszönjük, hogy játékunkat használta.");
            Application.Exit();
        }

        private void lbl_game_title_MouseHover(object sender, EventArgs e)
        {
            lbl_game_title.Text = "Kattints ide a ranglistához!";
            lbl_game_title.ForeColor = Color.Red;
        }

        private void lbl_game_title_MouseLeave(object sender, EventArgs e)
        {
            lbl_game_title.Text = "Fortnite";
            lbl_game_title.ForeColor = Color.Black;
        }
    }
}
