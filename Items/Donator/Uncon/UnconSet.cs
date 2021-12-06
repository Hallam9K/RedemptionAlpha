using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Uncon
{
    [AutoloadEquip(EquipType.Body)]
	public class UnconBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Suit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Body.Sets.HidesTopSkin[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesArms[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
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
                .AddIngredient(ModContent.ItemType<Archcloth>(), 2)
                .AddIngredient(ItemID.BlackThread, 3)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UnconBody2>())
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class UnconHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Visage");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 2)
                .AddIngredient(ItemID.BlackThread, 3)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                 .AddIngredient(ModContent.ItemType<UnconHead2>())
                 .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class UnconLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dominator Boots");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Legs.Sets.HidesBottomSkin[Mod.GetEquipSlot(Name, EquipType.Legs)] = true;
            ArmorIDs.Legs.Sets.HidesTopSkin[Mod.GetEquipSlot(Name, EquipType.Legs)] = true;
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
            if (male) equipSlot = Redemption.unconMaleLegID;
            if (!male) equipSlot = Redemption.unconFemLegID;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Archcloth>(), 2)
                .AddIngredient(ItemID.BlackThread, 3)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UnconLegs2>())
                .Register();
        }
    }
}
