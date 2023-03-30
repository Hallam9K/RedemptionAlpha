using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.BaseExtension;
using Terraria.GameContent.ItemDropRules;

namespace Redemption.Items.Usable
{
    public class OmegaGift : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega's Gift");
			/* Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}\n" +
                "'Gift from my friends to you'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
			Item.maxStack = 1;
            Item.consumable = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
		}
        public override bool OnPickup(Player player)
        {
            if (!player.Redemption().omegaGiftGiven)
                player.Redemption().omegaGiftGiven = true;
            return true;
        }
        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TBotVanityEyes>(), ModContent.ItemType<TBotVanityGoggles>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TBotVanityChestplate>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TBotVanityLegs>()));
        }
    }
}