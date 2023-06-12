using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class TeslaCoil : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Arcs lightning from coil, targeting multiple enemies at the same time\n" +
                "Right-click to change firing modes between multi-target and single target"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.width = 30;
            Item.height = 86;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 2;
            Item.channel = true;
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.UseSound = CustomSounds.Spark1;
            Item.shoot = ModContent.ProjectileType<TeslaCoil_Proj>();
        }
        public bool AttackMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.mana = 0;
            else
                Item.mana = 20;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                AttackMode = !AttackMode;

                if (!AttackMode)
                    CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.TeslaCoil.Multi"), true, true);
                else
                    CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.TeslaCoil.Single"), true, true);
            }
            else
            {
                if (!AttackMode)
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                else
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 1);
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shotType;
            if (!AttackMode)
                shotType = Language.GetTextValue("Mods.Redemption.Items.TeslaCoil.Multi");
            else
                shotType = Language.GetTextValue("Mods.Redemption.Items.TeslaCoil.Single");
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.LightCyan,
            };
            tooltips.Add(line);
        }
    }
}
