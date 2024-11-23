using Microsoft.Xna.Framework;
using ParticleLibrary.Core;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class OOFingergun_Laser : OO_Laser
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OO_Laser";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Laser");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleSystem.NewParticle(Projectile.Center + Projectile.velocity, Projectile.velocity, new SlashParticleAlt(10, 1), Color.IndianRed, 2f, layer: Layer.BeforePlayers);
            for (int i = 0; i < 3; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(Main.player[Projectile.owner].Center);
                Vector2 position = target.Center - direction * 10;
                ParticleSystem.NewParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, new SpeedParticle(), Color.IndianRed, 1f);
            }
        }
    }
    public class OOFingergun_Fingerflash : OO_Fingerflash
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OO_Fingerflash";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fingerflash");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile gun = Main.projectile[(int)Projectile.ai[0]];
            float shootSpeed = player.inventory[player.selectedItem].shootSpeed;
            if (!gun.active || gun.type != ModContent.ProjectileType<OOFingergun_Proj>())
                Projectile.Kill();

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 6 && Projectile.owner == Main.myPlayer)
                {
                    if (Main.projectile[gun.whoAmI].ModProjectile is OOFingergun_Proj fingergun)
                    {
                        fingergun.offset = 10;
                        fingergun.rotOffset = -0.4f;
                    }

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Laser1 with { Pitch = 0.1f, Volume = 0.4f }, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, gun.velocity * shootSpeed, ModContent.ProjectileType<OOFingergun_Laser>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                }
                if (++Projectile.frame >= 9)
                    Projectile.Kill();
            }
            Projectile.rotation = gun.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 gunPos = gun.Center + RedeHelper.PolarVector(-62, Projectile.rotation + MathHelper.PiOver2);
            Projectile.Center = gunPos;
            return false;
        }
    }
}