using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class HamSandwich : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ham Sandwich");
            Tooltip.SetDefault("'Unleash doomsday upon this fragile universe'" +
                "\nSummons !!??");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<HalPet>(), ModContent.BuffType<HalPetBuff>());
            Item.width = 24;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 5);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}