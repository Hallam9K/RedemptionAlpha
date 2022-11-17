using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class SunAuraBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Aura");
            Description.SetDefault("Empowered by ancient lihzahrd powers");
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
    public class SunAuraBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            if (player.HasBuff<SunAuraBuff>() && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                damage = (int)(damage * 1.08f);
                npc.AddBuff(BuffID.OnFire3, 120);
            }
        }
    }
}