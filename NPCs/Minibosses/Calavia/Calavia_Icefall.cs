using Microsoft.Xna.Framework;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.Calavia
{
    public class Calavia_IcefallArena : ModProjectile
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
    public class Calavia_IcefallMist : Icefall_Mist
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Mist");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override bool PreAI()
        {
            Projectile.velocity *= 0.98f;
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.Next(1, 3);

            if (Projectile.localAI[0] is 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] is 2)
                Projectile.rotation += 0.003f;

            if (Projectile.timeLeft < 120)
            {
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 5;

                if (Main.rand.NextBool(30) && Projectile.alpha <= 100 && Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.RandAreaInEntity(), Vector2.Zero, ModContent.ProjectileType<Calavia_Icefall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (Main.rand.NextBool(20) && Projectile.alpha <= 150)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }

                if (Projectile.alpha <= 100)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];
                        if (!target.active || !target.friendly || target.dontTakeDamage)
                            continue;

                        if (!Projectile.Hitbox.Intersects(target.Hitbox))
                            continue;

                        target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
                    }
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player target = Main.player[i];
                        if (!target.active || target.dead)
                            continue;

                        if (!Projectile.Hitbox.Intersects(target.Hitbox))
                            continue;

                        target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
                    }
                }
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            return false;
        }
    }
    public class Calavia_Icefall : Icefall_Proj
    {
        public override string Texture => "Redemption/Projectiles/Magic/Icefall_Proj";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
    }
}