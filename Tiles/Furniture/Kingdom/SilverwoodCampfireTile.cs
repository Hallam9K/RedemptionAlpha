using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Furniture.Kingdom;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Kingdom
{
    public class SilverwoodCampfireTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Silverwood Campfire");
            AddMapEntry(new Color(254, 121, 2), name);
            DustType = DustID.OrangeTorch;
            AnimationFrameHeight = 36;
            MinPick = 10;
            MineResist = 1;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            player.AddBuff(BuffID.Campfire, 10);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.rand.NextBool(2))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 10, j * 16 - 8), 28, 24, DustID.Smoke, Alpha: 20, Scale: 1f);
                    Main.dust[d].velocity.Y = -2f;
                    Main.dust[d].velocity.X *= 0.5f;
                    Main.dust[d].fadeIn = 300;
                    Main.dust[d].noGravity = true;
                }
            }
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 7)
                    frame = 0;
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.6f;
            b = 0.1f;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int height = tile.TileFrameY % AnimationFrameHeight >= 16 ? 18 : 16;
            int animate = Main.tileFrame[Type] * AnimationFrameHeight;

            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Tiles/Furniture/Kingdom/SilverwoodCampfireTile_Glow").Value;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            Color color = new(100, 100, 100, 0);
            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
                Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + xx, j * 16 - (int)Main.screenPosition.Y + yy) + zero;
                spriteBatch.Draw(texture, drawPosition, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<SilverwoodCampfire>());
        }
    }
}