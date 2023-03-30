using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lantard
{
    public class LantardPatreon_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fluffy Scarf");
            // Tooltip.SetDefault("Summons a chibi Ralsei");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<LantardPatreon_Pet>(), ModContent.BuffType<LantardPetBuff>());
            Item.width = 30;
            Item.height = 24;
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.value = Item.sellPrice(0, 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 15)
                .AddIngredient(ItemID.PinkDye)
                .AddTile(TileID.Loom)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}