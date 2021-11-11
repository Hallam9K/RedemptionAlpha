using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class IrradiatedHardenedSandWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = ModContent.DustType<IrradiatedStoneDust>();
            AddMapEntry(new Color(34, 44, 51));
		}
    }
    public class IrradiatedHardenedSandWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IrradiatedHardenedSandWallTile>();
        }
    }
}