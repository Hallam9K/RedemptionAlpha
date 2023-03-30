using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.SkySquire
{
    [AutoloadEquip(EquipType.Legs)]
    public class SkySquiresGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sky Squire's Greaves");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 59, 0);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient(ItemID.Cloud, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Greaves made of Kanite, worn by the Sky Squires of Ithon. The metal has a blue hue and a cold touch,\n" +
                    "being used as a replacement for iron which is lacking in southern Ithon.\n\n" +
                    "The Sky Squires are a unit of southern Ithon, protecting the kingdoms of Yln, Klycub, and Norapass.\n" +
                    "The armour they wear allows better flexibility than most, useful for marching to the rescue swiftly.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}