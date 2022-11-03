using Redemption.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class EnchantedPear : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'I eat pears'");

            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 28;
            Item.height = 30;
            Item.value = 120;
            Item.rare = ItemRarityID.Cyan;
            Item.buffType = ModContent.BuffType<WellFed4>();
            Item.buffTime = 10000;
        }
    }
}