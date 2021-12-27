using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class TeslaGenerator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Field Generator");
            Tooltip.SetDefault("Summons a small generator with a tesla field around it");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Summon;
			Item.sentry = true;
			Item.width = 30;
			Item.height = 50;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item44;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<TeslaGeneratorBuff>();
			Item.shoot = ModContent.ProjectileType<TeslaGenerator_Proj>();
			Item.mana = 10;
		}
		public override bool CanUseItem(Player player)
		{
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			if (tile.IsActiveUnactuated && Main.tileSolid[tile.type] && !Main.tileCut[tile.type])
				return false;

			return true;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position = Main.MouseWorld;
		}
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			if (player.ownedProjectileCounts[type] < player.maxTurrets)
			{
				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
				projectile.originalDamage = Item.damage;
			}
			if (player.ownedProjectileCounts[type] == player.maxTurrets)
			{
				for (int g = 0; g < Main.maxProjectiles; ++g)
				{
					if (Main.projectile[g].active && Main.projectile[g].type == type)
					{
						Main.projectile[g].Kill();
						break;
					}
				}
				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
				projectile.originalDamage = Item.damage;
			}
			return false;
		}
	}
}
