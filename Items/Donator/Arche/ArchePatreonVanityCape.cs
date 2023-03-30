using Redemption.Items.Materials.PreHM;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Arche
{
    [AutoloadEquip(EquipType.Back)]
    public class ArchePatreonVanityCape : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Iridescent Cape");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 34;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.accessory = true;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 5)
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 3)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
