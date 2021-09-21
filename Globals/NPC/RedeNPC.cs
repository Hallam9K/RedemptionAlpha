using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
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

        public override bool CanHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            if (target.GetModPlayer<BuffPlayer>().skeletonFriendly)
            {
                if (NPCTags.SkeletonHumanoid.Has(npc.type))
                    return false;
            }
            return true;
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
            if (NPCTags.Undead.Has(npc.type) || NPCTags.Skeleton.Has(npc.type))
            {
                if (ItemTags.Holy.Has(item.type))
                    damage = (int)(damage * 1.25f);

                if (ItemTags.Shadow.Has(item.type))
                    damage = (int)(damage * 0.8f);
            }
            if (NPCTags.Demon.Has(npc.type))
            {
                if (ItemTags.Holy.Has(item.type) || ItemTags.Celestial.Has(item.type))
                    damage = (int)(damage * 1.5f);

                if (ItemTags.Fire.Has(item.type))
                    damage = (int)(damage * 0.5f);

                if (ItemTags.Water.Has(item.type))
                    damage = (int)(damage * 1.25f);
            }
            if (NPCTags.Spirit.Has(npc.type))
            {
                if (ItemTags.Holy.Has(item.type) || ItemTags.Celestial.Has(item.type) || ItemTags.Arcane.Has(item.type))
                    damage = (int)(damage * 1.25f);
            }
            if (NPCLists.IsSlime.Contains(npc.type))
            {
                if (ItemTags.Fire.Has(item.type))
                    damage = (int)(damage * 1.25f);

                if (ItemTags.Ice.Has(item.type))
                    damage = (int)(damage * 0.75f);

                if (ItemTags.Water.Has(item.type))
                    damage = (int)(damage * 0.5f);
            }
            if (NPCTags.Cold.Has(npc.type))
            {
                if (ItemTags.Fire.Has(item.type))
                    damage = (int)(damage * 1.25f);

                if (ItemTags.Ice.Has(item.type))
                    damage = (int)(damage * 0.5f);

                if (ItemTags.Thunder.Has(item.type))
                    damage = (int)(damage * 1.1f);
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
                else if (Main.rand.NextBool(80) && item.axe > 0 && item.type != ModContent.ItemType<BeardedHatchet>())
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
            if (NPCTags.Undead.Has(npc.type) || NPCTags.Skeleton.Has(npc.type))
            {
                if (ProjectileTags.Holy.Has(projectile.type))
                    damage = (int)(damage * 1.25f);

                if (ProjectileTags.Shadow.Has(projectile.type))
                    damage = (int)(damage * 0.8f);
            }
            if (NPCTags.Demon.Has(npc.type))
            {
                if (ProjectileTags.Holy.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type))
                    damage = (int)(damage * 1.5f);

                if (ProjectileTags.Fire.Has(projectile.type))
                    damage = (int)(damage * 0.5f);

                if (ProjectileTags.Water.Has(projectile.type))
                    damage = (int)(damage * 1.25f);
            }
            if (NPCTags.Spirit.Has(npc.type))
            {
                if (ProjectileTags.Holy.Has(projectile.type) || ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Arcane.Has(projectile.type))
                    damage = (int)(damage * 1.25f);
            }
            if (NPCLists.IsSlime.Contains(npc.type))
            {
                if (ProjectileTags.Fire.Has(projectile.type))
                    damage = (int)(damage * 1.25f);

                if (ProjectileTags.Ice.Has(projectile.type))
                    damage = (int)(damage * 0.75f);

                if (ProjectileTags.Water.Has(projectile.type))
                    damage = (int)(damage * 0.5f);
            }
            if (NPCTags.Cold.Has(npc.type))
            {
                if (ProjectileTags.Fire.Has(projectile.type))
                    damage = (int)(damage * 1.25f);

                if (ProjectileTags.Ice.Has(projectile.type))
                    damage = (int)(damage * 0.5f);

                if (ProjectileTags.Thunder.Has(projectile.type))
                    damage = (int)(damage * 1.1f);
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
        public override void OnKill(Terraria.NPC npc)
        {
            if (NPCID.Sets.Skeletons[npc.type] && Main.rand.NextBool(3) && !npc.SpawnedFromStatue)
                RedeHelper.SpawnNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
        public override void ModifyNPCLoot(Terraria.NPC npc, NPCLoot npcLoot)
        {
            DecapitationCondition decapitationDropCondition = new();
            IItemDropRule conditionalRule = new LeadingConditionRule(decapitationDropCondition);
            int itemType = ItemID.Skull;
            if (npc.type == ModContent.NPCType<CorpseWalkerPriest>())
                itemType = ModContent.ItemType<CorpseWalkerSkullVanity>();
            else if (npc.type == ModContent.NPCType<EpidotrianSkeleton>() || npc.type == ModContent.NPCType<SkeletonAssassin>() ||
                npc.type == ModContent.NPCType<SkeletonDuelist>() || npc.type == ModContent.NPCType<SkeletonFlagbearer>() ||
                npc.type == ModContent.NPCType<SkeletonNoble>() || npc.type == ModContent.NPCType<SkeletonWanderer>() ||
                npc.type == ModContent.NPCType<SkeletonWarden>())
                itemType = ModContent.ItemType<EpidotrianSkull>();

            IItemDropRule rule = ItemDropRule.Common(itemType);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
        }

        public override void EditSpawnRate(Terraria.Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (RedeWorld.blobbleSwarm)
            {
                spawnRate = 10;
                maxSpawns = 20;
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (RedeWorld.blobbleSwarm)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Blobble>(), 10f);
            }
        }
    }
}