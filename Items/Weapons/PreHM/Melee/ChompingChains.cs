using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ChompingChains : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Throws three skulls from a flail\n" +
                "The skulls will latch onto enemies, dealing damage for 5 seconds before letting go"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.damage = 28;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 75, 0);
            Item.UseSound = CustomSounds.ChainSwing;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<ChompingChains_Proj>();
            Item.shootSpeed = 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChainKnife)
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 6)
                .AddIngredient(ItemID.Bone, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.ChompingChains.Lore"))
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
