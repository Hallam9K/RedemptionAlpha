using Redemption.Base;
using Redemption.Globals;
using Redemption.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Reflect : ModRedeProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Reflector Shield");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 58;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[2]];
            if (!npc.active || npc.ModNPC is not KS3)
                Projectile.Kill();

            Vector2 Pos = new(npc.Center.X + 48 * npc.spriteDirection, npc.Center.Y - 12);
            Projectile.Center = Pos;
            if (npc.ai[3] != 6 || npc.ai[0] != 3)
                Projectile.Kill();

            foreach (Projectile target in Main.ActiveProjectiles)
            {
                if (Projectile == target || !target.active || target.damage <= 0 || !target.friendly || target.hostile || target.ProjBlockBlacklist()) continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                for (int m = 0; m < 4; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)4 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                RedeDraw.SpawnExplosion(target.Center, new Color(200, 255, 221), shakeAmount: 0, scale: .2f, noDust: true, rot: RedeHelper.RandomRotation(), tex: "Redemption/Textures/SwordClash");

                SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                if (ProjectileID.Sets.CultistIsResistantTo[target.type])
                {
                    target.Kill();
                    continue;
                }
                if (Projectile.penetrate == 1)
                    Projectile.penetrate = 2;

                if (target.damage > 200)
                    target.damage = 200;

                target.damage /= 4;
                target.hostile = true;
                target.friendly = false;
                target.velocity = -target.velocity;
            }
        }
    }
}