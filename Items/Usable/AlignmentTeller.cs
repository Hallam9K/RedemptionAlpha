using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class AlignmentTeller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chalice of Alignment");
            Tooltip.SetDefault("Tells you your current alignment"
                + "\n[c/ffea9b:A sentient treasure, cursed with visions of what is yet to come]");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.maxStack = 1;
            Item.noUseGraphic = true;
            Item.value = 22000;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                    .AddIngredient(ModContent.ItemType<CursedGem>())
                    .AddIngredient(ItemID.SteampunkCup)
                    .AddTile(TileID.DemonAltar)
                    .Register();
        }


        public override bool? UseItem(Player player)
        {
            if (RedeWorld.alignment == 0)
            {
            }
            return true;
        }
    }
}