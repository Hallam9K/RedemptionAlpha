using Terraria;
using Terraria.ID;
using Redemption.DamageClasses;
using Terraria.ModLoader;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Accessories.PostML
{
    public class MutagenRitualist : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ritualist's Mutagen");
            Tooltip.SetDefault("15% increased ritual damage"
                + "\n10% increased ritual critical strike chance");
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
            player.GetDamage<RitualistClass>() *= 1.15f;
            player.GetCritChance<RitualistClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BlackholeFragment>(), 10)
                .AddIngredient(ModContent.ItemType<EmptyMutagen>())
                .AddIngredient(ItemID.DestroyerEmblem)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}