using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ObjectData;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;

namespace Redemption.Tiles.Furniture.PetrifiedWood
{
    public class PetrifiedWoodLanternTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(100, 100, 100), Language.GetText("MapObject.Lantern"));
            AdjTiles = new int[] { TileID.HangingLanterns };
            RegisterItemDrop(ModContent.ItemType<PetrifiedWoodLantern>());
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = DustID.Ash;
        }
        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            for (int x = left; x < left + 1; x++)
            {
                for (int y = top; y < top + 2; y++)
                {

                    if (Main.tile[x, y].TileFrameX >= 18)
                        Main.tile[x, y].TileFrameX -= 18;
                    else
                        Main.tile[x, y].TileFrameX += 18;
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.7f;
                g = 0.7f;
                b = 0.7f;
            }
        }
    }
}
