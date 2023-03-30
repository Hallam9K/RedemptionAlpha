using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Head)]
    public class TBotGoggles_Femi : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("T-Bot Head");
            /* Tooltip.SetDefault("Goggles" +
                "\nFeminine"); */
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TBotVanityGoggles>())
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TBotGoggles_Masc>())
                .Register();
        }
    }
}