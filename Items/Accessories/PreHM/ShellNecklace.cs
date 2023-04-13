using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Neck)]
    public class ShellNecklace : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shell Necklace");
            /* Tooltip.SetDefault("10% increased " + ElementID.NatureS + " elemental damage and resistance\n" +
                "Increased chance of Nature Boons to drop from the " + ElementID.NatureS + " elemental bonus\n" +
                "'Makes you feel one with nature'"); */
            ElementID.ItemNature[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.ElementalDamage[ElementID.Nature] += 0.1f;
            modPlayer.ElementalResistance[ElementID.Nature] += 0.1f;
            modPlayer.shellNecklace = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TreeBugShell>(), 2)
                .AddIngredient(ModContent.ItemType<CoastScarabShell>(), 2)
                .AddIngredient(ItemID.Chain, 4)
                .AddRecipeGroup(RedeRecipe.SilverRecipeGroup)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}