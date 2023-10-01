using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;
using ParticleLibrary;
using Redemption.Particles;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_Fireball : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 99, 86), new Color(204, 15, 20)), new RoundCap(), new ArrowGlowPosition(), 100f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (Projectile.wet && !Projectile.lavaWet))
                Projectile.Kill();

            Projectile.position.X = npc.position.X + (npc.width / 2) - 30;

            int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.5f);
            Main.dust[dust].noGravity = true;
            Color dustColor = new(255, 99, 86) { A = 0 };
            Main.dust[dust].color = dustColor;
            int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.5f);
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(204, 15, 20) { A = 0 };
            Main.dust[dust2].color = dustColor2;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 3);

            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(255, 99, 86) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.5f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(204, 15, 20) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            for (int i = 0; i < 12; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 3);
                Main.dust[dust2].velocity *= 5f;
                Main.dust[dust2].noGravity = true;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}