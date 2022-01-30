using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class CantripEmber : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ember");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(253, 221, 3), new Color(253, 62, 3)), new RoundCap(), new ArrowGlowPosition(), 50f, 150f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            if (Projectile.wet && !Projectile.lavaWet)
                Projectile.Kill();

            int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
            Main.dust[dust].noGravity = true;
            Color dustColor = new(253, 221, 3) { A = 0 };
            Main.dust[dust].color = dustColor;
            int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(253, 62, 3) { A = 0 };
            Main.dust[dust2].color = dustColor2;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(253, 221, 3) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(253, 62, 3) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            for (int i = 0; i < 12; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust2].velocity *= 5f;
                Main.dust[dust2].noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 160);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 160);
        }
    }
}