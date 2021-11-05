using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class LivingDeadLeavesTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<LivingDeadWoodTile>()] = true;
            Main.tileMerge[ModContent.TileType<LivingDeadWoodTile>()][Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.Leaves[Type] = true;
            AddMapEntry(new Color(65, 66, 64));
            MineResist = 1f;
            DustType = DustID.Ash;
            SoundType = SoundID.Grass;
        }
    }
}

