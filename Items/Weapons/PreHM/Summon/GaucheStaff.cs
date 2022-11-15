using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class GaucheStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Summons a granite guardian that emits an empowering aura\n" +
                "Within the aura, whip speed and damage is increased by 15%");
			SacrificeTotal = 1;

			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Summon;
			Item.sentry = true;
			Item.width = 48;
			Item.height = 56;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 1, 80, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<GraniteGuardian>();
			Item.mana = 20;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override bool CanUseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
				return false;

			return true;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			int floor = BaseWorldGen.GetFirstTileFloor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
			position = new Vector2(Main.MouseWorld.X, floor * 16 - 30);
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
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
