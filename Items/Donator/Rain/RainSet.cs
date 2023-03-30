using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.HM;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Rain
{
    [AutoloadEquip(EquipType.Body)]
	class RainPatreonVanityBody : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Form of a Living Weapon");
            // Tooltip.SetDefault("'A body etched by agony and filled with strength'");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
			Item.width = 30;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 12)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class RainPatreonVanityHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Horns of a Living Weapon");
            // Tooltip.SetDefault("'Strange, you can't seem to smile'");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 4)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class RainPatreonVanityLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Talons of a Living Weapon");
            // Tooltip.SetDefault("'With claws like these, who needs a sword?'");
            Item.ResearchUnlockCount = 1;
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
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 4)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Back)]
    public class RainPatreonVanityAcc : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tail of a Living Weapon");
            // Tooltip.SetDefault("'You can feel every twitch, even the slightest breeze'");
            ArmorIDs.Back.Sets.DrawInTailLayer[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Back)] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 12;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 3)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 2)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
