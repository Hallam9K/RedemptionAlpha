using Redemption.Items.Usable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class RowanAuraBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rowan Aura");
            // Description.SetDefault("Empowered by nature itself");
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
    public class RowanAuraBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            if (player.HasBuff<RowanAuraBuff>() && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                modifiers.FinalDamage *= 1.08f;
                if (Main.rand.NextBool(10) && npc.CanBeChasedBy())
                    Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<RowanTreeSummon_Berries>(), noGrabDelay: true);
            }
        }
    }
}