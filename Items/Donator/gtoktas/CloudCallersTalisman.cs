using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.gtoktas
{
    public class CloudCallersTalisman : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<CloudCallerPet>(), ModContent.BuffType<CloudCallerPetBuff>());
            Item.width = 30;
            Item.height = 32;
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