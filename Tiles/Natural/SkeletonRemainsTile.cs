using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class SkeletonRemainsTile1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Bone;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Skeletal Remains");
            AddMapEntry(new Color(129, 129, 95));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<GraveSteelShards>(), Main.rand.Next(5, 9));
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class SkeletonRemainsTile2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Bone;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Skeletal Remains");
            AddMapEntry(new Color(129, 129, 95));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 16, ModContent.ItemType<GraveSteelShards>(), Main.rand.Next(3, 6));
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class SkeletonRemains1 : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Remains (Sword)");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SkeletonRemainsTile1>();
        }
    }
    public class SkeletonRemains2 : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Remains (Front-facing)");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SkeletonRemainsTile2>();
        }
    }
}