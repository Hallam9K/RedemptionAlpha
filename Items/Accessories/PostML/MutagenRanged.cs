using Redemption.Items.Materials.PostML;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class MutagenRanged : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ranger's Mutagen");
            /* Tooltip.SetDefault("15% increased ranged damage"
                + "\n10% increased ranged critical strike chance"); */
            Item.ResearchUnlockCount = 1;
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
            player.GetDamage(DamageClass.Ranged) *= 1.15f;
            player.GetCritChance(DamageClass.Ranged) += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 10)
                .AddIngredient(ModContent.ItemType<EmptyMutagen>())
                .AddIngredient(ItemID.DestroyerEmblem)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}