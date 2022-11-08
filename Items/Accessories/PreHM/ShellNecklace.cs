using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Neck)]
    public class ShellNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shell Necklace");
            Tooltip.SetDefault("10% increased Nature elemental damage and resistance\n" +
                "'Makes you feel one with nature'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.ElementalDamage[9] += 0.1f;
            modPlayer.ElementalResistance[9] += 0.1f;
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