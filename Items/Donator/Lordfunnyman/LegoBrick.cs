using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lordfunnyman
{
    public class LegoBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Summons an ancient construct");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<Lordfunnyman_Pet>(), ModContent.BuffType<LegoPetBuff>());
            Item.width = 22;
            Item.height = 20;
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