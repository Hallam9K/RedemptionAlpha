using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
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
            public override string Texture => "Redemption/Placeholder";
            public override void SetStaticDefaults()
            {
                Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            }

            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.createWall = ModContent.WallType<IrradiatedDirtWallTile>();
            }
        }
    }
}