using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class FriedChicken : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'I'm lovin' it'");

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
            Item.width = 20;
            Item.height = 20;
            Item.value = 80;
            Item.rare = ItemRarityID.Blue;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 20000;
        }
    }
}