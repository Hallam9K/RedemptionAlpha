using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabCabinetTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 300;
            MineResist = 8f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Laboratory Cabinet");
            AddMapEntry(new Color(189, 191, 200), name);
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
            int left = i - Main.tile[i, j].frameX / 18 % 2;
            int top = j - Main.tile[i, j].frameY / 18 % 2;
            if (Main.tile[left, top].frameX == 0)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(ModContent.ItemType<RadiationPill>());
                if (Main.rand.NextBool(40))
                    player.QuickSpawnItem(ModContent.ItemType<EmptyMutagen>());
                if (Main.rand.NextBool(4))
                    player.QuickSpawnItem(ModContent.ItemType<FirstAidKit>());
                if (Main.rand.NextBool(20))
                    player.QuickSpawnItem(ItemID.AdhesiveBandage);
                if (Main.rand.NextBool(20))
                    player.QuickSpawnItem(ItemID.Vitamins);
                if (Main.rand.NextBool(66666))
                    player.QuickSpawnItem(ModContent.ItemType<Panacea>());
            }
            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].frameX < 36)
                        Main.tile[x, y].frameX += 36;
                }
            }
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class LabCabinet : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Cabinet");
            Tooltip.SetDefault("Gives Radiation Pills");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<LabCabinetTile>();
        }
    }
}
