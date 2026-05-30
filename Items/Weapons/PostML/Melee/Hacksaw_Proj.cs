using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Hacksaw_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Automated Hacksaw");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[1] >= 1 ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        private float speedBonus;
        private int spinFrame;
        private float glowAmount;
        private float damageIncrease;
        public int pauseTimer;
        public override void AI()
        {
            if (Projectile.frameCounter++ % 5 == 0)
            {
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
            if (Projectile.frameCounter % 3 == 0)
            {
                if (++spinFrame > 3)
                    spinFrame = 0;
            }
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            speedBonus = SetSpeedBonus(10, player.HeldItem.useTime);

            Vector2 armCenter = player.RotatedRelativePoint(player.MountedCenter);
            float swordRotation = 0f;
            switch (Projectile.ai[0])
            {
                case 0:
                    if (Projectile.ai[1] == 0)
                    {
                        swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                        Projectile.ai[1] = 1;
                        oldRotation = swordRotation;
                        directionLock = player.direction;
                    }
                    else if (Projectile.ai[1] >= 1)
                    {
                        player.direction = directionLock;
                        if (--pauseTimer <= 0)
                            Projectile.ai[1]++;
                        float timer = Projectile.ai[1] - 1;
                        if (timer % 20 == 0)
                            SoundEngine.PlaySound(SoundID.Item22, Projectile.position);

                        swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / 5f * speedBonus);

                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(armCenter + Projectile.velocity * 80 + new Vector2(0, 40), DustType<DustSpark>(), Projectile.velocity.RotatedBy((MathHelper.PiOver2 + Main.rand.NextFloat(-.1f, .1f)) * player.direction) * Main.rand.NextFloat(4, 6), 0, new Color(0, 251, 130) * 0.8f, 1.6f);
                            Dust.NewDustPerfect(armCenter + -Projectile.velocity * 80 + new Vector2(0, 40), DustType<DustSpark>(), Projectile.velocity.RotatedBy((-MathHelper.PiOver2 + Main.rand.NextFloat(-.1f, .1f)) * player.direction) * Main.rand.NextFloat(4, 6), 0, new Color(0, 251, 130) * 0.8f, 1.6f);
                        }
                        if (Projectile.ai[1] >= 14f / speedBonus && !player.channel)
                            Projectile.Kill();
                    }
                    break;
                case 1:
                    Projectile.idStaticNPCHitCooldown = 6;

                    if (Projectile.ai[1]++ % (30 - (int)(Projectile.ai[1] / 4)) == 0)
                        SoundEngine.PlaySound(SoundID.Item22, Projectile.position);

                    if (Projectile.ai[1] >= 30)
                    {
                        int d1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0, 0);
                        Main.dust[d1].position -= Projectile.velocity * 7;
                        Main.dust[d1].velocity = Projectile.velocity * 3;
                        Main.dust[d1].noGravity = true;
                    }

                    player.ChangeDir(Projectile.direction);
                    if (Projectile.owner == Main.myPlayer)
                        swordRotation = (Main.MouseWorld - armCenter).ToRotation();

                    glowAmount += 0.01f * speedBonus;
                    if (!player.channel)
                        Projectile.Kill();
                    if (glowAmount >= 0.9f)
                    {
                        player.RedemptionScreen().ScreenShakeIntensity += 10;
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                        for (int i = 0; i < 15; i++)
                        {
                            int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0, 0, Scale: 3);
                            Main.dust[d2].velocity = Projectile.velocity * 9;
                            Main.dust[d2].noGravity = true;
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 3);
                            Main.dust[d2].velocity = Projectile.velocity * 12;
                            Main.dust[d2].noGravity = true;
                        }

                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 25, ProjectileType<Hacksaw_Heat_Proj>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner);

                        Projectile.Kill();
                    }
                    break;
                case 2:
                    Projectile.idStaticNPCHitCooldown = 6;
                    glowAmount = MathHelper.Lerp(0, 0.7f, damageIncrease / 4);
                    if (Projectile.ai[1]++ % 30 == 0)
                        SoundEngine.PlaySound(SoundID.Item22, Projectile.position);

                    player.ChangeDir(Projectile.direction);
                    if (Projectile.owner == Main.myPlayer)
                        swordRotation = (Main.MouseWorld - armCenter).ToRotation();

                    if (Projectile.localAI[0]-- <= 0 && damageIncrease > 0)
                        damageIncrease -= 0.02f;

                    if (!player.channel)
                        Projectile.Kill();
                    break;
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            Projectile.Center = armCenter + Projectile.velocity * 36f;

            Vector2 rand = Projectile.position + new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2));
            Projectile.position = rand;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center = player.MountedCenter;
            float point = 0f;
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, center + center.DirectionTo(Main.MouseWorld) * 90, 20 * Projectile.scale, ref point))
                    return null;
                else
                    return false;
            }
            return null;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 armCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item23 with { Pitch = 1f, Volume = 2f }, Projectile.position);
                player.RedemptionScreen().ScreenShakeIntensity += 0.5f;
                pauseTimer = 1;
                Vector2 directionTo = target.DirectionTo(armCenter);
                for (int i = 0; i < 5; i++)
                    Dust.NewDustPerfect(target.Center + directionTo * 10 + new Vector2(0, 35), DustType<DustSpark>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f) + 3.14f) * -Main.rand.NextFloat(0.5f, 5f), 0, new Color(255, 230, 60) * 0.8f, 1.6f);
            }
            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item23 with { Pitch = 1f, Volume = 2f }, Projectile.position);
                player.RedemptionScreen().ScreenShakeIntensity += 1;
                Vector2 directionTo = target.DirectionTo(armCenter);
                for (int i = 0; i < 5; i++)
                    Dust.NewDustPerfect(target.Center + directionTo * 10 + new Vector2(0, 35), DustType<DustSpark>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f) + 3.14f) * -Main.rand.NextFloat(0.5f, 5f), 0, new Color(255, 230, 60) * 0.8f, 1.6f);
            }
            if (Projectile.ai[0] == 2)
            {
                SoundEngine.PlaySound(SoundID.Item23 with { Pitch = 1f, Volume = 2f }, Projectile.position);
                player.RedemptionScreen().ScreenShakeIntensity += 1;
                Vector2 directionTo = target.DirectionTo(armCenter);
                for (int i = 0; i < 5; i++)
                    Dust.NewDustPerfect(target.Center + directionTo * 10 + new Vector2(0, 35), DustType<DustSpark>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f) + 3.14f) * -Main.rand.NextFloat(0.5f, 5f), 0, new Color(255, 230, 60) * 0.8f, 1.6f);

                pauseTimer = 2;
                Projectile.localAI[0] = 20;
                damageIncrease += 0.04f * speedBonus;
                damageIncrease = MathHelper.Clamp(damageIncrease, 0, 4);
                modifiers.FinalDamage *= damageIncrease + 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects spriteEffects2 = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D spinTex = Request<Texture2D>(Texture + "_Effect").Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            int spinHeight = spinTex.Height / 4;
            int spinY = spinHeight * spinFrame;
            Rectangle spinRect = new(0, spinY, spinTex.Width, spinHeight);
            Vector2 spinOrigin = new(spinTex.Width / 2, spinHeight / 2);
            Vector2 armCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0, 29);
                Color color = Color.LightBlue with { A = 0 } * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color * 0.5f, oldrot[k], drawOrigin, Projectile.scale, spriteEffects, 0);
            }
            if (Projectile.ai[0] == 0)
                Main.EntitySpriteDraw(spinTex, armCenter - Main.screenPosition, new Rectangle?(spinRect), Projectile.GetAlpha(Color.White with { A = 0 }) * 0.5f, Projectile.rotation, spinOrigin, Projectile.scale + 0.4f, spriteEffects2, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.Orange) * glowAmount, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
            return false;
        }
    }
}