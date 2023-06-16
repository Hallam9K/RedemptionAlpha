using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class PureIronStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Staff");
            /* Tooltip.SetDefault("Casts an icy snowflake" +
                "\nHold down left click to increase the size of the snowflake"); */
            Item.staff[Item.type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 38;
            Item.height = 44;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.reuseDelay = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.channel = true;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.shootSpeed = 8;
            Item.shoot = ModContent.ProjectileType<IceBolt>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<PureIronStaff_Proj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 10)
            .AddIngredient(ItemID.Diamond, 3)
            .AddTile(TileID.Anvils)
            .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.PureIronStaff.Lore"))
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
