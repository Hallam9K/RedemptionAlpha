using Microsoft.Xna.Framework;
using Redemption.Items.Armor.Vanity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.Items.Usable;
using Terraria.Audio;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;

namespace Redemption.Tiles.Furniture.Misc
{
    public class HangingTiedTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = DustID.Bone;
            HitSound = CustomSounds.BoneHit;
            RegisterItemDrop(ModContent.ItemType<OldTophat>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Hanging Tied");
            AddMapEntry(new Color(81, 81, 81), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
            {
                for (int n = 0; n < 25; n++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(i * 16, j * 16), 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                    Main.dust[dustIndex].velocity *= 2f;
                    Main.dust[dustIndex].noGravity = true;
                }
                DustHelper.DrawDustImage(new Vector2(i * 16, j * 16), DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);

                SoundEngine.PlaySound(SoundID.Item74, new Vector2(i * 16, j * 16));
                WorldGen.KillTile(i, j);
            }
            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                return true;

            Texture2D flare = Redemption.WhiteFlare.Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                spriteBatch.Draw(flare, new Vector2(((i + 1.5f) * 16) - (int)Main.screenPosition.X, ((j + 1.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, Main.GlobalTimeWrappedHourly, origin, 1.5f, 0, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 1.5f) * 16) - (int)Main.screenPosition.X, ((j + 1.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, -Main.GlobalTimeWrappedHourly, origin, 1.5f, 0, 1f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            }
            return true;
        }
    }
    public class HangingTied : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Hanging Tied");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<HangingTiedTile>();
        }
    }
}