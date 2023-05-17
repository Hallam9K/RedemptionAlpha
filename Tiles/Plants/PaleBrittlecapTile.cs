using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class PaleBrittlecapTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<AncientGrassTile>(), ModContent.TileType<AncientDirtTile>() }; //ModContent.TileType<AncientSlateTile>() };
            TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = ModContent.DustType<SlateDust>();
            RegisterItemDrop(ModContent.ItemType<PaleBrittlecap>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Pale Brittlecap");
            AddMapEntry(new Color(170, 150, 110), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 10;
    }
}