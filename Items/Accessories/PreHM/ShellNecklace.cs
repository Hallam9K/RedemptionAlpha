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

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 38;
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
                .AddIngredient(ItemType<TreeBugShell>(), 2)
                .AddIngredient(ItemType<CoastScarabShell>(), 2)
                .AddIngredient(ItemID.Chain, 4)
                .AddRecipeGroup(RedeRecipe.SilverRecipeGroup)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}