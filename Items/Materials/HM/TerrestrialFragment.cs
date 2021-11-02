using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.HM
{
    public class TerrestrialFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terrestrial Fragment");
            Tooltip.SetDefault("'The blessing of life resides within this fragment'");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Cyan;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightGreen.ToVector3() * 0.55f * Main.essScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula)
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentStardust)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}