using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bubble");
            ElementID.ProjWater[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ToxicBubble);
            AIType = ProjectileID.ToxicBubble;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
        }
        public override bool CanHitPlayer(Player target) => ZAPPED;
        public bool ZAPPED;
        private float teleAlpha;
        public override void AI()
        {
            if (ZAPPED)
            {
                if (Main.rand.NextBool(2))
                    DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(38, RedeHelper.RandomRotation()), 1f, 30, 0.1f);

                Projectile.velocity *= 0.5f;
                Projectile.velocity += new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2));
                teleAlpha += 0.016f;
                if (teleAlpha >= 1)
                {
                    Projectile.Kill();
                }
                Projectile.timeLeft = 2;
                return;
            }
            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (proj.active && Projectile != proj && !ZAPPED)
                {
                    if (Main.rand.NextBool(5) && proj.type == ModContent.ProjectileType<UkkoElectricBlast2>())
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                            dust.noGravity = true;
                            dust.velocity = -Projectile.DirectionTo(dust.position);
                        }
                        ZAPPED = true;
                    }
                }
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (target.active && !target.dead && Projectile.Hitbox.Intersects(target.Hitbox))
                    target.AddBuff(BuffID.Wet, 600);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!ZAPPED)
                return;

            Projectile.hostile = true;
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Zap2, Projectile.position);
            RedeHelper.PlayerRadiusDamage(200, Projectile, NPCHelper.HostileProjDamageInc(Projectile.damage), Projectile.knockBack);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Scale: 2);
                Main.dust[dustIndex].velocity *= 4f;
            }
            DustHelper.DrawCircle(Projectile.Center, DustID.Electric, 6, 4, 3, 0.5f, dustSize: 2, nogravity: true);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D telegraph = ModContent.Request<Texture2D>("Redemption/Textures/RadialTelegraph3").Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(telegraph.Width / 2f, telegraph.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(telegraph, position, null, Color.LightBlue * teleAlpha, Projectile.rotation, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return true;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
}