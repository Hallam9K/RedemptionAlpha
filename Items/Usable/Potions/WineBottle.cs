using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class WineBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bottle of Wine");

            Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'A reward for a long days work'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 18;
            Item.height = 42;
            Item.value = 80;
            Item.rare = ItemRarityID.Blue;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 6000;
        }
    }
}