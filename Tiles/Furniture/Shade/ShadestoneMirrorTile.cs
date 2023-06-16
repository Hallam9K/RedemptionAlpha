using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Placeable.Furniture.Shade;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneMirrorTile : ShadestoneMirrorTile2
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RedeTileHelper.CannotMineTileBelow[Type] = true;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;
    }
    public class ShadestoneMirrorTile2 : ModTile
    {
        public override string Texture => "Redemption/Tiles/Furniture/Shade/ShadestoneMirrorTile";
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            DustType = ModContent.DustType<ShadestoneDust>();

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<ShadestoneMirror>());

            // Etc
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadestone Mirror");
            AddMapEntry(new Color(85, 101, 158), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) + 2 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.COLOR_GLOWPULSE * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
    public class ShadestoneMirrorSpecial : PlaceholderTile
    {
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Shadestone Mirror");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<ShadestoneMirrorTile>();
        }
    }
}
