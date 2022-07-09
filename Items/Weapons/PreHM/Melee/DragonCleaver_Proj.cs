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
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class DragonCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/DragonCleaver";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] >= 1 ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;
        private float glow;

        public override void AI()
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
            {
                Projectile.Kill();
            }

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                    glow += 0.01f;
                    glow = MathHelper.Clamp(glow, 0, 0.4f);
                    if (glow >= 0.4 && Projectile.localAI[0] == 0)
                    {
                        RedeDraw.SpawnRing(Projectile.Center, Color.OrangeRed, 0.2f, 0.85f, 4);
                        RedeDraw.SpawnRing(Projectile.Center, Color.OrangeRed, 0.2f);
                        SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                        Projectile.localAI[0] = 1;
                    }
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        oldRotation = swordRotation;
                        directionLock = player.direction;
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                        if (Projectile.localAI[0] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(15, (Main.MouseWorld - player.Center).ToRotation()),
                                ModContent.ProjectileType<FireSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;

                    Projectile.ai[0]++;

                    float timer = Projectile.ai[0] - 1;

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / (Projectile.localAI[0] == 1 ? 10f : 20f) / SwingSpeed);

                    if (Projectile.ai[0] >= (Projectile.localAI[0] == 1 ? 11 : 21) * SwingSpeed)
                        Projectile.Kill();

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile target = Main.projectile[i];
                        if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                            continue;

                        if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !ProjectileTags.Fire.Has(target.type) || ProjectileLists.IsTechnicallyMelee.Contains(target.type))
                            continue;

                        DustHelper.DrawCircle(target.Center, DustID.Torch, 1, 4, 4, nogravity: true);
                        target.Kill();
                    }
                }
            }

            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 40f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                damage *= 4;

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.OrangeRed * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}