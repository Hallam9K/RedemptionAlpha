using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class ElderWoodCrossbow : ModItem
    {
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 54;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1, 0, 0);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.UseSound = SoundID.Item89;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 12;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<ElderWoodBolt>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}