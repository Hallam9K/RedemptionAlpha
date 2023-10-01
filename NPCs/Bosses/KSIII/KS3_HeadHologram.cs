using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_HeadHologram : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hologram");
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.alpha = 255;
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Slayer.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.1f };
        public int faceType;
        public override void AI()
        {
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Cyan, Color.LightCyan);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2 * (faceType + 1))
                    Projectile.frame = 2 * faceType;
            }
            Projectile.LookAtEntity(Main.player[RedeHelper.GetNearestAlivePlayer(Projectile)]);
            if (Projectile.localAI[0]++ < 30)
                Projectile.alpha -= 4;

            if (!Main.dedServ)
            {
                if (Projectile.localAI[0] == 30)
                {
                    string line1 = Main.rand.Next(4) switch
                    {
                        1 => "So stop bothering me,[0.1] I have a certain android I need to 'lecture'.",
                        2 => "So stop bothering me,[0.1] I don't care about you.",
                        3 => "So stop bothering me and we can all go about our day.",
                        _ => "So stop bothering me and leave me to my 4D chess.",
                    };
                    DialogueChain chain = new();
                    chain.Add(new(Projectile, "Hey,[0.1] get lost.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null))
                         .Add(new(Projectile, "[@f1]You really aren't worth my time,[0.1] ya know.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null))
                         .Add(new(Projectile, "[@f3]" + line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null))
                         .Add(new(Projectile, "[@f0]I'll beat you up if you annoy me again.", new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, endID: 1));
                    chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                    chain.OnEndTrigger += Chain_OnEndTrigger;
                    ChatUI.Visible = true;
                    ChatUI.Add(chain);
                }
            }
            if (Projectile.localAI[0] > 5000)
            {
                if (RedeBossDowned.slayerDeath < 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RedeBossDowned.slayerDeath = 1;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }

                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            faceType = signature switch
            {
                "f1" => 1,
                "f3" => 3,
                _ => 0,
            };
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            Projectile.localAI[0] = 5000;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 10;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}