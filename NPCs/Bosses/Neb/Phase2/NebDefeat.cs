using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class NebDefeat : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus, Angel of the Cosmos");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 62;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }
        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Neb.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice3 with { Pitch = -.4f };
        private readonly Color nebColor = new(255, 100, 174);
        private readonly Color nebColor2 = new(4, 0, 108);
        public readonly Vector2 modifier = new(0, -180);

        public float fadeAlpha = 255;
        public override void AI()
        {
            RedeSystem.Silence = true;
            if (Main.windSpeedTarget < 1)
                Main.windSpeedTarget = 1;
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 20)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            ScreenPlayer.CutsceneLock(player, Projectile, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 0);
            Projectile.localAI[0]++;
            Projectile.velocity.X = 0;
            if (RedeBossDowned.nebDeath >= 8)
            {
                if (Projectile.localAI[0] >= 120)
                    Projectile.Kill();
            }
            else
            {
                if (!Main.dedServ && Projectile.localAI[0] == 180)
                {
                    string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.1");
                    string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.2");
                    string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.3");
                    string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.4");
                    string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.5");
                    string s6 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.6");
                    string s7 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.7");
                    string s8 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.8");
                    string s9 = Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.9");
                    DialogueChain chain = new();
                    chain.Add(new(Projectile, s1, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s2, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s3, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s4, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s5, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s6, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s7, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                         .Add(new(Projectile, s8, nebColor, nebColor2, voice, .05f, 2f, 0, false, bubble: Bubble, modifier: modifier, endID: 1))
                         .Add(new(Projectile, s9, nebColor, nebColor2, voice, .15f, 2f, .5f, true, bubble: Bubble, modifier: modifier));
                    chain.OnEndTrigger += Chain_OnEndTrigger;
                    ChatUI.Visible = true;
                    ChatUI.Add(chain);
                }
                if (Projectile.localAI[0] >= 5000)
                {
                    fadeAlpha -= 2;
                    if (fadeAlpha <= 0)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            Projectile.localAI[0] = 5000;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 0.0f)
                Projectile.velocity.X = oldVelocity.X * -0.0f;
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 0.0f)
                Projectile.velocity.Y = oldVelocity.Y * -0.0f;
            return false;
        }
        private Asset<Texture2D> glowMask;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            glowMask ??= ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Phase2/NebDefeatFade");
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int num215 = texture.Height() / 4;
            int y7 = num215 * Projectile.frame;
            Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture.Width(), num215)), lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(texture.Width() / 2f, num215 / 2f), Projectile.scale, effects, 0f);
            Main.spriteBatch.Draw(glowMask.Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture.Width(), num215)), RedeColor.NebColour * ((255 - fadeAlpha) / 255f), Projectile.rotation, new Vector2(texture.Width() / 2f, num215 / 2f), Projectile.scale, effects, 0f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Enchanted_Pink, 0f, 0f, 100, default, 5f);
                Main.dust[dustIndex].velocity *= 2.9f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                RedeSystem.Silence = false;
                RedeBossDowned.nebDeath = 8;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}
