using Redemption.Items.Materials.PostML;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class MutagenMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Warrior's Mutagen");
            /* Tooltip.SetDefault("15% increased melee damage"
                + "\n10% increased melee critical strike chance"); */
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
            player.GetDamage(DamageClass.Melee) *= 1.15f;
            player.GetCritChance(DamageClass.Melee) += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddIngredient(ModContent.ItemType<EmptyMutagen>())
                .AddIngredient(ItemID.DestroyerEmblem)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}