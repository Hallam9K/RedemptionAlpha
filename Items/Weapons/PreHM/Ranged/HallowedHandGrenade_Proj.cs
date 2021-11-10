using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.NPCs.Critters;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class HallowedHandGrenade_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ranged/HallowedHandGrenade";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallowed Hand Grenade of Anglon");
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 190;
        }

        private float TeleGlowTimer;
        private bool TeleGlow;
        public override void AI()
        {
            if (TeleGlow)
            {
                TeleGlowTimer += 3;
                if (TeleGlowTimer > 60)
                {
                    TeleGlow = false;
                    TeleGlowTimer = 0;
                }
            }

            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            if (Projectile.localAI[0] < 180)
                Projectile.velocity.Y += 0.2f;

            if (Projectile.localAI[0]++ == 180)
            {
                Projectile.velocity *= 0;
                TeleGlow = true;
                Projectile.alpha = 255;
                if (Projectile.DistanceSQ(Main.player[Main.myPlayer].Center) < 800 * 800)
                    Main.player[Main.myPlayer].GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 10;
                SoundEngine.PlaySound(SoundID.Item14);
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 15;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 20;
                    Main.dust[dust].noGravity = true;
                }
                for (int g = 0; g < 6; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].scale = 1.5f;
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
                Rectangle boom = new((int)Projectile.Center.X - 150, (int)Projectile.Center.Y - 150, 300, 300);
                foreach (NPC target in Main.npc.Take(Main.maxNPCs))
                {
                    if (!target.active || target.friendly)
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = Projectile.Center.X > target.Center.X ? -1 : 1;
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
                }
            }
            if (Projectile.localAI[0] == 182)
            {
                Projectile.friendly = false;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.localAI[0] < 180)
                Projectile.localAI[0] = 180;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 6);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(Color.White, Color.Orange, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, 4f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Projectile.rotation, origin2, 4f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.5f;
            Projectile.velocity.X *= 0.8f;
            return false;
        }
    }
}