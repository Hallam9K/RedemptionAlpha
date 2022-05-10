using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class ForbiddenFries : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starlite Fries");

            Tooltip.SetDefault("Medium improvements to all stats" +
                "\n'Delicious'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 26;
            Item.height = 34;
            Item.value = 80;
            Item.rare = ItemRarityID.Lime;
            Item.buffType = BuffID.WellFed2;
            //also maybe infect player when eaten
            Item.buffTime = 26000;
        }
    }
}