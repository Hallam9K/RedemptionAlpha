using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class ChickendEgg_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chickend Egg");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
            if (!Main.rand.NextBool(10))
                return;
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Scale: .4f);
            Main.dust[dust2].velocity *= 0;
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(217, 84, 155) { A = 0 };
            Main.dust[dust2].color = dustColor2;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Scale: .6f);
            Main.dust[dust].velocity *= .1f;
            Main.dust[dust].noGravity = true;
            Color dustColor = new(251, 151, 146) { A = 0 };
            Main.dust[dust].color = dustColor;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath11 with { Volume = .5f }, Projectile.position);
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MothronEgg, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f, Scale: 2);
            for (int i = 0; i < 6; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>());
                Main.dust[dust2].velocity *= 0;
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(217, 84, 155) { A = 0 };
                Main.dust[dust2].color = dustColor2;
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Scale: 1.2f);
                Main.dust[dust].velocity *= .1f;
                Main.dust[dust].noGravity = true;
                Color dustColor = new(251, 151, 146) { A = 0 };
                Main.dust[dust].color = dustColor;
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Chick_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}