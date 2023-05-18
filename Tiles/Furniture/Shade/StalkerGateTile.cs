using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class StalkerGateTile : ModTile
    {
        private bool _activated = false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Height, 0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadesteel Grate");
            AddMapEntry(new Color(83, 87, 123), name);
            MinPick = 1000;
            MineResist = 30f;
            DustType = ModContent.DustType<ShadesteelDust>();
            AnimationFrameHeight = 36;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (_activated)
                frame = 1;
            else
                frame = 0;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (SoullessArea.soullessInts[1] > 1 && !_activated)
                _activated = true;

            if (_activated)
            {
                Main.tileSolid[Type] = false;
                Main.tileBlockLight[Type] = false;
            }
            else
            {
                Main.tileSolid[Type] = true;
                Main.tileBlockLight[Type] = true;
            }
        }
    }
    public class StalkerGate : PlaceholderTile
    {
        public override void SetSafeStaticDefaults()
        {
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<StalkerGateTile>();
        }
    }
}