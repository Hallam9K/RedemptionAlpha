using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class RedeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool decapitated;
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            // Decapitation
            if (npc.life < npc.lifeMax && item.CountsAsClass(DamageClass.Melee) && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && NPCTags.SkeletonHumanoid.Has(npc.type))
            {
                if (Main.rand.NextBool(200) && !ItemTags.BluntSwing.Has(item.type))
                {
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
                else if (Main.rand.NextBool(80) && item.axe > 0)
                {
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
            }
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            DecapitationCondition decapitationDropCondition = new();
            IItemDropRule conditionalRule = new LeadingConditionRule(decapitationDropCondition);
            IItemDropRule rule = ItemDropRule.Common(ItemID.Skull);
            conditionalRule.OnSuccess(rule);
            globalLoot.Add(conditionalRule);
        }
    }
}