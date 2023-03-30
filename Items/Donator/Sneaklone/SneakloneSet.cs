using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Rarities;

namespace Redemption.Items.Donator.Sneaklone
{
    [AutoloadEquip(EquipType.Body)]
    class SneakloneSuit : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Facility Guard Vest");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Body.Sets.HidesTopSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesArms[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class SneakloneHelmet1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Facility Guard Helmet");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class SneakloneHelmet2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Facility Guard Helmet");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class SneakloneLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Facility Guard Leggings");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
    }
}