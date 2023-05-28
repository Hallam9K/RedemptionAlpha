using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
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
            // DisplayName.SetDefault("Ice Mist");
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
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.One.RotatedBy(RedeHelper.RandomRotation()) * Main.rand.Next(400, 450), Vector2.Zero, ModContent.ProjectileType<Icefall_Mist>(), 0, 0, Main.myPlayer, 1);
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 540 * 540 && Projectile.DistanceSQ(target.Center) < 650 * 650)
                    target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
            }
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (!target.active || target.dead)
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 540 * 540 && Projectile.DistanceSQ(target.Center) < 650 * 650)
                    target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 180);
            }
        }
    }
    public class Calavia_IcefallMist : Icefall_Mist
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Mist");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = Main.rand.Next(140, 180);
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

            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 3;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 5;

                if (Main.rand.NextBool(15) && Projectile.alpha <= 100 && Main.myPlayer == Projectile.owner)
                {
                    int proj = ModContent.ProjectileType<Calavia_Icefall>();
                    if (Projectile.ai[0] is 1)
                        proj = ModContent.ProjectileType<Icefall_Proj>();
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.RandAreaInEntity(), Vector2.Zero, proj, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Main.projectile[p].DamageType = DamageClass.Summon;
                    Main.projectile[p].netUpdate = true;
                }

                if (Main.rand.NextBool(10) && Projectile.alpha <= 150)
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
                    if (Projectile.ai[0] is 0)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player target = Main.player[i];
                            if (!target.active || target.dead)
                                continue;

                            if (!Projectile.Hitbox.Intersects(target.Hitbox))
                                continue;

                            target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 60);
                        }
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