using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Wasteland;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class MutatedLivingBloomRoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutated Living Root");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ElementID.ProjNature[Type] = true;
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
            Projectile.Redemption().friendlyHostile = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return target.type != ModContent.NPCType<MutatedLivingBloom>() ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override void AI()
        {
            if (Main.rand.NextBool(2) && Projectile.localAI[0] < 30)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, 2, DustID.LifeDrain, 0, 0);
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
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.life < host.lifeMax - 10)
            {
                int steps = (int)host.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(host.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                host.life += 10;
                host.HealEffect(10);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.life < host.lifeMax - 10)
            {
                int steps = (int)host.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(host.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                host.life += 10;
                host.HealEffect(10);
            }
        }
    }
}