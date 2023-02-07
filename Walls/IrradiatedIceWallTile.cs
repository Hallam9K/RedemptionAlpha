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
            DustType = DustID.Ash;
            AddMapEntry(new Color(65, 97, 66));
		}
	}
    public class IrradiatedIceWall : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IrradiatedIceWallTile>();
        }
    }
}