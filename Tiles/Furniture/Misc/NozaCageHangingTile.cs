using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace Redemption.Tiles.Furniture.Misc
{
    public class NozaCageHangingTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);
            DustType = DustID.Lead;
            MinPick = 50;
            MineResist = 4f;

            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Cage");
            AddMapEntry(new Color(116, 121, 144), name);
        }
        public override bool CanExplode(int i, int j) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
