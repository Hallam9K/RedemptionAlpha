using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lizzy
{
    public class LizzyCookie : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lizard Cookie");
            /* Tooltip.SetDefault("Summons a chibi Lizzy\n" +
                "'\"Baked\" with love'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<LizzyPet>(), ModContent.BuffType<LizzyPetBuff>());
            Item.width = 24;
            Item.height = 14;
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.value = Item.sellPrice(0, 2);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Hay, 40)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}