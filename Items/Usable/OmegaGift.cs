using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Armor.Vanity.TBot;
using Terraria.GameContent.Creative;
using Redemption.BaseExtension;

namespace Redemption.Items.Usable
{
    public class OmegaGift : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega's Gift");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}\n" +
                "'Gift from my friends to you'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
        public override void RightClick(Player player)
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<TBotVanityEyes>());
                    break;
                case 1:
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<TBotVanityGoggles>());
                    break;
            }
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<TBotVanityChestplate>());
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<TBotVanityLegs>());
        }
    }
}