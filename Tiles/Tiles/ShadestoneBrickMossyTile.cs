using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Tiles.Plants;
using Redemption.Dusts.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Dusts;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneBrickMossyTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 1000;
            MineResist = 11f;
            HitSound = CustomSounds.BrickHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mossy Shadestone Brick");
            AddMapEntry(new Color(22, 26, 35));
        }
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, ModContent.DustType<VoidFlame>(), 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<NanoAxe2>())
                return true;
            return false;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<ShadestoneBrickTile>();
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tile = Framing.GetTileSafely(i, j);
            if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                if (tile.Slope != SlopeType.SlopeUpLeft && tile.Slope != SlopeType.SlopeUpRight)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<NooserootVines>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
            if (WorldGen.genRand.NextBool(8))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<ShadestoneBrickTile>(), Type, false);
        }
    }
}
