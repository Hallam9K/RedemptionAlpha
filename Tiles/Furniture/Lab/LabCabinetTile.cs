using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
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
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
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
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Laboratory Cabinet");
            AddMapEntry(new Color(189, 191, 200), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            return Main.tile[left, top].TileFrameX == 0;
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
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<RadiationPill>());
                if (Main.rand.NextBool(40))
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<EmptyMutagen>());
                if (Main.rand.NextBool(4))
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<FirstAidKit>());
                if (Main.rand.NextBool(20))
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ItemID.AdhesiveBandage);
                if (Main.rand.NextBool(20))
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ItemID.Vitamins);
                if (Main.rand.NextBool(666))
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<Panacea>());
            }
            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX < 36)
                        Main.tile[x, y].TileFrameX += 36;
                }
            }
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class LabCabinet : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Cabinet");
            // Tooltip.SetDefault("Gives Radiation Pills");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<LabCabinetTile>();
        }
    }
}
