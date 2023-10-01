using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    public class Golem_GroundShock : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave");
            Main.projFrames[Projectile.type] = 6;
            ElementID.ProjEarth[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 6;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 30);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.height = 16 + (int)Projectile.ai[1];
            Projectile.position.Y -= (int)Projectile.ai[1] / 2;
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(Projectile.Bottom, Projectile.width, 2, DustID.Stone, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity.Y = -Main.rand.Next(3, 6) - (Projectile.ai[1] / 4);
            }
        }
    }
}
