using Microsoft.Xna.Framework;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Right-click to change attack modes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 36;
            Item.height = 36;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.value = Item.sellPrice(0, 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 420;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<Ukonvasara_Sword>();
        }

        public int AttackMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            Point tileBelow = player.Bottom.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);
            if (player.altFunctionUse != 2 && AttackMode == 2 && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;
            if (player.altFunctionUse == 2)
                Item.UseSound = SoundID.Item37;
            else
                Item.UseSound = SoundID.Item1;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                AttackMode++;
                if (AttackMode >= 3)
                    AttackMode = 0;

                switch (AttackMode)
                {
                    case 0:
                        CombatText.NewText(player.getRect(), Color.LightCyan, "Sword Mode", true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.LightCyan, "Hammer Mode", true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.LightCyan, "Axe Mode", true, false);
                        break;
                }
            }
            else
            {
                switch (AttackMode)
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Ukonvasara_Proj2>(), (int)(damage * 1.3f), knockback, player.whoAmI);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Ukonvasara_Axe>(), damage, knockback, player.whoAmI);
                        break;

                }
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shotType = "";
            switch (AttackMode)
            {
                case 0:
                    shotType = "Sword Mode: Does a three combo slash before being thrown";
                    break;
                case 1:
                    shotType = "Hammer Mode: Launches the player at cursor point, leaves an electric trail behind and strikes enemies hit by the weapon with lighting";
                    break;
                case 2:
                    shotType = "Axe Mode: While airborne, the axe will launch the player onto the ground, striking that location with lighting";
                    break;
            }
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.LightGoldenrodYellow,
            };
            tooltips.Add(line);
            if (Main.keyState.PressingShift())
            {
                TooltipLine line2 = new(Mod, "Lore",
                    "'The hammer of Ukko, crafted by his ancestors and refined by him. Upon his human death,\n" +
                    "the hammer was laid atop his grave as is custom in local tradition. It is a great conductor of Thunder magic.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line2);
            }
            else
            {
                TooltipLine line2 = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line2);
            }
            if (AttackMode > 0)
            {
                TooltipLine axeLine = new(Mod, "HammerBonus", "Hammer Bonus: Deals quadruple damage to Guard Points") { OverrideColor = Colors.RarityOrange };
                tooltips.Add(axeLine);
            }
        }
    }
}
