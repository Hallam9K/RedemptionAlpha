using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedCrimstoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = ModContent.DustType<IrradiatedStoneDust>();
			AddMapEntry(new Color(20, 20, 20));
        }
        public class IrradiatedCrimstoneWall : PlaceholderTile
        {
            public override string Texture => "Redemption/Placeholder";
            public override void SetStaticDefaults()
            {
                Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            }

            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.createWall = ModContent.WallType<IrradiatedCrimstoneWallTile>();
            }
        }
    }
}