using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class LogStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Log Staff");
            Tooltip.SetDefault("Summons a small log that fires acorns in an arc");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Summon;
			Item.sentry = true;
			Item.width = 36;
			Item.height = 36;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 7;
			Item.value = Item.sellPrice(0, 0, 75, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
			Item.autoReuse = false;
			Item.buffType = ModContent.BuffType<LogStaffBuff>();
			Item.shoot = ModContent.ProjectileType<LogStaff_Proj>();
			Item.mana = 4;
		}
		public override bool CanUseItem(Player player)
		{
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
				return false;

			return true;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			int floor = BaseWorldGen.GetFirstTileFloor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
			position = new Vector2(Main.MouseWorld.X, floor * 16 - 10);
		}
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			if (player.ownedProjectileCounts[type] < player.maxTurrets)
			{
				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, player.direction);
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
				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, player.direction);
				projectile.originalDamage = Item.damage;
			}
			return false;
		}
	}
}
