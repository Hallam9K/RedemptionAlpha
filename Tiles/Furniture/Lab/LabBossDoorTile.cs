using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabBossDoorTile : ModTile
    {
        private bool _activated = false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Reinforced Door");
            AddMapEntry(new Color(110, 106, 120), name);
            MinPick = 1000;
            MineResist = 10f;
            DustType = ModContent.DustType<LabPlatingDust>();
            AnimationFrameHeight = 54;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (_activated)
                frame = 1;
            else
                frame = 0;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!RedeHelper.BossActive())
            {
                Main.tileSolid[Type] = false;
                _activated = false;
            }
            else
            {
                Main.tileSolid[Type] = true;
                _activated = true;
            }
        }
    }
    public class LabBossDoor : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Vertical Reinforced Lab Door");
            /* Tooltip.SetDefault("Closes when a boss is active" +
                "\n[c/ff0000:Unbreakable]"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<LabBossDoorTile>();
        }
    }
    public class LabBossDoorTileH : ModTile
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
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Reinforced Door");
            AddMapEntry(new Color(110, 106, 120), name);
            MinPick = 1000;
            MineResist = 10f;
            DustType = ModContent.DustType<LabPlatingDust>();
            AnimationFrameHeight = 18;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (_activated)
                frame = 1;
            else
                frame = 0;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!RedeHelper.BossActive())
            {
                Main.tileSolid[Type] = false;
                _activated = false;
            }
            else
            {
                Main.tileSolid[Type] = true;
                _activated = true;
            }
        }
    }
    public class LabBossDoorH : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Horizontal Reinforced Lab Door");
            /* Tooltip.SetDefault("Closes when any of the Lab Minibosses/Bosses are active" +
                "\n[c/ff0000:Unbreakable]"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<LabBossDoorTileH>();
        }
    }
}