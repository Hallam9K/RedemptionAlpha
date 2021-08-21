using Microsoft.Xna.Framework;
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
        public bool invisible;
        public Entity attacker = Main.LocalPlayer;
        public Terraria.NPC npcTarget;

        public override void ResetEffects(Terraria.NPC npc)
        {
            invisible = false;
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            #region Elemental Attributes
            if (NPCTags.Plantlike.Has(npc.type))
            {
                if (ItemTags.Fire.Has(item.type))
                    damage = (int)(damage * 1.15f);

                if (ItemTags.Nature.Has(item.type))
                    damage = (int)(damage * 0.75f);

                if (ItemTags.Poison.Has(item.type))
                    damage = (int)(damage * 0.5f);
            }
            else if (NPCTags.Undead.Has(npc.type) || NPCTags.Skeleton.Has(npc.type))
            {
                if (ItemTags.Holy.Has(item.type))
                    damage = (int)(damage * 1.25f);

                if (ItemTags.Shadow.Has(item.type))
                    damage = (int)(damage * 0.8f);
            }
            #endregion

            // Decapitation
            if (npc.life < npc.lifeMax && item.CountsAsClass(DamageClass.Melee) && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && NPCTags.SkeletonHumanoid.Has(npc.type))
            {
                if (Main.rand.NextBool(200) && !ItemTags.BluntSwing.Has(item.type))
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
                else if (Main.rand.NextBool(80) && item.axe > 0)
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    damage = damage < npc.life ? npc.life : damage;
                    crit = true;
                }
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            #region Elemental Attributes
            if (NPCTags.Plantlike.Has(npc.type))
            {
                if (ProjectileTags.Fire.Has(projectile.type))
                    damage = (int)(damage * 1.15f);

                if (ProjectileTags.Nature.Has(projectile.type))
                    damage = (int)(damage * 0.75f);

                if (ProjectileTags.Poison.Has(projectile.type))
                    damage = (int)(damage * 0.5f);
            }
            else if (NPCTags.Undead.Has(npc.type) || NPCTags.Skeleton.Has(npc.type))
            {
                if (ProjectileTags.Holy.Has(projectile.type))
                    damage = (int)(damage * 1.25f);

                if (ProjectileTags.Shadow.Has(projectile.type))
                    damage = (int)(damage * 0.8f);
            }
            #endregion
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<RedeNPC>().attacker = npc;
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, int damage, float knockback, bool crit)
        {
            attacker = player;
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            Terraria.Player player = Main.player[npc.GetNearestAlivePlayer()];
            if (projectile.friendly && !projectile.hostile)
                attacker = player;
            else if (npc.ClosestNPCToNPC(ref npc, 1000, npc.Center))
                attacker = npc;
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