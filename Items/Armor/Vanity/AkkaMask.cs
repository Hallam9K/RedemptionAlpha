using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AkkaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}