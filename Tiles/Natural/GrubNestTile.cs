using Microsoft.Xna.Framework;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class GrubNestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<IrradiatedSandstoneTile>() };
            TileObjectData.addTile(Type);
            DustType = DustID.Ash;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Grub Nest");
            AddMapEntry(new Color(40, 60, 40), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}