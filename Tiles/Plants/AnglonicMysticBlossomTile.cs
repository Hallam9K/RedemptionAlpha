using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent.Metadata;
using Redemption.Items.Placeable.Plants;

namespace Redemption.Tiles.Plants
{
    public class AnglonicMysticBlossomTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Anglonic Mystic Blossom");
            AddMapEntry(new Color(235, 175, 255), name);
            RegisterItemDrop(ModContent.ItemType<AnglonicMysticBlossom>());
            DustType = DustID.GrassBlades;
            HitSound = SoundID.Grass;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
    }
}
