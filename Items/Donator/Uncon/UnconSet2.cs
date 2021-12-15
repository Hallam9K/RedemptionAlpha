using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Uncon
{
    [AutoloadEquip(EquipType.Body)]
	public class UnconBody2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Suit (Skinless)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 26;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UnconBody>())
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class UnconHead2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Visage (Skinless)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UnconHead>())
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class UnconLegs2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Boots (Skinless)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            if (male) equipSlot = Redemption.unconMaleLeg2ID;
            if (!male) equipSlot = Redemption.unconFemLeg2ID;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UnconLegs>())
                .Register();
        }
    }
}
