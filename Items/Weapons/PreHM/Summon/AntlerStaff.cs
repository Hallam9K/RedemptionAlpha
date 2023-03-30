using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class AntlerStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Summons a Forret to fight for you");
			Item.ResearchUnlockCount = 1;

			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 46;
			Item.DamageType = DamageClass.Summon;
			Item.width = 52;
			Item.height = 52;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 45, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item44;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<ForretBuff>();
			Item.shoot = ModContent.ProjectileType<Forret>();
			Item.mana = 10;
		}

		public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<PureIronAlloy>(), 7)
				.AddIngredient(ItemID.Leather, 4)
				.AddTile(TileID.Anvils)
				.Register();
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			knockback = 0;
			position = Main.MouseWorld;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			return false;
		}
	}
}
