using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class EaglecrestBoulder_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Boulder");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Boulder);
            AIType = ProjectileID.Boulder;
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 8;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                RedeDraw.SpawnExplosion(Projectile.Center, new Color(255, 255, 174), shakeAmount: 0, scale: 1, noDust: true);
                Projectile.localAI[0] = 1;
            }
            glowOpacity -= 0.02f;
            Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -30, 30);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 42;
            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.knockBackResist != 0)
                target.velocity += Projectile.velocity * target.knockBackResist;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Length() > 10 && (oldVelocity.Y > 4 || oldVelocity.Y < -4))
                StrikeLightning();
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.penetrate--;
                if (oldVelocity.X > 2 || oldVelocity.X < -2)
                {
                    Projectile.velocity.Y -= oldVelocity.Y * 0.1f;
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
                    Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
                }
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 2 || oldVelocity.Y < -2)
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
                    Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
                }
                Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
            }
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            return false;
        }
        private void StrikeLightning()
        {
            Player player = Main.player[Projectile.owner];
            glowOpacity = 1;
            RedeDraw.SpawnExplosion(Projectile.Center, new Color(255, 255, 174), shakeAmount: 0, scale: 1, noDust: true);
            if (Projectile.DistanceSQ(player.Center) < 800 * 800)
                player.RedemptionScreen().ScreenShakeIntensity += 15;

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                    -Projectile.velocity.X * 0.01f, -Projectile.velocity.Y * 0.6f, Scale: 2);
            for (int i = 0; i < 3; i++)
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center - new Vector2(0, 400), Projectile.Center, 2f, 30, 0.1f, 1);
            DustHelper.DrawCircle(Projectile.Center - new Vector2(0, 400), DustID.Sandnado, 1, 4, 4, 1, 3, nogravity: true);

            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Thunderstrike, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 400), new Vector2(0, 5), ModContent.ProjectileType<EaglecrestJavelin_Thunder>(), (int)(Projectile.damage * .75f), 8, Projectile.owner);
                Main.projectile[proj].DamageType = DamageClass.Magic;
                Main.projectile[proj].netUpdate = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.position);
            if (!Main.dedServ)
            {
                for (int i = 0; i < 8; i++)
                    Gore.NewGore(Projectile.GetSource_FromThis(), RedeHelper.RandAreaInEntity(Projectile), Projectile.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore5").Type, 1);
            }
            for (int i = 0; i < 15; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
        }
        private float glowOpacity = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * .5f, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            if (glowOpacity > 0)
            {
                int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(new Color(255, 255, 174)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * .5f * glowOpacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
                }
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 255, 174)) * glowOpacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }
    }
}