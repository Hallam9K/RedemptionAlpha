using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedDirtWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = DustID.Ash;
			AddMapEntry(new Color(69, 58, 52));
        }
        public class IrradiatedDirtWall : PlaceholderTile
        {
            public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
            public override void SetSafeStaticDefaults()
            {
                // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            }

            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.createWall = ModContent.WallType<IrradiatedDirtWallTile>();
            }
        }
    }
}