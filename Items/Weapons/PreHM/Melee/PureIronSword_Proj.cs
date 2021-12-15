using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;
using Redemption.Projectiles.Melee;
using Redemption.Base;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class PureIronSword_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/PureIronSword";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure-Iron Sword");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
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

                    glow += 0.02f;
                    glow = MathHelper.Clamp(glow, 0, 0.8f);
                    if (glow >= 0.8 && Projectile.localAI[0] == 0)
                    {
                        DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2, 2, 2, 1, 2, nogravity: true);
                        SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                        Projectile.localAI[0] = 1;
                    }
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        oldRotation = swordRotation;
                        directionLock = player.direction;
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        if (Projectile.localAI[0] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, Vector2.Zero, ModContent.ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                        }
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;

                    Projectile.ai[0]++;

                    float timer = Projectile.ai[0] - 1;

                    if (Projectile.localAI[0] == 1  && timer % 14 == 0)
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / (Projectile.localAI[0] == 1 ? 7f : 13f) / SwingSpeed);

                    if (Projectile.ai[0] >= (Projectile.localAI[0] == 1 ? 44 : 13) * SwingSpeed)
                        Projectile.Kill();

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile target = Main.projectile[i];
                        if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                            continue;

                        if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !ProjectileTags.Ice.Has(target.type) || ProjectileLists.IsTechnicallyMelee.Contains(target.type))
                            continue;

                        DustHelper.DrawCircle(target.Center, DustID.IceTorch, 1, 4, 4, nogravity: true);
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
                player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.localAI[0] == 1)
                damage *= 2;

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<BuffPlayer>().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.LightBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}