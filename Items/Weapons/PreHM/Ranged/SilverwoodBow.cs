using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Redemption.Items.Weapons.PreHM.Melee;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class SilverwoodBow : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Daerel's Silverwood Bow");
            /* Tooltip.SetDefault("20% chance not to consume ammo"
                + "\nShoots silverwood arrows that stick onto enemies, draining their life"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SwordSlicer>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 36;
            Item.height = 60;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 11;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 15f;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextFloat() >= .2f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SilverwoodArrow>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.SilverwoodBow.Lore"))
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
