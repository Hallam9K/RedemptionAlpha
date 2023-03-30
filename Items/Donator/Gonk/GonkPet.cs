using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Gonk
{
    public class GonkPet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Samus Head");
            // Tooltip.SetDefault("Summons a chibi Samus\n");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<GonkPatreon_Pet>(), ModContent.BuffType<GonkPetBuff>());
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
    }
}