using Microsoft.Xna.Framework;
using Terraria;
using System;
using Terraria.ModLoader;
using Redemption.Base;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Globals;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PNebula1_Friendly : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/PNebula1";
        public int proType = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Piercing Nebula");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void OnSpawn(IEntitySource source)
        {
            if (proType == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula2_Friendly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula3_Friendly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }
        }
        public override void AI()
        {
            Projectile.alpha += 2;
            Projectile.localAI[0]++;
            if (proType != 0)
                ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), Color.HotPink * (Projectile.Opacity * 2f), 1f * Projectile.Opacity, .45f, Main.rand.Next(10, 20));

            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (proType != 0)
            {
                if (offsetLeft)
                {
                    vectorOffset -= 0.5f;
                    if (vectorOffset <= -1.3f)
                    {
                        vectorOffset = -1.3f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.5f;
                    if (vectorOffset >= 1.3f)
                    {
                        vectorOffset = 1.3f;
                        offsetLeft = true;
                    }
                }
                float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;

            if (proType != 0)
                return;
            Main.player[Projectile.owner].RedemptionScreen().ScreenShakeIntensity += 2;
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/GiantStar_Proj").Value;
            RedeDraw.SpawnExplosion(Projectile.Center, Main.DiscoColor * 0.6f, 6, 0, 30, 2, 1 * Projectile.Opacity, true, tex, RedeHelper.RandomRotation());
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.4f * Projectile.timeLeft / 120;
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                if (hitInfo.Damage < 40)
                    hitInfo.Damage = 40;
            };
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (proType == 0)
            {
                Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(new Color(255, 255, 255, 0)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
    }
    public class PNebula2_Friendly : PNebula1_Friendly
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 1;
            offsetLeft = false;
        }
    }
    public class PNebula3_Friendly : PNebula1_Friendly
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 2;
            offsetLeft = true;
        }
    }
}