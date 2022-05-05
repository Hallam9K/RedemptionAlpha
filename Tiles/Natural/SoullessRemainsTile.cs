using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Items;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;

namespace Redemption.Tiles.Natural
{
    public class SoullessRemainsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<VoidFlame>();
            MinPick = 200;
            MineResist = 6f;
            SoundStyle = 48;
            SoundType = SoundID.NPCHit;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Soulless Remains");
            AddMapEntry(new Color(210, 200, 191), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 1;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<HintIcon>();
            }
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 1;
            if (Main.tile[left, top].TileFrameX != 0)
                return true;

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            if (Main.tile[i, j].TileFrameX == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                spriteBatch.Draw(flare, new Vector2(((i + 1.3f) * 16) - (int)Main.screenPosition.X, ((j + 0.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, Main.GlobalTimeWrappedHourly, origin, 1.3f, SpriteEffects.None, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 1.3f) * 16) - (int)Main.screenPosition.X, ((j + 0.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, -Main.GlobalTimeWrappedHourly, origin, 1.3f, SpriteEffects.None, 1f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            }
            return true;
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 1;
            if (Main.tile[left, top].TileFrameX == 0)
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<PrisonGateKey>());

            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 1; y++)
                {
                    if (Main.tile[x, y].TileFrameX < 54)
                        Main.tile[x, y].TileFrameX += 54;
                }
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 1;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_TileBreak(i, j), ModContent.ItemType<PrisonGateKey>());
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class SoullessRemains : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulless Remains");
            Tooltip.SetDefault("Gives Prison Gate Key");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SoullessRemainsTile>();
        }
    }
}
