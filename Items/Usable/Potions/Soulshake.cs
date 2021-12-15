using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Soulshake : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medium improvements to all stats" +
                "\n'Tastes like marshmallow'");

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
            Item.width = 22;
            Item.height = 38;
            Item.value = 80;
            Item.rare = ItemRarityID.Blue;
            Item.buffType = BuffID.WellFed2;
            Item.buffTime = 16000;
        }
    }
}