using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Megaswave
{
    public class SmolderedScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Summons a chibi Miniswave");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<MegaswavePet>(), ModContent.BuffType<MegaswavePetBuff>());
            Item.width = 22;
            Item.height = 28;
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