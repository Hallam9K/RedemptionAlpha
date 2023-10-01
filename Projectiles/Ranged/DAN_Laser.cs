using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class DAN_Laser : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("D.A.N Laser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 700;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.Redemption().EnergyBased = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ > 10)
            {
                if (Projectile.ai[1] == 1)
                {
                    ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new LightningParticle(), Color.White, 3, 2);
                    ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new LightningParticle(), Color.White, 2, 2);
                    return;
                }
                ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new LightningParticle(), Color.White, 3, 3);
                ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new LightningParticle(), Color.White, 2, 4);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            player.RedemptionScreen().ScreenShakeIntensity += 18;
            for (int k = 0; k < 3; k++)
            {
                Projectile.localAI[1] += Main.rand.Next(5, 14);
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 origin = Projectile.Center;
                    origin.X += Projectile.localAI[1] * 16 * i;
                    int numtries = 0;
                    int x = (int)(origin.X / 16);
                    int y = (int)(origin.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        origin.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        origin.Y = y * 16;
                    }
                    if (numtries >= 20)
                        break;

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), origin, new Vector2(0, -6), ProjectileID.GeyserTrap, Projectile.damage / 6, Projectile.knockBack, player.whoAmI);
                    Main.projectile[p].hostile = false;
                    Main.projectile[p].friendly = true;
                    Main.projectile[p].netUpdate = true;
                }
            }
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == 1)
            {
                RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed, glowScale: 6);
                RedeDraw.SpawnRing(Projectile.Center, Color.Red, glowScale: 5);
            }
            else
            {
                RedeDraw.SpawnRing(Projectile.Center, new Color(158, 57, 248), glowScale: 6);
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 182, 49), glowScale: 5);
            }
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast, Projectile.position);
        }
    }
}
