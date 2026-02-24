using Redemption.Globals;
using Redemption.NPCs.Lab.MACE;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public abstract class BaseTerraBombaProj : ModProjectile
    {
        protected abstract int ConversionID { get; }
        public override void SetStaticDefaults()
        {
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 3;
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
                Projectile.width = 500;
                Projectile.height = 500;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.damage = 550;
                Projectile.knockBack = 15f;
            }
            else
            {
                if (Main.rand.NextBool(2))
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                    Main.dust[dustIndex].scale = 0.1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[dustIndex].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].position = Projectile.Center + new Vector2(0f, -(float)Projectile.height / 2).RotatedBy(Projectile.rotation, default) * 1.1f;
                    dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    Main.dust[dustIndex].scale = 1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].position = Projectile.Center + new Vector2(0f, -(float)Projectile.height / 2 - 6).RotatedBy(Projectile.rotation, default) * 1.1f;
                }
            }
            Projectile.velocity.Y += 0.2f;
        }
        public override void OnKill(int timeLeft)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                int radius = 100;
                Vector2 Center = Projectile.Center;
                int radiusLeft = (int)(Center.X / 16f - radius);
                int radiusRight = (int)(Center.X / 16f + radius);
                int radiusUp = (int)(Center.Y / 16f - radius);
                int radiusDown = (int)(Center.Y / 16f + radius);
                if (radiusLeft < 0) { radiusLeft = 0; }
                if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
                if (radiusUp < 0) { radiusUp = 0; }
                if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

                float distRad = radius * 16f;
                for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
                {
                    for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                    {
                        double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, y1 * 16f + 8f), Center);
                        if (!WorldGen.InWorld(x1, y1, 0)) continue;
                        if (dist < distRad && Main.tile[x1, y1] != null)
                        {
                            WorldGen.Convert(x1, y1, ConversionID, 0);
                        }
                    }
                }
            });

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileType<ShockwaveBoom2>(), 0, 0, Projectile.owner);

            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NukeExplosion with { Pitch = .2f }, Projectile.position);

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 16; i++)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, -8 + Main.rand.Next(0, 17), -3 + Main.rand.Next(-11, 0), ProjectileType<MACE_Miniblast>(), Projectile.damage / 3, 3, Main.myPlayer, 1);
                    Main.projectile[proj].timeLeft = 300;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].netUpdate = true;
                }
            }
            RedeDraw.SpawnExplosion(Projectile.Center, Color.Orange, scale: 2);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.Orange * .7f, scale: 4);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.Orange * .4f, scale: 8);
            RedeHelper.NPCRadiusDamage(400, Projectile, Projectile.damage, Projectile.knockBack);
            RedeHelper.PlayerRadiusDamage(400, Projectile, Projectile.damage, Projectile.knockBack);
            for (int g = 0; g < 12; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
            }
        }
    }
    public class TerraBombaCorruption_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Corruption;
    }
    public class TerraBombaCrimson_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Crimson;
    }
    public class TerraBombaGlowingMushroom_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.GlowingMushroom;
    }
    public class TerraBombaHallow_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Hallow;
    }
    public class TerraBombaPurity_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Purity;
    }
    public class TerraBombaDirt_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Dirt;
    }
    public class TerraBombaSand_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Sand;
    }
    public class TerraBombaSnow_Proj : BaseTerraBombaProj
    {
        protected override int ConversionID => BiomeConversionID.Snow;
    }
}
