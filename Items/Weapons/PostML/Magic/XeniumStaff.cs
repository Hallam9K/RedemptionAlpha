using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Projectiles.Magic;
using Redemption.Projectiles.Ranged;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class XeniumStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Casts two harmless bubble mines\n" +
                "Right-click to fire a small beam that detonates any mine it hits");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = 58;
            Item.height = 58;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item117;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<XeniumBubble_Proj>();
            Item.shootSpeed = 14f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 70f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                position += Offset;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<XeniumStaff_Proj>();
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
            for (int i = 0; i < 2; i++)
            {
                velocity += velocity.RotatedByRandom(0.4f);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 12)
                .AddIngredient(ModContent.ItemType<Capacitator>())
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 5)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}