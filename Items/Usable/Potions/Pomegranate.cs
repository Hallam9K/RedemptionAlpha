using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Pomegranate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'No more pomegranates!'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
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
            Item.width = 28;
            Item.height = 26;
            Item.value = 80;
            Item.rare = ItemRarityID.Green;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 12000;
        }
    }
}