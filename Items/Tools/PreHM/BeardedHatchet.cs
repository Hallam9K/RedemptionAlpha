using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Items.Tools.PreHM
{
    public class BeardedHatchet : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Increased chance to decapitate skeletons, guaranteeing skull drops" +
                "\nDeals 75% more damage to skeletons"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
			Item.damage = 11;
            Item.DamageType = DamageClass.Melee;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 25;
            Item.useAnimation = 30;
            Item.axe = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 6550;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
		}

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Skeleton.Contains(target.type))
                modifiers.FinalDamage *= 1.75f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool skele = NPCLists.SkeletonHumanoid.Contains(target.type);
            bool humanoid = skele || NPCLists.Humanoid.Contains(target.type);
            if (target.life < target.lifeMax && target.life < damageDone * 100 && humanoid)
            {
                if (Main.rand.NextBool(skele ? 20 : 80))
                {
                    CombatText.NewText(target.getRect(), Color.Orange, "Decapitated!");
                    target.Redemption().decapitated = true;
                    hit.Crit = true;
                    target.StrikeInstantKill();
                }
            }
        }
    }
}