using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneRubbleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneSlabTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneMossyTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickMossyTile>()] = true;
            ItemDrop = ModContent.ItemType<ShadestoneRubble>();
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 300;
            MineResist = 6f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Shadestone Rubble");
            AddMapEntry(new Color(65, 68, 83));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}