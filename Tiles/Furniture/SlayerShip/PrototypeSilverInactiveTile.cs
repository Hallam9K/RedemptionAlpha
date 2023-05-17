using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Redemption.Items.Materials.HM;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class PrototypeSilverInactiveTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            HitSound = SoundID.Tink;
            MinPick = 200;
            MineResist = 9f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Inactive Prototype Silver Mk.I");
            AddMapEntry(new Color(189, 191, 200), name);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.rand.NextBool(2))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<CarbonMyofibre>(), Main.rand.Next(4, 7));
            if (Main.rand.NextBool(3))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<Plating>(), Main.rand.Next(2, 5));
            if (Main.rand.NextBool(3))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<Capacitor>(), Main.rand.Next(1, 3));
            if (Main.rand.NextBool(6))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<AIChip>());
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class PrototypeSilverInactive : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Inactive Prototype Silver");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<AndroidInactiveTile>();
        }
    }
}
