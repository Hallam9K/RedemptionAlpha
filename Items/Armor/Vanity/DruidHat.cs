using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class DruidHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Druid's Hat");
            SacrificeTotal = 1;
            ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.vanity = true;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
        }
        // TODO: shimmer version of Wizard's Hat
    }
}