using Redemption.Buffs.Minions;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class MicroshieldCore_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Microshield Core");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 34;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;
        NPC target;
        public int timer;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            CheckActive(owner);
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if (RedeHelper.ClosestNPC(ref target, 700, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
                if (Projectile.DistanceSQ(target.Center) >= 200 * 200)
                    Projectile.Move(target.Center, 10, 40);
                else
                    Projectile.velocity *= 0.98f;

                if (++Projectile.ai[0] % 50 == 0 && Main.myPlayer == owner.whoAmI)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Laser1 with { Volume = .6f }, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center) * 8, ModContent.ProjectileType<MicroshieldCore_Bolt>(), 100, 3, owner.whoAmI);
                }
            }
            else
            {
                if (Projectile.DistanceSQ(owner.Center) >= 200 * 200)
                    Projectile.Move(owner.Center, Projectile.DistanceSQ(owner.Center) > 700 * 700 ? 18 : 10, 40);
                else
                    Projectile.velocity *= 0.98f;
            }

            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MicroshieldCoreBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}
