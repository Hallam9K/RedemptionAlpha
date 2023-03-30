using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class MysteriousXenomiteFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Summons a lil Xenomite Elemental to light your way!");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<XenomiteElementalPet>(), ModContent.BuffType<XenomiteElementalPetBuff>());
            Item.rare = ItemRarityID.Lime;
            Item.width = 28;
            Item.height = 28;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}