using Microsoft.Xna.Framework;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class FanOShivs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fan o' Shivs");
            /* Tooltip.SetDefault("Not consumable" +
                "\nConsumes throwing knives if any are in your inventory, increasing damage" +
                "\n'I'm sorry, Edwin...'"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SoulScepter>();

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item19;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 13;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<FanOShivs_Proj>();
            Item.useAmmo = ItemID.ThrowingKnife;
        }

        public override bool NeedsAmmo(Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 3;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void PickAmmo(Item weapon, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (type == ItemID.PoisonedKnife)
            {
                type = ProjectileType<FanOShivsPoison_Proj>();
                damage.Flat += 3;
            }
            else if (type == ItemID.ThrowingKnife)
            {
                type = ProjectileType<FanOShivs_Proj>();
                damage.Flat += 3;
            }
        }
    }
}