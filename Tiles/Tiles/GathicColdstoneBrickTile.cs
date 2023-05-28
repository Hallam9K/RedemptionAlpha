using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class GathicColdstoneBrickTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientHallBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicGladestoneTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicGladestoneBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicStoneBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicStoneTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicFroststoneTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicFroststoneBrickTile>()][Type] = true;
            Main.tileMerge[ModContent.TileType<GathicFroststoneTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicFroststoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicColdstoneTile>()] = true;
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
            RegisterItemDrop(ModContent.ItemType<GathicFroststoneBrick>(), 0);
            DustType = DustID.Ice;
            HitSound = CustomSounds.StoneHit;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(125, 135, 186));
        }
        public override void RandomUpdate(int i, int j)
        {
            bool tileUp = !Framing.GetTileSafely(i, j - 1).HasTile;
            bool tileDown = !Framing.GetTileSafely(i, j + 1).HasTile;
            bool tileLeft = !Framing.GetTileSafely(i - 1, j).HasTile;
            bool tileRight = !Framing.GetTileSafely(i + 1, j).HasTile;
            if (Main.rand.NextBool(1000) && j > (int)(Main.maxTilesY * .25f) && RedeGen.cryoCrystalSpawn)
            {
                if (tileUp)
                {
                    WorldGen.PlaceObject(i, j - 1, ModContent.TileType<CryoCrystalTile>(), true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                }
                else if (tileDown)
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<CryoCrystalTile>(), true);
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                }
                else if (tileLeft)
                {
                    WorldGen.PlaceObject(i - 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                    NetMessage.SendObjectPlacement(-1, i - 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                }
                else if (tileRight)
                {
                    WorldGen.PlaceObject(i + 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                    NetMessage.SendObjectPlacement(-1, i + 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                }
            }
        }
    }
}