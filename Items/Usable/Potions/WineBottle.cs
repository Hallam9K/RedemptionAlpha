using Redemption.Buffs;
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

            Tooltip.SetDefault("Massive improvements to all stats" +
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
            Item.width = 24;
            Item.height = 58;
            Item.value = 80;
            Item.rare = ItemRarityID.Cyan;
            Item.buffType = ModContent.BuffType<WellFed4>();
            Item.buffTime = 20000;
        }
    }
}