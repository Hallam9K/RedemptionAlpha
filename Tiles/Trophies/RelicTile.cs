using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System;
using ReLogic.Content;
using Terraria.Localization;
using Redemption.Items.Placeable.Trophies;
using System.Collections.Generic;

namespace Redemption.Tiles.Trophies
{
    public class RelicTile : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 15;

        public Asset<Texture2D> RelicTexture;

        public virtual string RelicTextureName => "Redemption/Tiles/Trophies/RelicTile";
        public override string Texture => "Redemption/Textures/RelicPedestal";

        public override void Load()
        {
            if (!Main.dedServ)
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
        }

        public override void Unload()
        {
            RelicTexture = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;

            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Tile t = Main.tile[i, j];
            int placeStyle = t.TileFrameX / FrameWidth;

            int itemType = 0;
            switch (placeStyle)
            {
                case 0:
                    itemType = ModContent.ItemType<ErhanRelic>();
                    break;
                case 1:
                    itemType = ModContent.ItemType<KS3Relic>();
                    break;
                case 2:
                    itemType = ModContent.ItemType<SoIRelic>();
                    break;
                case 3:
                    itemType = ModContent.ItemType<ThornRelic>();
                    break;
                case 4:
                    itemType = ModContent.ItemType<KeeperRelic>();
                    break;
                case 5:
                    itemType = ModContent.ItemType<CleaverRelic>();
                    break;
                case 6:
                    itemType = ModContent.ItemType<GigaporaRelic>();
                    break;
                case 7:
                    itemType = ModContent.ItemType<OORelic>();
                    break;
                case 8:
                    itemType = ModContent.ItemType<PZRelic>();
                    break;
                case 9:
                    itemType = ModContent.ItemType<AkkaRelic>();
                    break;
                case 10:
                    itemType = ModContent.ItemType<UkkoRelic>();
                    break;
                case 11:
                    itemType = ModContent.ItemType<NebRelic>();
                    break;
                case 12:
                    itemType = ModContent.ItemType<FowlEmperorRelic>();
                    break;
                case 13:
                    itemType = ModContent.ItemType<CockatriceRelic>();
                    break;
                case 14:
                    itemType = ModContent.ItemType<BasanRelic>();
                    break;
            }

            if (itemType > 0)
                yield return new Item(itemType);
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
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

            Texture2D texture = RelicTexture.Value;

            int frameY = tile.TileFrameX / FrameWidth;
            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
            }
        }
    }
}
