using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class GathicFroststoneBrickWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
            DustType = DustID.Ice;
            HitSound = SoundID.Item50;
            AddMapEntry(new Color(24, 91, 144));
        }
    }
	public class GathicFroststoneBrickWallTileUnsafe : ModWall
	{
		public override string Texture => "Redemption/Walls/GathicFroststoneBrickWallTile";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
            DustType = DustID.Ice;
            HitSound = SoundID.Item50;
            AddMapEntry(new Color(24, 91, 144));
        }
    }
}