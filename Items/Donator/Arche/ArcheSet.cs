using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Arche
{
    [AutoloadEquip(EquipType.Body)]
	class ArchePatreonVanityBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iridescent Outfit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            DisplayName.SetDefault("Iridescent Hat");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Mod.GetEquipSlot(Name, EquipType.Head)] = true;
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
            DisplayName.SetDefault("Iridescent Leggings");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            if (!male)
                equipSlot = Mod.AddEquipTexture(ModContent.GetInstance<ArchePatreonVanityLegs>(), EquipType.Legs, "Redemption/Items/Donator/Arche/ArchePatreonVanityLegs_FemaleLegs");
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
