using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class PlantMatterTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            Main.tileMerge[Type][TileID.CorruptGrass] = true;
            Main.tileMerge[TileID.CorruptGrass][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonGrass] = true;
            Main.tileMerge[TileID.CrimsonGrass][Type] = true;
            Main.tileMerge[Type][TileID.HallowedGrass] = true;
            Main.tileMerge[TileID.HallowedGrass][Type] = true;
            Main.tileMerge[Type][TileID.JungleGrass] = true;
            Main.tileMerge[TileID.JungleGrass][Type] = true;
            Main.tileMerge[Type][TileID.GolfGrass] = true;
            Main.tileMerge[TileID.GolfGrass][Type] = true;
            Main.tileMerge[Type][TileID.GolfGrassHallowed] = true;
            Main.tileMerge[TileID.GolfGrassHallowed][Type] = true;
            Main.tileMerge[Type][TileID.MushroomGrass] = true;
            Main.tileMerge[TileID.MushroomGrass][Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = DustID.GrassBlades;
            MinPick = 0;
            MineResist = 1f;
            HitSound = SoundID.Grass;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Plant Matter");
            AddMapEntry(new Color(109, 155, 67), name);
		}
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, DustType, 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}