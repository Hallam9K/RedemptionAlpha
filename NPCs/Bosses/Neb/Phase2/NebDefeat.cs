using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.UI;
using Terraria.GameContent;
using Redemption.BaseExtension;
using Terraria.Localization;

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
            Projectile.width = 64;
            Projectile.height = 62;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }

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
            player.RedemptionScreen().ScreenFocusPosition = Projectile.Center;
            player.RedemptionScreen().lockScreen = true;
            player.RedemptionScreen().cutscene = true;
            Projectile.localAI[0]++;
            Projectile.velocity.X = 0;
            if (RedeBossDowned.nebDeath >= 8)
            {
                if (Projectile.localAI[0] >= 120)
                    Projectile.Kill();
            }
            else
            {
                if (!Main.dedServ)
                {
                    if (Projectile.localAI[0] == 180)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.1"), 150, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 330)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.2"), 150, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 480)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.3"), 200, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 680)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.4"), 140, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 820)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.5"), 200, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 1020)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.6"), 240, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 1260)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.7"), 240, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                    if (Projectile.localAI[0] == 1500)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Defeat.8"), 260, 1, 0.6f, Language.GetTextValue("Mods.Redemption.Cutscene.Nebuleus.Name"), 1, RedeColor.NebColour, null, null, Projectile.Center, 0, 0, true);
                    }
                }
                if (Projectile.localAI[0] >= 1760)
                {
                    fadeAlpha -= 5;
                    if (fadeAlpha <= 0)
                    {
                        Projectile.Kill();
                    }
                }
            }
            if (MoRDialogueUI.Visible)
            {
                RedeSystem.Instance.DialogueUIElement.PointPos = Projectile.Center;
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.NebColour;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 0.0f)
                Projectile.velocity.X = oldVelocity.X * -0.0f;
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 0.0f)
                Projectile.velocity.Y = oldVelocity.Y * -0.0f;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/Phase2/NebDefeatFade").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int num215 = texture.Height / 4;
            int y7 = num215 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture.Width, num215)), lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(texture.Width / 2f, num215 / 2f), Projectile.scale, effects, 0f);
            Main.spriteBatch.Draw(glowMask, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture.Width, num215)), RedeColor.NebColour * ((255 - fadeAlpha) / 255f), Projectile.rotation, new Vector2(texture.Width / 2f, num215 / 2f), Projectile.scale, effects, 0f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Enchanted_Pink, 0f, 0f, 100, default, 5f);
                Main.dust[dustIndex].velocity *= 2.9f;
            }
            RedeBossDowned.nebDeath = 8;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.WorldData);
            RedeSystem.Silence = false;
        }
    }
}
