using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.DamageClasses;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;

namespace Redemption.Items.Accessories.PostML
{
    public class MutagenDruid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Druid's Mutagen");
            Tooltip.SetDefault("15% increased druidic damage"
                + "\n10% increased druidic critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<DruidClass>() *= 1.15f;
            player.GetCritChance<DruidClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TerrestrialFragment>(), 10)
                .AddIngredient(ModContent.ItemType<EmptyMutagen>())
                .AddIngredient(ItemID.DestroyerEmblem)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}