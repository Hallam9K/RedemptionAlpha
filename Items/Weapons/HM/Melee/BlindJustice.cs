using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BlindJustice : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blind Justice, Demon's Terror");
            /* Tooltip.SetDefault("Hold left-click to charge a Radiance Spin Slash\n" +
                "Right-click the moment you're about to be hit by a physical attack (contact damage or weapon) to perform a parry, granting immunity frames\n" +
                "Contact damage parries will not take effect if the enemy is stationary or moving twice as slow as you\n" +
                "After a successful parry, left-click to counter with an instant Radiance Spin Slash\n" +
                "Deals double damage against demonic enemies"); */

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 72;
            Item.height = 72;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 3, 50);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 108;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<BlindJustice_Proj>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 20)
                .AddIngredient(ModContent.ItemType<LostSoul>(), 10)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, player.position);
                type = ModContent.ProjectileType<BlindJustice_Proj2>();
            }
            else
                type = ModContent.ProjectileType<BlindJustice_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.BlindJustice.Lore"))
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
