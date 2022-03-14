using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;
using Redemption.Base;

namespace Redemption.Items.Tools.PreHM
{
    public class BeardedHatchet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increased chance to decapitate skeletons, guaranteeing skull drops" +
                "\nDeals 45% more damage to skeletons");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.life < target.lifeMax && NPCLists.SkeletonHumanoid.Contains(target.type))
            {
                if (Main.rand.NextBool(20))
                {
                    CombatText.NewText(target.getRect(), Color.Orange, "Decapitated!");
                    target.Redemption().decapitated = true;
                    damage = damage < target.life ? target.life : damage;
                    crit = true;
                }
            }
            if (NPCLists.Skeleton.Contains(target.type))
            {
                damage = (int)(damage * 1.45f);
            }
        }
    }
}