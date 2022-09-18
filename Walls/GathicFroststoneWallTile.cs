using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class GathicFroststoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = true;
            DustType = DustID.Ice;
            HitSound = SoundID.Item50;
            ItemDrop = ModContent.ItemType<GathicFroststoneWall>();
			AddMapEntry(new Color(24, 91, 144));
		}
    }
    public class GathicFroststoneWallTileUnsafe : ModWall
    {
        public override string Texture => "Redemption/Walls/GathicFroststoneWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ice;
            HitSound = SoundID.Item50;
            ItemDrop = ModContent.ItemType<GathicFroststoneWall>();
            AddMapEntry(new Color(24, 91, 144));
        }
    }
}