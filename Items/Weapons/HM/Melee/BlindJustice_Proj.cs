using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BlindJustice_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/BlindJustice";
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blind Justice, Demon's Terror");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 76;
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
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                    glow += 0.03f;
                    glow = MathHelper.Clamp(glow, 0, 0.8f);
                    if (glow >= 0.8 && Projectile.localAI[0] == 0)
                    {
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f, 0.85f, 4);
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.NebSound2 with { Pitch = 0.1f }, player.position);
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
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.NebSound1 with { Pitch = 0.1f }, player.position);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<BlindJustice_Aura>(), Projectile.damage, 0, Projectile.owner);
                        }
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    Projectile.ai[0]++;
                    float timer = Projectile.ai[0] - 1;
                    if (Projectile.localAI[0] == 1 && timer % 14 == 0)
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / (Projectile.localAI[0] == 1 ? 6f : 17f) / SwingSpeed);

                    if (Projectile.ai[0] >= (Projectile.localAI[0] == 1 ? 48 : 17) * SwingSpeed)
                        Projectile.Kill();

                    if (Projectile.localAI[0] == 1)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile target = Main.projectile[i];
                            if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 200)
                                continue;

                            if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !ProjectileTags.Shadow.Has(target.type) || target.Redemption().TechnicallyMelee)
                                continue;

                            DustHelper.DrawCircle(target.Center, DustID.GoldFlame, 1, 4, 4, nogravity: true);
                            target.Kill();
                        }
                    }
                }
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 60f;

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
            if (Projectile.localAI[0] == 1)
                damage *= 2;
            if (NPCLists.Demon.Contains(target.type))
                damage *= 2;

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, Color.LightBlue * Projectile.Opacity * glow, Projectile.rotation, origin, Projectile.scale, spriteEffects);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
