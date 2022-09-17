using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class ObliterationDrive : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Dealing damage to enemies has a chance to give the player a stack of Obliteration Motivation" +
                "\nObliteration Motivation increases damage and defense, at the cost of decreased life regen" +
                "\nStacks up to 5 times");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.expert = true;
            Item.accessory = true;
		}
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        }
	}
}
