using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class MicroshieldDrone : ModProjectile
    {
        Player Owner => Main.player[Projectile.owner];

        public float speed;

        public bool targetExist;

        public float distance = 1000;

        public const int RESTORE_TIME = 1800;

        public const int MAX_ENDURANCE = 500;

        public Vector2 moveTo;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Microshield Drone");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Default;
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.frame = 3;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            if (!Owner.GetModPlayer<MicroshieldCore_Player>().shieldDisabled)
            {
                Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, 0f, 0f);
            }
            if (!CheckActive(Owner))
                return;

            Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter);
            if (!targetExist)
            {
                Vector2 playerDir = Projectile.Center - rrp;
                LookInDirection(-playerDir);
                Projectile.Move(rrp + RedeHelper.PolarVector(50, Main.GameUpdateCount * 0.01f), 30, 2);
                distance = 1000;
            }
            float dist = distance;
            targetExist = false;
            if (!Owner.GetModPlayer<MicroshieldCore_Player>().shieldDisabled)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    var proj = Main.projectile[i];
                    if (!proj.active || proj.type == Type || proj.damage <= 0 || proj.velocity == Vector2.Zero || !ProjReflect.FriendlyReflectCheck(Projectile, proj, 10000) || ProjReflect.ProjBlockBlacklist(proj))
                        continue;

                    if (proj.Distance(rrp) > dist)
                        continue;

                    targetExist = true;
                    dist = proj.Distance(rrp);
                    if (Projectile.localAI[1] % 30 == 0)
                        moveTo = rrp.DirectionTo(proj.Center) * 60;

                    Projectile.Move(rrp + moveTo, 30, 2);
                    LookInDirection(-moveTo);

                    if (proj.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, (Projectile.Center - Owner.Center).ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f)), ProjectileType<GirusDischarge>(), 180, 3, Main.myPlayer);
                            }
                        }
                        if (proj.damage < MAX_ENDURANCE - Owner.GetModPlayer<MicroshieldCore_Player>().damageEndured)
                        {
                            ProjReflect.FriendlyReflectEffect(proj, false);
                        }
                        Owner.GetModPlayer<MicroshieldCore_Player>().damageEndured += proj.damage * 0.75f;
                        if (Owner.GetModPlayer<MicroshieldCore_Player>().damageEndured >= MAX_ENDURANCE)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath56, Projectile.position);
                            for (int j = 0; j < 7; j++)
                            {
                                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.5f);
                                Main.dust[dustIndex].velocity *= 1.9f;
                            }
                            Owner.GetModPlayer<MicroshieldCore_Player>().damageEndured = 0;
                            Owner.GetModPlayer<MicroshieldCore_Player>().shieldDisabled = true;
                        }
                        else
                        {
                            CombatText.NewText(Projectile.getRect(), Color.MediumVioletRed, (int)(MAX_ENDURANCE - Owner.GetModPlayer<MicroshieldCore_Player>().damageEndured), true, true);
                            SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                        }
                    }
                }
            }
            else
            {
                if (++Owner.GetModPlayer<MicroshieldCore_Player>().restoreTimer >= RESTORE_TIME)
                {
                    Owner.GetModPlayer<MicroshieldCore_Player>().shieldDisabled = false;
                    Owner.GetModPlayer<MicroshieldCore_Player>().restoreTimer = 0;
                }
            }
            if (Main.myPlayer == Owner.whoAmI && Projectile.DistanceSQ(Owner.Center) > 2000 * 2000)
            {
                Projectile.position = Owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            Projectile.localAI[1]++;
        }
        public static float c = 1f / 255f;
        public Color innerColor = new(150 * c * 0.5f, 20 * c * 0.5f, 54 * c * 0.5f, 1f);
        public Color borderColor = new(215 * c, 79 * c, 214 * c, 1f);
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            Texture2D texture = Request<Texture2D>("Redemption/Textures/PlainCircle").Value;

            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);

            Effect ShieldEffect = Request<Effect>("Redemption/Effects/Shield2").Value;
            Texture2D HexagonTexture = Request<Texture2D>("Redemption/Textures/Hexagons").Value;

            ShieldEffect.Parameters["offset"].SetValue(new Vector2(0.5f, 0f));
            ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
            ShieldEffect.Parameters["time"].SetValue(0);
            ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, .5f).ToVector4());
            ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, .5f).ToVector4());
            ShieldEffect.Parameters["sinMult"].SetValue(1);
            ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(2, 2));
            ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (texture.Width / 2), 1f / (texture.Height / 2)));
            ShieldEffect.Parameters["frameAmount"].SetValue(1f);
            ShieldEffect.Parameters["direction"].SetValue((-Projectile.rotation + 1.57f).ToRotationVector2());
            ShieldEffect.Parameters["visibleRatio"].SetValue(0.4f);

            Main.spriteBatch.BeginAdditive(true);
            ShieldEffect.CurrentTechnique.Passes[0].Apply();

            Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 pos = Projectile.Center + rrp.DirectionTo(Projectile.Center) * -20;

            if (!Owner.GetModPlayer<MicroshieldCore_Player>().shieldDisabled)
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, rect, Color.Red * 0.2f, 0, origin, 0.5f, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return true;
        }
        private void LookInDirection(Vector2 look)
        {
            float angle = 0.5f * (float)Math.PI;
            if (look.X != 0f)
            {
                angle = (float)Math.Atan(look.Y / look.X);
            }
            else if (look.Y < 0f)
            {
                angle += (float)Math.PI;
            }
            if (look.X < 0f)
            {
                angle += (float)Math.PI;
            }
            Projectile.rotation = angle - (float)Math.PI / 2;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.GetModPlayer<MicroshieldCore_Player>().microshieldDrone)
                Projectile.timeLeft = 2;
            return !owner.dead && owner.active;
        }
    }
}
