using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.BaseExtension;

namespace Redemption.Tiles.Furniture.Misc
{
    public class HKStatueTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(2, 9);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 500;
            MineResist = 15f;
            AddMapEntry(new Color(104, 91, 83));
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            player.AddBuff(ModContent.BuffType<HKStatueBuff>(), 20);
            player.Redemption().foundHall = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<HKStatue>());
        }

        public override bool CanExplode(int i, int j) => false;
    }
}