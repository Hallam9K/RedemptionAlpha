using Microsoft.Xna.Framework;
using Redemption.Projectiles.Minions;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Summon
{
    public class Pihlajasauva : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Summons a rowan tree that emits an empowering aura\n" +
                "Within the aura, your minions can cause rowan berries to drop from their targets and their damage is increased by 8%\n" +
                "Rowan berries will heal for a small amount and give major improvements to all stats for a short time\n" +
                "Right-click to disable the sentry");*/
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RowanTreeSummon>();
            Item.mana = 28;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.type != type || proj.owner != player.whoAmI)
                        continue;
                    proj.timeLeft = 2;
                }
                return false;
            }
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, player.direction);
            projectile.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
