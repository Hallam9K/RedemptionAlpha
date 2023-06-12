using Microsoft.Xna.Framework;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Right-click to change attack modes");
            Item.ResearchUnlockCount = 1;
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
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.Mode1"), true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.Mode2"), true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.Mode3"), true, false);
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
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.SwordMode");
                    break;
                case 1:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.HammerMode");
                    break;
                case 2:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.AxeMode");
                    break;
            }
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.LightGoldenrodYellow,
            };
            tooltips.Add(line);
            if (Main.keyState.PressingShift())
            {
                TooltipLine line2 = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.Ukonvasara.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line2);
            }
            else
            {
                TooltipLine line2 = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line2);
            }
            switch (AttackMode)
            {
                default:
                    TooltipLine swordLine = new(Mod, "SlashBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(swordLine);
                    break;
                case 1:
                    TooltipLine hammerLine = new(Mod, "HammerBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.HammerBonus")) { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(hammerLine);
                    break;
                case 2:
                    TooltipLine axeLine = new(Mod, "AxeBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.AxeBonus")) { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(axeLine);
                    break;
            }
        }
    }
}
