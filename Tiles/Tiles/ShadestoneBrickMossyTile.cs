using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Plants;
using Redemption.Dusts.Tiles;
using Redemption.Items.Tools.PostML;

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
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneRubbleTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneSlabTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneMossyTile>()] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 500;
            MineResist = 11f;
            HitSound = CustomSounds.BrickHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mossy Shadestone Brick");
            AddMapEntry(new Color(22, 26, 35));
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
            if (!Framing.GetTileSafely(i, j + 1).HasTile && Framing.GetTileSafely(i, j).HasTile)
            {
                if (Main.rand.NextBool(5))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Small>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Small>(), Main.rand.Next(3), 0, -1, -1);
                }
                if (Main.rand.NextBool(7))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Medium>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Medium>(), Main.rand.Next(3), 0, -1, -1);
                }
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Large>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Large>(), Main.rand.Next(3), 0, -1, -1);
                }
            }
            if (Main.rand.NextBool(8))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<ShadestoneBrickTile>(), Type, false);
        }
    }
}