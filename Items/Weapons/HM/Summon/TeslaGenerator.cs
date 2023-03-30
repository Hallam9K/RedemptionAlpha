using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class TeslaGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tesla Field Generator");
            // Tooltip.SetDefault("Summons a small generator with a tesla field around it");
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<TeslaGeneratorBuff>();
            Item.shoot = ModContent.ProjectileType<TeslaGenerator_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<TeslaGenerator_Lightning>());
            Item.mana = 10;
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
            position = new Vector2(Main.MouseWorld.X, floor * 16 - 22);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            player.UpdateMaxTurrets();
            return false;
        }
    }
}
