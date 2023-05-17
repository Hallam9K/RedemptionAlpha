using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneMonolith2Tile : ModTile
    {
        public Asset<Texture2D> GlowTexture;

        public virtual string GlowTextureName => Texture + "_Glow";
        public override void Load()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>(GlowTextureName);
        }

        public override void Unload()
        {
            GlowTexture = null;
        }
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;

            DustType = ModContent.DustType<ShadestoneDust>();

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(1, 4);
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadestone Monolith");
            AddMapEntry(new Color(133, 135, 174), name);
        }
        private int pulseTimer;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0 && pulseTimer++ % 40 == 0)
                RedeDraw.SpawnCirclePulse(new Vector2((i + 1.5f) * 16, (j + 2) * 16), Color.GhostWhite, 0.3f);
        }
        public const int FrameWidth = 16 * 3;
        public const int FrameHeight = 16 * 4;
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            Point p = new(i, j);
            Tile tile = Framing.GetTileSafely(p.X, p.Y);
            if (tile == null || !tile.HasTile)
                return;

            Texture2D texture = GlowTexture.Value;

            Rectangle frame = texture.Frame(1, 1, 0, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -30f);

            spriteBatch.Draw(texture, drawPos, frame, color, 0, origin, 1f, effects, 0f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0, origin, 1f, effects, 0f);
            }
        }
        public override bool CanExplode(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}