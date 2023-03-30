using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CorpseWalkerStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Corpse-Walker Staff");
			// Tooltip.SetDefault("Summons a Corpse-Walker Skull to fight for you");
			Item.ResearchUnlockCount = 1;

			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Summon;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 3;
			Item.value = 400;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item44;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<CorpseSkullBuff>();
			Item.shoot = ModContent.ProjectileType<CorpseWalkerSkull>();
            Item.ExtraItemShoot(ModContent.ProjectileType<CorpseWalkerSkull_Proj>());
            Item.mana = 5;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
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