using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Shade;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadesteelHangingCellTile : ModTile
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
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleLineSkip = 3;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Hanging Cell");
            AddMapEntry(new Color(83, 87, 123), name);
            DustType = ModContent.DustType<ShadesteelDust>();
            MinPick = 310;
            MineResist = 11f;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class ShadesteelHangingCell2Tile : ModTile
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
            RegisterItemDrop(ModContent.ItemType<ShadesteelHangingCell>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Hanging Cell");
            AddMapEntry(new Color(83, 87, 123), name);
            DustType = ModContent.DustType<ShadesteelDust>();
            AnimationFrameHeight = 72;
            MinPick = 310;
            MineResist = 11f;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frame == 0)
            {
                if (frameCounter++ > Main.rand.Next(120, 300))
                {
                    if (!Main.rand.NextBool(3))
                    {
                        frame = 1;
                    }
                    frameCounter = 0;
                }
            }
            else if (frame >= 1 && frame <= 5)
            {
                frameCounter++;
                if (frameCounter >= 10)
                {
                    frameCounter = 0;
                    frame++;
                }
            }
            else if (frame == 6)
            {
                if (frameCounter++ > Main.rand.Next(100, 200))
                {
                    if (!Main.rand.NextBool(3))
                    {
                        frame = 7;
                    }
                    frameCounter = 0;
                }
            }
            else if (frame >= 7)
            {
                frameCounter++;
                if (frameCounter >= 10)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame >= 8)
                    {
                        frame = 0;
                    }
                }
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class ShadesteelHangingCell2 : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Hanging Shadesteel Cell (With Echo)");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<ShadesteelHangingCell2Tile>();
        }
    }
}
