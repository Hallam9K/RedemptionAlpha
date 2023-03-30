using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Waasephi
{
    public class WaasephiPet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Purple Yarn");
            // Tooltip.SetDefault("Summons a chibi Waasephi");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<WaasephiPet_Proj>(), ModContent.BuffType<WaasephiPetBuff>());
            Item.width = 24;
            Item.height = 26;
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.value = Item.sellPrice(0, 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient(ItemID.PurpleDye)
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