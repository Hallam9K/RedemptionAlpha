using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class ToxicAngel2Tile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 5;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<AncientGrassTile>(), ModContent.TileType<AncientDirtTile>(), ModContent.TileType<AncientSlateTile>(), ModContent.TileType<AncientLushGrassTile>(), ModContent.TileType<OvergrownAncientSlateBeamTile>(), ModContent.TileType<OvergrownAncientSlateBrickTile>() };
            TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = DustID.PinkFairy;
            RegisterItemDrop(ModContent.ItemType<ToxicAngel2>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Purified Toxic Angel");
            AddMapEntry(new Color(240, 200, 120), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .5f;
            g = 1f;
            b = 1f;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 10;
    }
}