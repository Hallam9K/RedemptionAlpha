using Redemption.BaseExtension;
using Redemption.Items.Weapons.PostML.Ranged;
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
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<Salamanisku>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 36;
            Item.height = 36;
            Item.rare = RarityType<TurquoiseRarity>();
            Item.value = Item.sellPrice(0, 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 500;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<Ukonvasara_Sword_Proj>();
        }

        public int AttackMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.UseSound = SoundID.Item37;
            else
                Item.UseSound = SoundID.Item1;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale2 = player.GetAdjustedItemScale(Item);

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
                        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<Ukonvasara_Hammer>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<Ukonvasara_Axe>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
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
                    Item.Redemption().CanSwordClash = true;
                    Item.Redemption().TechnicallySlash = true;
                    Item.Redemption().TechnicallyHammer = false;
                    Item.Redemption().TechnicallyAxe = false;
                    break;
                case 1:
                    Item.Redemption().CanSwordClash = false;
                    Item.Redemption().TechnicallySlash = false;
                    Item.Redemption().TechnicallyHammer = true;
                    Item.Redemption().TechnicallyAxe = false;
                    break;
                case 2:
                    Item.Redemption().CanSwordClash = false;
                    Item.Redemption().TechnicallySlash = false;
                    Item.Redemption().TechnicallyHammer = false;
                    Item.Redemption().TechnicallyAxe = true;
                    break;
            }
        }
    }
}
