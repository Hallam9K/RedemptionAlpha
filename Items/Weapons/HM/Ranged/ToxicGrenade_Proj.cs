using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Projectiles.Misc;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class ToxicGrenade_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/ToxicGrenade";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Grenade");
            ElementID.ProjPoison[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 190;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Gas1 with { Volume = 0.7f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Rectangle boom = new((int)Projectile.Center.X - 80, (int)Projectile.Center.Y - 80, 160, 160);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                    continue;

                target.immune[Projectile.whoAmI] = 20;
                int hitDirection = target.RightOfDir(Projectile);
                BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 2);
                Main.dust[dust].velocity *= 4;
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 25; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Scale: 2);
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Scale: 2);
                Main.dust[dust].velocity *= 2;
                Main.dust[dust].noGravity = true;
            }
            for (int g = 0; g < 3; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[goreIndex].velocity.X *= 3f;
            }

            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ToxicGas_Proj>(), Projectile.damage / 2, 0, Projectile.owner);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.whoAmI] = 20;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X > 4 || oldVelocity.X < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 4 || oldVelocity.Y < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity.Y *= 0.3f;
            Projectile.velocity.X *= 0.7f;
            return false;
        }
    }
}