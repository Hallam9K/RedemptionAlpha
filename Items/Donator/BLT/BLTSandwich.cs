using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.BLT
{
    public class BLTSandwich : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<BLTSandwichPet>(), ModContent.BuffType<BLTSandwichBuff>());
            Item.width = 28;
            Item.height = 26;
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.value = Item.sellPrice(0, 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RottenChunk, 4)
                .AddRecipeGroup(RecipeGroupID.Wood)
                .AddTile(TileID.CookingPots)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 4)
                .AddRecipeGroup(RecipeGroupID.Wood)
                .AddTile(TileID.CookingPots)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}