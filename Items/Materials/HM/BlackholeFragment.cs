using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Materials.HM
{
    public class BlackholeFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blackhole Fragment");
            // Tooltip.SetDefault("'The crushing gravity of a black hole resides within this fragment'");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Cyan;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.55f * Main.essScale);
        }
        // TODO: re-add recipe when rit is done
        /*public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula)
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentStardust)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }*/
    }
}