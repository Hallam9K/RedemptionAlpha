using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class DeadGrassWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(45, 45, 45));
            SoundType = 6;
        }
    }
    public class DeadGrassWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<DeadGrassWallTile>();
        }
    }
}