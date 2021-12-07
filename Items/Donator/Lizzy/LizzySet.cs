using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.HM;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Redemption.Globals;

namespace Redemption.Items.Donator.Lizzy
{
    [AutoloadEquip(EquipType.Body)]
	class LizzyBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beezard Outfit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Body.Sets.HidesTopSkin[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesArms[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
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
                .AddIngredient(ItemID.BeeShirt)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.BeeShirt)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class LizzyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beezard Hood");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeeHat)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.BeeHat)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class LizzyLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beezard Pants");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Legs.Sets.HidesBottomSkin[Mod.GetEquipSlot(Name, EquipType.Legs)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeePants)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.BeePants)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Back)]
    public class LizzyTail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beezard Abdomen");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 16;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Stinger)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.GreenThread)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
