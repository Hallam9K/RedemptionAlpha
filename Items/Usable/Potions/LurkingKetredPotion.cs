using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class LurkingKetredPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadevision Potion");
            Tooltip.SetDefault("Clears the haze of the Soulless Caverns");
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
            Item.width = 22;
            Item.height = 30;
            Item.maxStack = 30;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = ModContent.BuffType<ShadevisionPotionBuff>();
            Item.buffTime = 18000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShadeFish>())
                .AddIngredient(ModContent.ItemType<LurkingKetred>())
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
