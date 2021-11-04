using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class LivingDeadLeavesWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(32, 32, 32));
            SoundType = 6;
        }
    }
    public class LivingDeadLeavesWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Petrified Leaves Wall");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<LivingDeadLeavesWallTile>();
        }
    }
}