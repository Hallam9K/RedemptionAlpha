using Redemption.Globals;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Projectiles.Hostile
{
    public class LivingBloomRoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Root");
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 28;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.Redemption().Unparryable = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return target.type != ModContent.NPCType<LivingBloom>() ? null : false;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2) && Projectile.localAI[0] < 30)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, 2, DustID.DryadsWard, 0, 0);
                Main.dust[dust].velocity.Y -= 4f;
                Main.dust[dust].velocity.X *= 0f;
                Main.dust[dust].noGravity = true;
            }
            if (Projectile.velocity.Length() != 0)
            {
                Projectile.hostile = true;
                Projectile.friendly = true;
            }
            else 
            {
                Projectile.hostile = false;
                Projectile.friendly = false;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 30)
                Projectile.alpha -= 10;
            else if (Projectile.localAI[0] == 30)
            {
                Projectile.hostile = true;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                Projectile.velocity.Y -= 3;
            }
            else if (Projectile.localAI[0] == 40)
            {
                Projectile.velocity.Y = 0;
            }
            else if (Projectile.localAI[0] > 45)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}