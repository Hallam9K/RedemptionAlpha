using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedCrimstoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			WallID.Sets.Crimson[Type] = true;
			DustType = DustID.Ash;
			AddMapEntry(new Color(50, 40, 40));
        }
        public class IrradiatedCrimstoneWall : PlaceholderTile
        {
            public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
            public override void SetSafeStaticDefaults()
            {
                // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            }

            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.createWall = ModContent.WallType<IrradiatedCrimstoneWallTile>();
            }
        }
    }
}