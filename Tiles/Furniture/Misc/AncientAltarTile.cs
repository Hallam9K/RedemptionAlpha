using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class AncientAltarTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.addTile(Type);
            DustType = 7;
            MinPick = 500;
            MineResist = 3f;
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ancient Altar");
            AddMapEntry(new Color(120, 190, 40), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CursedGem>();
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;

        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<CursedGem>());
            }
            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 3; y++)
                {
                    if (Main.tile[x, y].TileFrameX < 36)
                    {
                        Main.tile[x, y].TileFrameX += 36;
                    }
                }
            }
            return true;
        }
    }
    public class AncientAltar : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Altar");
            Tooltip.SetDefault("Gives the Cursed Gem" +
                "\n[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<AncientAltarTile>();
        }
    }
}