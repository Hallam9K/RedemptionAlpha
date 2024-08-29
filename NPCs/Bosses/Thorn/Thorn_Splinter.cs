using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_Splinter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjNature[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.timeLeft < 120)
                Projectile.velocity.Y += .01f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            int chance = Main.expertMode ? 2 : 4;
            if (Main.rand.NextBool(chance))
                target.AddBuff(BuffID.Poisoned, 80);
            Projectile.timeLeft = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = Main.expertMode ? 2 : 4;
            if (Main.rand.NextBool(chance))
                target.AddBuff(BuffID.Poisoned, 80);
            Projectile.timeLeft = 2;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.JungleGrass);
        }
    }
}