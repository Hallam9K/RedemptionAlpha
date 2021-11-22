using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class FirstAidKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("First-Aid Kit");
            Tooltip.SetDefault("Does not give Potion Sickness\n" +
                "'These are in limited supply'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.healLife = 175;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}