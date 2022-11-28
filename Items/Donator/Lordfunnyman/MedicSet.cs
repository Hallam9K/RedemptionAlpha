using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Lordfunnyman
{
    [AutoloadEquip(EquipType.Body)]
    public class MedicOutfit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic Outfit");
            SacrificeTotal = 1;
            ArmorIDs.Body.Sets.HidesArms[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class MedicLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic Boots");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
    [AutoloadEquip(EquipType.Back)]
    public class MedicBackpack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic Backpack Kit");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ModContent.RarityType<DonatorRarity>();
            Item.accessory = true;
            Item.vanity = true;
        }
    }
}