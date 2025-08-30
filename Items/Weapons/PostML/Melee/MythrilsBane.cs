using Redemption.BaseExtension;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Projectiles.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class MythrilsBane : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mythril's Bane");
            /* Tooltip.SetDefault("Hitting armed enemies with the blade inflicts Disarmed and Broken Armor\n" +
                "Disarmed heavily decreases contact damage and their weapon damage\n" +
                "Blocks weak physical projectiles"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<DarkSteelBow>();
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 84;
            Item.height = 84;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 170;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<MythrilsBaneSlash_Proj>();

            Item.Redemption().TechnicallySlash = true;
            Item.Redemption().CanSwordClash = true;
        }
        public override bool MeleePrefix() => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale2 = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<MythrilsBane_Proj>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.MythrilsBane.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}