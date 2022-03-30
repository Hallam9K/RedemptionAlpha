using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class ChakrogAnglerPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angler Potion");
            Tooltip.SetDefault("You emit bright light while submerged" +
                "\nClears the Soulless Cavern's waters" +
                "\nIncreased damage while submerged");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
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
            Item.maxStack = 30;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = ModContent.BuffType<AnglerPotionBuff>();
            Item.buffTime = 36000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ChakrogAngler>())
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
