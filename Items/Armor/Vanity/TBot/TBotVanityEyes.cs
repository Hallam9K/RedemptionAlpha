using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Head)]
    public class TBotVanityEyes : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("T-Bot Head");
            // Tooltip.SetDefault("Eyes");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
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
                .AddIngredient(ModContent.ItemType<TBotEyes_Masc>())
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TBotEyes_Femi>())
                .Register();
        }
    }
}