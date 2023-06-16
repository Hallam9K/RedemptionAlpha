using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Lab
{
    public class MossyLabTableTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            DustType = ModContent.DustType<LabPlatingDust>();
            AdjTiles = new int[] { TileID.Tables };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            // Etc
            AddMapEntry(new Color(189, 191, 200), Language.GetText("MapObject.Table"));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class MossyLabTable : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Mossy Laboratory Table");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<MossyLabTableTile>();
        }
    }
}
