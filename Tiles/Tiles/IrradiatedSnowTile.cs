using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Redemption.Tiles.Natural;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedSnowTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            DustType = DustID.Ash;
            HitSound = SoundID.Item126;
            AddMapEntry(new Color(204, 215, 191));
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
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            Radiation modPlayer = player.RedemptionRad();
            BuffPlayer suit = player.RedemptionPlayerBuff();
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (!fail && dist <= 4 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(6) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller1, player.position);

                if (Main.rand.NextBool(100) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && !tileBelow2.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), 0, 0, -1, -1);
            }
            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }
    }
}