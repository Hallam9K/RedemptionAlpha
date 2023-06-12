using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Redemption.Items.Materials.PreHM;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Accessories.PreHM
{
    public class TrappedSoulBauble : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ArcaneS);
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("The player occasionally emits a strong force, causing every enemy caught in the blast to give a small magic damage boost" +
                "\n10% increased " + ElementID.ArcaneS + " elemental damage and resistance" +
                 "\n+20 max mana"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 10));
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().trappedSoul = true;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Arcane] += 0.10f;
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Arcane] += 0.10f;

            player.statManaMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LostSoul>(10)
                .AddIngredient(ItemID.Glass, 10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddIngredient(ItemID.Chain, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
