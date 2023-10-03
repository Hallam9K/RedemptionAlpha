using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Buffs.NPCBuffs;
using Terraria.Graphics.Shaders;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Particles;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class EaglecrestJavelin_Proj : ModProjectile
    {
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Javelin");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] >= 1 ? null : false;
        public override bool? CanCutTiles() => Projectile.ai[0] >= 1 ? null : false;
        private float glow;
        private int thunderCooldown;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
                    Projectile.Center = new Vector2(playerCenter.X, playerCenter.Y - 16);
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 20;
                    player.itemAnimation = 20;
                    Projectile.spriteDirection = player.direction;
                    Projectile.rotation = MathHelper.PiOver2 * player.direction;

                    glow += 0.02f;
                    glow = MathHelper.Clamp(glow, 0, 0.4f);
                    if (glow >= 0.4 && Projectile.localAI[0] == 0)
                    {
                        DustHelper.DrawCircle(Projectile.Center, DustID.Sandnado, 2, 2, 2, 1, 2, nogravity: true);
                        SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                        Projectile.localAI[0] = 1;
                    }
                    if (!player.channel)
                    {
                        if (Projectile.localAI[0] == 1)
                        {
                            Projectile.ai[0] = 1;
                            SoundEngine.PlaySound(SoundID.Item19, Projectile.position);
                            Projectile.velocity = RedeHelper.PolarVector(22, (Main.MouseWorld - player.Center).ToRotation());
                        }
                        else
                        {
                            player.itemTime = 2;
                            player.itemAnimation = 2;
                            Projectile.Kill();
                        }
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    Projectile.tileCollide = true;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.velocity.Y += 0.2f;
                }
            }

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else if (Projectile.ai[0] < 31)
                player.itemRotation -= MathHelper.ToRadians(-20f * player.direction);

            thunderCooldown--;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] >= 1)
                StrikeLightning();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 30;
            target.immune[Projectile.owner] = 0;

            if (thunderCooldown <= 0)
            {
                StrikeLightning();
                thunderCooldown = 10;
            }
        }
        private void StrikeLightning()
        {
            Player player = Main.player[Projectile.owner];
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
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 400), new Vector2(0, 5), ModContent.ProjectileType<EaglecrestJavelin_Thunder>(), (int)(Projectile.damage * .75f), 8, Projectile.owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition - Vector2.UnitY * Projectile.gfxOffY;
                Color color = new Color(255, 180, 0) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(8, 8), null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class EaglecrestJavelin_Thunder : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 80;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 30;
            target.immune[Projectile.owner] = 0;
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 30);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Projectile.RightOfDir(target);
        }
    }
}