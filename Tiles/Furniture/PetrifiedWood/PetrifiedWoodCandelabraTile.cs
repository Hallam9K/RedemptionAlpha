using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;

namespace Redemption.Tiles.Furniture.PetrifiedWood
{
    public class PetrifiedWoodCandelabraTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Petrified Wood Candelabra");
            AddMapEntry(new Color(100, 100, 100), name);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[]{ TileID.Candelabras };
            DustType = DustID.Ash;
        }
        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX / 18 % 2;
            int top = j - Main.tile[i, j].frameY / 18 % 2;
            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {

                    if (Main.tile[x, y].frameX >= 36)
                        Main.tile[x, y].frameX -= 36;
                    else
                        Main.tile[x, y].frameX += 36;
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left + 1, top);
                Wiring.SkipWire(left + 1, top + 1);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 36)
            {
                r = 0.7f;
                g = 0.7f;
                b = 0.7f;
            }
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<PetrifiedWoodCandelabra>());
    }
}