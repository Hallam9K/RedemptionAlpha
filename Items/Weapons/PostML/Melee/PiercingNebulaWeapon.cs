using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.NPCs.Bosses.Neb;
using Redemption.Projectiles.Melee;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PiercingNebulaWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Nebula");
            Tooltip.SetDefault("'Penetrates through even the fabric of space'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 444;
            Item.DamageType = DamageClass.Melee;
            Item.width = 82;
            Item.height = 82;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.UseSound = SoundID.Item125;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PNebula1_Friendly>();
            Item.shootSpeed = 18f;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DayBreak)
                .AddIngredient(ModContent.ItemType<LifeFragment>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(i == 0 ? 0.5f : -0.5f), ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    }
                    break;
                case 2:
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(i == 0 ? 0.78f : -0.78f), ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    }
                    break;
                case 3:
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(i == 0 ? 0.6f : -0.6f), ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(i == 0 ? 1.2f : -1.2f), ModContent.ProjectileType<PNebula1_Friendly>(), damage, knockback, Main.myPlayer);
                    }
                    break;
            }
            return false;
        }
    }
}