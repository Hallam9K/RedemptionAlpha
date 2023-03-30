using Redemption.Buffs;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class BileFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flask of Bile");
            // Tooltip.SetDefault("Melee attacks inflict Burning Acid");
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = true;
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.buffType = ModContent.BuffType<BileFlaskBuff>();
            Item.buffTime = 52000;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 2)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}
