using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SkySquiresCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Squire's Cap");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 0, 24, 0);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawAltHair = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
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
                    "'A cap made of Kanite, worn by the Sky Squires of Ithon. The metal has a blue hue and a cold touch,\n" +
                    "being used as a replacement for iron which is lacking in southern Ithon.\n\n" +
                    "The Sky Squires are a unit of southern Ithon, protecting the kingdoms of Yln, Klycub, and Yesnora.\n" +
                    "The armour they wear allows better flexibility than most, useful for marching to the rescue swiftly'")
                {
                    overrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}