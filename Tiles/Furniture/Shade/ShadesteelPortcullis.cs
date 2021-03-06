using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadesteelPortcullisClose : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Shadesteel Portcullis");
            AddMapEntry(new Color(83, 87, 123), name);
            MinPick = 500;
            MineResist = 12f;
            DustType = ModContent.DustType<ShadesteelDust>();
            AdjTiles = new int[] { TileID.ClosedDoor };
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            WorldGen.KillTile(i, j, noItem: true);
            WorldGen.PlaceObject(i, j, ModContent.TileType<ShadesteelPortcullisOpen>());
            NetMessage.SendObjectPlacment(-1, i, j, ModContent.TileType<ShadesteelPortcullisOpen>(), 0, 0, -1, -1);
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left, top + 2);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class ShadesteelPortcullisOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
            TileID.Sets.HousingWalls[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Shadesteel Portcullis");
            AddMapEntry(new Color(83, 87, 123), name);
            MinPick = 500;
            MineResist = 12f;
            DustType = ModContent.DustType<ShadesteelDust>();
            AdjTiles = new int[] { TileID.OpenDoor };
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            WorldGen.KillTile(i, j, noItem: true);
            WorldGen.PlaceObject(i, j, ModContent.TileType<ShadesteelPortcullisClose>());
            NetMessage.SendObjectPlacment(-1, i, j, ModContent.TileType<ShadesteelPortcullisClose>(), 0, 0, -1, -1);
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left, top + 2);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class ShadePortcullis : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }
        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<ShadesteelPortcullisClose>();
        }
    }
}