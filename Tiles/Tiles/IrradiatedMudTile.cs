using Redemption.BaseExtension;
using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedMudTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedJungleGrassTile>()] = true;
            Main.tileMerge[TileType<IrradiatedJungleGrassTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.JungleGrass] = true;
            Main.tileMerge[TileID.JungleGrass][Type] = true;
            Main.tileMerge[Type][TileID.MushroomGrass] = true;
            Main.tileMerge[TileID.MushroomGrass][Type] = true;
            Main.tileMerge[Type][TileID.CorruptJungleGrass] = true;
            Main.tileMerge[TileID.CorruptJungleGrass][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonJungleGrass] = true;
            Main.tileMerge[TileID.CrimsonJungleGrass][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Mud[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(77, 67, 69));
            MinPick = 10;
            MineResist = 1f;
            DustType = DustID.Ash;
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
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertTile(i, j, TileID.Mud);
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (!fail && dist <= 4)
                player.RedemptionRad().Irradiate(.05f, 0, 2, 1, 6);
        }
    }
}