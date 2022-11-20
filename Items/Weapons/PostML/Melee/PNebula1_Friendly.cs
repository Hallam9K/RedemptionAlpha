using Microsoft.Xna.Framework;
using Terraria;
using System;
using Terraria.ModLoader;
using Redemption.Base;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.NPCs.Bosses.Neb;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PNebula1_Friendly : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/PNebula1";
        public int proType = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        private NPC target;
        public override void OnSpawn(IEntitySource source)
        {
            if (proType != 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula2>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula3>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 30)
            {
                if (proType != 0)
                    ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), Color.Pink, 0.6f, 0, 1);

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
            if (proType == 0 && RedeHelper.ClosestNPC(ref target, 800, Projectile.Center, true))
            {
                float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
                Projectile.rotation.SlowRotation(Projectile.DirectionTo(target.Center).ToRotation() + 1.57f, (float)Math.PI / 80f);
                Projectile.velocity = RedeHelper.PolarVector(shootSpeed, Projectile.rotation + 1.57f);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
    public class PNebula2_Friendly : PNebula1_Friendly
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Nebula");
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
            DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 2;
            offsetLeft = true;
        }
    }
}