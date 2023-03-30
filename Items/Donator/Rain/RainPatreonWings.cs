using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.HM;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Rain
{
    [AutoloadEquip(EquipType.Wings)]
    public class RainPatreonWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wings of a Living Weapon");
            // Tooltip.SetDefault("'Best hold back, you wouldn't want to hurt yourself'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 12)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
