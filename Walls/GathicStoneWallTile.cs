using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class GathicStoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = true;
            DustType = ModContent.DustType<SlateDust>();
			ItemDrop = ModContent.ItemType<GathicStoneWall>();
			AddMapEntry(new Color(49, 43, 39));
		}
    }
    public class GathicStoneWallTileUnsafe : ModWall
    {
        public override string Texture => "Redemption/Walls/GathicStoneWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = ModContent.DustType<SlateDust>();
            ItemDrop = ModContent.ItemType<GathicStoneWall>();
            AddMapEntry(new Color(49, 43, 39));
        }
    }
}