using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class MossyGoliath_SS_Screech : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Roar");
        }
        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 124;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.alpha += 4;
            Projectile.scale += .04f;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    npc.AddBuff(BuffID.Confused, 200);
            }
        }
    }
}