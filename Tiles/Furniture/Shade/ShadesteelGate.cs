using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Redemption.Dusts.Tiles;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadesteelGateClose : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
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
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadesteel Gate");
            AddMapEntry(new Color(83, 87, 123), name);
            MinPick = 1000;
            MineResist = 12f;
            DustType = ModContent.DustType<ShadesteelDust>();
            AdjTiles = new int[] { TileID.ClosedDoor };
        }
        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.DoorOpen);
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            WorldGen.KillTile(i, j, noItem: true);
            WorldGen.PlaceObject(i, j, ModContent.TileType<ShadesteelGateOpen>());
            NetMessage.SendObjectPlacement(-1, i, j, ModContent.TileType<ShadesteelGateOpen>(), 0, 0, -1, -1);
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
            return true;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
        public override bool CanExplode(int i, int j) => false;
    }
    public class ShadesteelGateOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleLineSkip = 3;
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
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadesteel Gate");
            AddMapEntry(new Color(83, 87, 123), name);
            MinPick = 1000;
            MineResist = 12f;
            DustType = ModContent.DustType<ShadesteelDust>();
            AdjTiles = new int[] { TileID.OpenDoor };
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.DoorClosed);
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            WorldGen.KillTile(i, j, noItem: true);
            WorldGen.PlaceObject(i, j, ModContent.TileType<ShadesteelGateClose>());
            NetMessage.SendObjectPlacement(-1, i, j, ModContent.TileType<ShadesteelGateClose>(), 0, 0, -1, -1);
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class ShadesteelGate : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<ShadesteelGateClose>();
        }
    }
}