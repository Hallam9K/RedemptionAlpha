using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;

namespace Redemption.NPCs.FowlMorning
{
    public class HeadlessChicken_Fire : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ghost-Fire");
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        private float speed;
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<HeadlessChicken>())
                Projectile.Kill();

            Vector2 originPos = host.Center + new Vector2(5 * host.spriteDirection, -20);

            Player player = Main.player[host.target];
            Projectile.timeLeft = 10;

            if (host.ai[2] == 1)
            {
                switch (Projectile.localAI[1])
                {
                    case 0:
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Pitch = .1f }, Projectile.position);

                        speed = 14 + (Projectile.Distance(player.Center) / 30);
                        speed = MathHelper.Min(speed, 20);
                        Projectile.velocity = RedeHelper.PolarVector(speed, (player.Center - Projectile.Center).ToRotation());
                        Projectile.localAI[1] = 1;
                        break;
                    case 1:
                        Projectile.velocity *= 0.97f;
                        if (Projectile.velocity.Length() < 5)
                        {
                            Projectile.localAI[1] = 2;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        Projectile.Move(originPos, 20, 20);
                        if (Projectile.DistanceSQ(originPos) < 50 * 50)
                        {
                            host.ai[2] = 2;
                            Projectile.netUpdate = true;
                        }
                        break;
                }
            }
            else
            {
                Projectile.localAI[1] = 0;
                Projectile.Center = originPos;
                Projectile.velocity *= 0;
            }

            int dust2 = Dust.NewDust(Projectile.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.3f);
            Main.dust[dust2].velocity.X *= 0;
            Main.dust[dust2].velocity.Y -= Main.rand.NextFloat(.4f, 1f);
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(217, 84, 155) { A = 0 };
            Main.dust[dust2].color = dustColor2;
            int dust = Dust.NewDust(Projectile.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.3f);
            Main.dust[dust].velocity *= .04f;
            Main.dust[dust2].velocity.Y -= Main.rand.NextFloat(.4f, 1f);
            Main.dust[dust].noGravity = true;
            Color dustColor = new(251, 151, 146) { A = 0 };
            Main.dust[dust].color = dustColor;

            FlareTimer += Main.rand.Next(-5, 6);
            FlareTimer = MathHelper.Clamp(FlareTimer, 10, 30);
            FlareScale += Main.rand.NextFloat(-.1f, .1f);
            FlareScale = MathHelper.Clamp(FlareScale, .9f, 1.1f);
        }
        private float FlareTimer;
        private float FlareScale;
        public override void PostDraw(Color lightColor)
        {
            RedeDraw.DrawEyeFlare(Main.spriteBatch, ref FlareTimer, Projectile.Center - Main.screenPosition, Color.IndianRed, 0, FlareScale);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.expertMode)
                target.AddBuff(BuffID.OnFire, 180);
        }
    }
}