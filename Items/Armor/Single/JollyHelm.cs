using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
    public class JollyHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sunset Helm");
            // Tooltip.SetDefault("'Comes from an ashen world'");
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.value = 7500;
            Item.rare = ItemRarityID.Gray;
            Item.defense = 4;
        }
    }
}