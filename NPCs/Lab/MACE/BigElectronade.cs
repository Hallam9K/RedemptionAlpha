using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Particles;

namespace Redemption.NPCs.Lab.MACE
{
    public class BigElectronade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 100;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 15)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 2;
            }
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.15f;
            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;
        }
        public override bool CanHitPlayer(Player target) => false;
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ElectricNoise, Projectile.position);

            DustHelper.DrawCircle(Projectile.Center, DustID.Electric, 3, 3, 3, 1, 1, nogravity: true);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity *= 5;
                Main.dust[dust].noGravity = true;
            }
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BigElectronade_TeslaField>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X > 4 || oldVelocity.X < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 4 || oldVelocity.Y < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity.Y *= 0.7f;
            Projectile.velocity.X *= 0.7f;
            return false;
        }
    }
    public class BigElectronade_TeslaField : ModProjectile
    {
        public override string Texture => "Redemption/Textures/StaticBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tesla Field");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.01f;

            if (Projectile.timeLeft > 30 && Main.rand.NextBool(10))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90, RedeHelper.RandomRotation()), 1, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90, RedeHelper.RandomRotation()), 1, 20, 0.1f);
            }

            if (Projectile.timeLeft <= 60)
                Projectile.alpha += 5;
        }
        public override bool CanHitPlayer(Player target) => Projectile.alpha < 100;
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 360);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 1.3f, 1f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.3f, 1f, 1.3f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Orange, Color.LightCyan);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), -Projectile.rotation, drawOrigin, Projectile.scale * scale2, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}