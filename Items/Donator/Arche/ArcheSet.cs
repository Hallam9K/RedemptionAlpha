using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.PreHM;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Arche
{
    [AutoloadEquip(EquipType.Body)]
	public class ArchePatreonVanityBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Iridescent Outfit");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 7)
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 4)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class ArchePatreonVanityHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Iridescent Hat");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 5)
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 2)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class ArchePatreonVanityLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Iridescent Leggings");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            if (male) equipSlot = Redemption.archeMaleLegID;
            if (!male) equipSlot = Redemption.archeFemLegID;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 6)
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 2)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
