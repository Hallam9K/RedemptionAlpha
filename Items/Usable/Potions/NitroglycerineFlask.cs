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
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = true;
            Item.width = 28;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.buffType = BuffType<ExplosiveFlaskBuff>();
            Item.buffTime = Item.flaskTime;
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
