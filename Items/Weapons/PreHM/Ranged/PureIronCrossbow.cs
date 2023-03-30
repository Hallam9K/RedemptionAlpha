using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class PureIronCrossbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Crossbow");
            /* Tooltip.SetDefault("Replaces arrows with frigid bolts that stick to enemies and eventually cause a frosty eruption\n" +
                "The eruption's damage increases for each bolt stuck to the target and can freeze most enemies" +
                "\nEnemies with knockback immunity cannot be frozen"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 72;
            Item.height = 26;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.UseSound = SoundID.Item89;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 45;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<FrigidBolt>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 10)
                .AddTile(TileID.Anvils)
                .Register();
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