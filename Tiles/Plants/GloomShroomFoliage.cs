using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class GloomShroomFoliage : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 5;
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = DustID.GlowingMushroom;
            RegisterItemDrop(ItemType<GloomMushroom>());
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(142, 167, 169), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .01f;
            g = .15f;
            b = .25f;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 10;
    }
    public class GloomShroomFoliage2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 26 };
            TileObjectData.newTile.CoordinateWidth = 30;
            TileObjectData.newTile.DrawYOffset = -8;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = DustID.GlowingMushroom;
            RegisterItemDrop(ItemType<GloomMushroomBig>());
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(142, 167, 169), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .03f;
            g = .3f;
            b = .5f;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 10;
    }
}