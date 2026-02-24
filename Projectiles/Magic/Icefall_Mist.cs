using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Icefall_Mist : ModProjectile
    {
        private float noiseOffset;

        public override string Texture => "Redemption/Textures/Smoke";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Mist");
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = Main.rand.Next(180, 281);
            Projectile.scale = Main.rand.NextFloat(0.4f, 0.7f) * 3;
            Projectile.rotation = RedeHelper.RandomRotation();
            noiseOffset = Main.rand.NextFloat();
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1)
                Projectile.scale *= .75f;
        }
        public override void AI()
        {
            bool weak = Projectile.ai[0] == 1;
            Projectile.velocity *= 0.98f;
            if (Projectile.localAI[0] == 0)
            {
                if (weak && Projectile.ai[1] != 1)
                    Projectile.timeLeft += 160;
                Projectile.localAI[0] = Main.rand.Next(1, 3);
            }

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.003f;

            if (Projectile.timeLeft < 120)
            {
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 5;

                if (!weak && Main.rand.NextBool(30) && Projectile.alpha <= 100 && Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.RandAreaInEntity(), Vector2.Zero, ProjectileType<Icefall_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (Main.rand.NextBool(60) && Projectile.alpha <= 150)
                {
                    RedeParticleManager.CreateSimpleStarParticle(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, Main.rand.NextFloat(0.2f, 1f) * 0.2f, new Color(150, 180, 240, 0));
                }

                if (Projectile.alpha <= 100)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];
                        if (!target.active || !target.CanBeChasedBy())
                            continue;

                        if (!Projectile.Hitbox.Intersects(target.Hitbox))
                            continue;

                        target.AddBuff(BuffType<PureChillDebuff>(), weak ? 5 : 180);
                    }
                }
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture = Request<Texture2D>("Redemption/Textures/SoftGlow").Value;
            Rectangle rect = texture.Frame();
            Vector2 drawOrigin = rect.Size() / 2f;
            Color color = new Color(193, 253, 255) * Projectile.Opacity;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);

            Effect effect = Request<Effect>("Redemption/Effects/Mist").Value;
            Texture2D noise = Request<Texture2D>("Redemption/Textures/Noise/noise").Value;

            effect.Parameters["noiseResolution"].SetValue(new Vector2(0.5f, 0.5f));
            effect.Parameters["noiseOffset"].SetValue(new Vector2(noiseOffset));
            effect.Parameters["progress"].SetValue(0.5f + 0.25f * Projectile.Opacity);
            effect.Parameters["fadeoutFactor"].SetValue(2);

            Main.graphics.GraphicsDevice.Textures[1] = noise;
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, color, Projectile.rotation, drawOrigin, Projectile.scale * 3, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
            return false;
        }
        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }*/
    }
    public class Icefall_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Icefall");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjIce[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.1f;
            Projectile.frame = Main.rand.Next(3);
            Projectile.localAI[0] = Main.rand.Next(1, 3);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            if (Projectile.ai[0] is 1)
            {
                Projectile.rotation += Projectile.velocity.X / 20 * Projectile.direction;
                Projectile.velocity.Y += 0.2f;
                return;
            }
            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.02f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.02f;

            Projectile.scale += 0.02f;
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0, 1);
            if (Projectile.scale >= 1)
                Projectile.velocity.Y += 0.2f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                int dustIndex4 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
                Main.dust[dustIndex4].velocity *= 2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 180);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] is 1)
                modifiers.FinalDamage *= 4;
            modifiers.FinalDamage *= Projectile.scale;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.FinalDamage *= Projectile.scale;
    }
}
