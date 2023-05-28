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
    public class IrradiatedEbonstoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileStone[Type] = true;
            Main.tileMerge[Type][TileID.Crimstone] = true;
            Main.tileMerge[TileID.Crimstone][Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.Ebonstone] = true;
            Main.tileMerge[TileID.Ebonstone][Type] = true;
            Main.tileMerge[Type][TileID.Pearlstone] = true;
            Main.tileMerge[TileID.Pearlstone][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCrimstoneTile>()] = true;
            TileID.Sets.Conversion.Stone[Type] = true;
            TileID.Sets.Stone[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = true;
            TileID.Sets.Corrupt[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            DustType = DustID.Ash;
            MinPick = 100;
            MineResist = 2.5f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(59, 58, 64));
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
            Tile tileAbove2 = Framing.GetTileSafely(i, j - 2);

            if (!tileAbove.HasTile && !tileAbove2.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && !tileBelow2.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<DeadRockStalacmitesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<DeadRockStalacmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<DeadRockStalacmites2Tile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<DeadRockStalacmites2Tile>(), 0, 0, -1, -1);
            }
            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.02f;
            g = 0.01f;
            b = 0.04f;
        }
    }
}