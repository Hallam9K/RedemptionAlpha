using Redemption.Items.Materials.PostML;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class MutagenSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Summoner's Mutagen");
            /* Tooltip.SetDefault("15% increased summon damage"
                + "\n10% increased summon critical strike chance"); */
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
            player.GetDamage(DamageClass.Summon) *= 1.15f;
            player.GetCritChance(DamageClass.Summon) += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ModContent.ItemType<EmptyMutagen>())
                .AddIngredient(ItemID.DestroyerEmblem)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}