using Microsoft.Xna.Framework;
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
        }
        public override bool? UseItem(Player player)
        {
            int throwingKnife = player.FindItem(ItemID.ThrowingKnife);
            int poisonedKnife = player.FindItem(ItemID.PoisonedKnife);
            if (poisonedKnife >= 0)
            {
                player.inventory[poisonedKnife].stack--;
                if (player.inventory[poisonedKnife].stack <= 0)
                    player.inventory[poisonedKnife] = new Item();
            }
            else if (throwingKnife >= 0)
            {
                player.inventory[throwingKnife].stack--;
                if (player.inventory[throwingKnife].stack <= 0)
                    player.inventory[throwingKnife] = new Item();
            }
            return null;
        }

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

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.FindItem(ItemID.PoisonedKnife) >= 0)
            {
                type = ModContent.ProjectileType<FanOShivsPoison_Proj>();
                damage += 3;
            }
            else if (player.FindItem(ItemID.ThrowingKnife) >= 0)
                damage += 3;
        }
    }
}
