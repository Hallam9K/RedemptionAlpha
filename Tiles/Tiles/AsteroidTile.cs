using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class AsteroidTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][TileID.Meteorite] = true;
            Main.tileMerge[TileID.Meteorite][Type] = true;
            Main.tileMerge[Type][TileID.MeteoriteBrick] = true;
            Main.tileMerge[TileID.MeteoriteBrick][Type] = true;
            Main.tileMerge[Type][TileID.Iron] = true;
            Main.tileMerge[TileID.Iron][Type] = true;
            Main.tileMerge[Type][TileID.Cobalt] = true;
            Main.tileMerge[TileID.Cobalt][Type] = true;
            Main.tileMerge[Type][TileID.Gold] = true;
            Main.tileMerge[TileID.Gold][Type] = true;
            Main.tileMerge[Type][TileID.Platinum] = true;
            Main.tileMerge[TileID.Platinum][Type] = true;
            Main.tileMerge[Type][TileID.BreakableIce] = true;
            Main.tileMerge[TileID.BreakableIce][Type] = true;
            Main.tileMerge[Type][TileID.LunarOre] = true;
            Main.tileMerge[TileID.LunarOre][Type] = true;
            DustType = ModContent.DustType<SlateDust>();
            HitSound = SoundID.Tink;
            MinPick = 50;
            MineResist = 2.5f;
            AddMapEntry(new Color(87, 79, 69));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.05f;
            g = 0.05f;
            b = 0.05f;
        }
    }
}