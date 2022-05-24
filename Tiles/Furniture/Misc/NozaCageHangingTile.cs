using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts;
using Redemption.Items.Placeable.Furniture.Misc;

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
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cage");
            AddMapEntry(new Color(116, 121, 144), name);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<NozaCageHanging>());
        public override bool CanExplode(int i, int j) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
