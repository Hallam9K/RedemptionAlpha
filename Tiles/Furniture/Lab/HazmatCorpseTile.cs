using Microsoft.Xna.Framework;
using Redemption.Items;
using Redemption.Items.Accessories.HM;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class HazmatCorpseTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.GreenBlood;
            MinPick = 500;
            MineResist = 8f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Hazmat Corpse");
            AddMapEntry(new Color(242, 183, 111), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<HintIcon>();
        }
        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX / 18 % 3;
            int top = j - Main.tile[i, j].frameY / 18 % 2;
            if (Main.tile[left, top].frameX == 0)
            {
                Player player = Main.LocalPlayer; // TODO: crowbar and hazmat corpse drop
                //player.QuickSpawnItem(ModContent.ItemType<Crowbar>());
                //player.QuickSpawnItem(ModContent.ItemType<HazmatSuit>());
            }
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].frameX < 54)
                        Main.tile[x, y].frameX += 54;
                }
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int left = i - Main.tile[i, j].frameX / 18 % 3;
            int top = j - Main.tile[i, j].frameY / 18 % 2;
            if (Main.tile[left, top].frameX == 0)
            {
                Player player = Main.LocalPlayer;
                //player.QuickSpawnItem(ModContent.ItemType<Crowbar>());
                //player.QuickSpawnItem(ModContent.ItemType<HazmatSuit>());
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class HazmatCorpse : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hazmat Corpse");
            Tooltip.SetDefault("Gives Crowbar and Hazmat Suit" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<HazmatCorpseTile>();
        }
    }
}
