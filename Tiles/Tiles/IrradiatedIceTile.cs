using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedIceTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.Conversion.Ice[Type] = true;
            TileID.Sets.Ices[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.IcesSlush[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedSnowTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSnowTile>()] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[TileID.IceBrick][Type] = true;
            Main.tileMerge[Type][TileID.IceBrick] = true;
            Main.tileMerge[TileID.CorruptIce][Type] = true;
            Main.tileMerge[Type][TileID.CorruptIce] = true;
            Main.tileMerge[TileID.FleshIce][Type] = true;
            Main.tileMerge[Type][TileID.FleshIce] = true;
            Main.tileMerge[TileID.HallowedIce][Type] = true;
            Main.tileMerge[Type][TileID.HallowedIce] = true;
            Main.tileLighted[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(163, 183, 155));
            SoundStyle = 50;
            SoundType = SoundID.Item;
            ItemDrop = ModContent.ItemType<IrradiatedIce>();
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.04f;
            b = 0.0f;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            Radiation modPlayer = player.GetModPlayer<Radiation>();
            BuffPlayer suit = player.GetModPlayer<BuffPlayer>();
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (!fail && dist <= 4 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(6) && !Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Muller1").WithVolume(.9f).WithPitchVariance(.1f), player.position);

                if (Main.rand.NextBool(100) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && !tileBelow2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<RadioactiveIciclesTile>(), 0, 0, -1, -1);
            }
        }
    }
}

