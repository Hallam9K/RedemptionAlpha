using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedHardenedSandWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(86, 62, 54));
		}
    }
    public class IrradiatedHardenedSandWall : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IrradiatedHardenedSandWallTile>();
        }
    }
}