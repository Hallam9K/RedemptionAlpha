using Redemption.Globals;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class EaglecrestSling : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ElementID.ItemEarth[Type] = true;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 34;
            Item.height = 22;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 75);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 40;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<EaglecrestSling_Throw>();
        }

        public int shot;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shot = (int)MathHelper.Clamp(shot, 0, 5);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<EaglecrestSling_Throw>(), damage, knockback, player.whoAmI, 0, 0, shot);
            return false;
        }
    }
}