using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class AncientFallenStatueTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.CoordinateHeights = new int[] { 18, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(4, 6);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 4, 3);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(2, 6);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 4, 1);
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            MinPick = 1000;
            MineResist = 15f;
            HitSound = CustomSounds.StoneHit;
            AddMapEntry(new Color(104, 91, 83));
            RegisterItemDrop(ModContent.ItemType<AncientFallenStatue>());
            DustType = ModContent.DustType<SlateDust>();
        }
        public override bool CanExplode(int i, int j) => false;
    }
}