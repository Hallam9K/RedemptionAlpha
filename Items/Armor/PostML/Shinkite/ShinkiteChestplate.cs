using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PostML.Shinkite
{
    [AutoloadEquip(EquipType.Body)]
    public class ShinkiteChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = Redemption.shinkiteCapeID;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = Redemption.shinkiteCapeID;

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 32;
            Item.sellPrice(7, 95, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 38;
        }
    }
}