using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Plants;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class ChakrogAnglerPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Angler Potion");
            /* Tooltip.SetDefault("You emit bright light while submerged" +
                "\nClears the Soulless Cavern's waters" +
                "\nIncreased damage while submerged"); */
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = ModContent.BuffType<AnglerPotionBuff>();
            Item.buffTime = 36000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ChakrogAngler>())
                .AddIngredient(ModContent.ItemType<Nooseroot>(), 2)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
