using Microsoft.Xna.Framework;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.Calavia
{
    public class Calavia_Icefall : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Mist");
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.ai[0] == 4 || (npc.type != ModContent.NPCType<Calavia_Intro>() && npc.type != ModContent.NPCType<Calavia>()))
                Projectile.Kill();
            else
                Projectile.timeLeft = 10;

            if (Projectile.ai[1]++ % 6 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.One.RotatedBy(RedeHelper.RandomRotation()) * Main.rand.Next(350, 400), Vector2.Zero, ModContent.ProjectileType<Icefall_Mist>(), 0, 0, Main.myPlayer, 1);
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 480 * 480 && Projectile.DistanceSQ(target.Center) < 600 * 600)
                    target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
            }
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (!target.active || target.dead)
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 480 * 480 && Projectile.DistanceSQ(target.Center) < 600 * 600)
                    target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
            }
        }
    }
}