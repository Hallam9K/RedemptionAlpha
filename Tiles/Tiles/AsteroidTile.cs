using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
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
            Main.tileMerge[Type][TileID.Meteorite] = true;
            Main.tileMerge[TileID.Meteorite][Type] = true;
            Main.tileMerge[Type][TileID.MeteoriteBrick] = true;
            Main.tileMerge[TileID.MeteoriteBrick][Type] = true;
            ItemDrop = ModContent.ItemType<Asteroid>();
            DustType = ModContent.DustType<SlateDust>();
            HitSound = SoundID.Tink;
            MinPick = 50;
            MineResist = 2.5f;
            AddMapEntry(new Color(84, 76, 79));
        }
    }
}