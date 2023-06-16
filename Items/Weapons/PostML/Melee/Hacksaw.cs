using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Hacksaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Automated Hacksaw");
            // Tooltip.SetDefault("Right-click to change attack modes");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 54;
            Item.height = 82;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 7, 50);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 160;
            Item.knockBack = 4;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<Hacksaw_Proj>();
        }

        public int AttackMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.UseSound = CustomSounds.ShootChange;
            else
                Item.UseSound = SoundID.Item23;
            if (AttackMode == 2)
                Item.axe = 35;
            else
                Item.axe = 0;
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
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Hacksaw.Mode1"), true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Hacksaw.Mode2"), true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.Hacksaw.Mode3"), true, false);
                        break;
                }
            }
            else
            {
                switch (AttackMode)
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 2);
                        break;

                }
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shotType = "";
            if (AttackMode is 1)
                Item.ExtraItemShoot(ModContent.ProjectileType<Hacksaw_Heat_Proj>());
            else
                Item.ExtraItemShoot();
            switch (AttackMode)
            {
                case 0:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Hacksaw.AttackMode1");
                    break;
                case 1:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Hacksaw.AttackMode2.1") + ElementID.FireS + Language.GetTextValue("Mods.Redemption.Items.Hacksaw.AttackMode2.2");
                    break;
                case 2:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.Hacksaw.AttackMode3");
                    break;
            }
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.LightCyan,
            };
            tooltips.Add(line);
            if (Main.keyState.PressingShift())
            {
                TooltipLine line2 = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.Hacksaw.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line2);
            }
            else
            {
                TooltipLine line2 = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.Hacksaw.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line2);
            }
        }
    }
}
