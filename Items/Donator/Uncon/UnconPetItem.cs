using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Uncon
{
    public class UnconPetItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Terminal Blade");
            /* Tooltip.SetDefault("Summons chibi Tremor\n" +
                "'A broken sword not of this world, said to be a sign of the end times.'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<UnconPet>(), ModContent.BuffType<UnconPetBuff>());
            Item.width = 26;
            Item.height = 30;
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