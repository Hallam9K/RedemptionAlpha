using Redemption.Items.Materials.PreHM;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Uncon
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class UnconPatreon_CapeAcc : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Dominator Coat Cape");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.accessory = true;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 3)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
