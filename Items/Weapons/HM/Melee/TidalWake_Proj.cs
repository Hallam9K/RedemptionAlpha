using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class TidalWake_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tidal Wake");
            ElementID.ProjWater[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
            Projectile.scale = 0.1f;
            Projectile.Redemption().TechnicallyMelee = true;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 20f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 320;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 17f;
        }
        public override void AI()
        {
            if (Projectile.localAI[1]++ % 30 == 0 && Projectile.scale >= 1)
                SoundEngine.PlaySound(SoundID.Item21 with { Pitch = -0.8f }, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(10) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = dustPosition - nextDustPosition + Projectile.velocity;
                if (Main.rand.NextBool(5) && Projectile.scale >= 1)
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Water, dustVelocity, Scale: 0.5f);
                    dust.scale = distance / 30;
                    dust.scale = MathHelper.Clamp(dust.scale, 0.2f, 2);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            if (Projectile.scale >= 1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && Projectile.DistanceSQ(npc.Center) < 150 * 150 && !npc.boss && npc.knockBackResist != 0 && !npc.friendly && npc.lifeMax <= 1200)
                    {
                        float num3 = 10f;
                        float x = Projectile.Center.X - npc.Center.X;
                        float y = Projectile.Center.Y - npc.Center.Y;
                        float num6 = (float)Math.Sqrt(x * x + y * y);
                        num6 = num3 / num6;
                        x *= num6;
                        y *= num6;
                        int succPower;
                        if (Projectile.DistanceSQ(npc.Center) < 20 * 20)
                            succPower = 15;
                        else if (Projectile.DistanceSQ(npc.Center) < 50 * 50)
                            succPower = 18;
                        else if (Projectile.DistanceSQ(npc.Center) < 75 * 75)
                            succPower = 20;
                        else if (Projectile.DistanceSQ(npc.Center) < 100 * 100)
                            succPower = 30;
                        else
                            succPower = 40;
                        npc.velocity.X = (npc.velocity.X * (succPower - 1) + x) / succPower;
                        npc.velocity.Y = (npc.velocity.Y * (succPower - 1) + y) / succPower;
                        if (npc.lifeMax > 50)
                            npc.AddBuff(ModContent.BuffType<TidalWakeDebuff>(), 10);
                    }
                }
            }
            if (Projectile.alpha > 0)
                Projectile.alpha -= 4;
            if (Projectile.scale < 1)
                Projectile.scale += 0.04f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (target.life <= 0 && target.HasBuff<TidalWakeDebuff>())
            {
                int steps = (int)Projectile.Distance(player.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(Projectile.Center, player.Center, (float)i / steps), 2, 2, DustID.WaterCandle, Scale: 3);
                        dust.velocity = -player.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                SoundEngine.PlaySound(SoundID.Item3, Projectile.position);
                player.statLife += 10;
                player.HealEffect(10);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D whirl = ModContent.Request<Texture2D>("Redemption/Textures/SpinnyNoise").Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 origin2 = new(whirl.Width / 2f, whirl.Height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(whirl, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Blue) * 0.2f, Projectile.rotation * 0.75f, origin2, Projectile.scale * 0.4f, spriteEffects, 0);
            Main.EntitySpriteDraw(whirl, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.DeepSkyBlue) * 0.3f, Projectile.rotation * 0.5f, origin2, Projectile.scale * 0.3f, spriteEffects, 0);
            Main.EntitySpriteDraw(whirl, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightBlue) * 0.4f, Projectile.rotation * 0.25f, origin2, Projectile.scale * 0.2f, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, 1, spriteEffects, 0);
            return false;
        }
    }
}