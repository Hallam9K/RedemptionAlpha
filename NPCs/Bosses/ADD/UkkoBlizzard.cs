using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoBlizzard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Spike");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Blizzard);
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
        public override void PostAI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, Projectile.Opacity * 0.7f, Projectile.Opacity * 1f);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Chilled, 120);
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.Frozen, 40);
        }
    }
}
