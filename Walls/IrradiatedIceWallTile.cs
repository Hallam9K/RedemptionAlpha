using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class IrradiatedIceWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = DustID.Ice;
            AddMapEntry(new Color(92, 118, 47));
		}
	}
    public class IrradiatedIceWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IrradiatedIceWallTile>();
        }
    }
}