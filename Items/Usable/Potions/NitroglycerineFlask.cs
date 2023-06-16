using Redemption.Buffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class NitroglycerineFlask : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ExplosiveS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flask of Nitroglycerine");
            // Tooltip.SetDefault("Melee attacks gain the Explosive Bonus");
            Item.ResearchUnlockCount = 20;
            ElementID.ItemExplosive[Type] = true;
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
            Item.buffType = ModContent.BuffType<ExplosiveFlaskBuff>();
            Item.buffTime = 52000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.ExplosivePowder, 5)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}
