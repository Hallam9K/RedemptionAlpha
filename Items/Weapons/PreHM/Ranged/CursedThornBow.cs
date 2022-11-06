using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class CursedThornBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Replaces wooden arrows with seed-laden arrows that sprout small thorn traps");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 54;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(0, 0, 44, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<ThornArrow>();
        }
    }
}
