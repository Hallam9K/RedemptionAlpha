using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Minions;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Summon
{
    public class XeniumDrone : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Autoturret");
            /* Tooltip.SetDefault("Summons a friendly Xenium Autoturret to fight for you"
                + "\nFires bullets from your inventory"
                + "\n80% chance not to consume ammo"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Summon;
            Item.width = 30;
            Item.height = 28;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 45, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<XeniumTurretBuff>();
            Item.shoot = ModContent.ProjectileType<XeniumTurret>();
            Item.mana = 10;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 15)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 10)
                .AddIngredient(ModContent.ItemType<AIChip>(), 1)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}