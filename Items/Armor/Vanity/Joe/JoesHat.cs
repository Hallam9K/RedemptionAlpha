using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Joe
{
    [AutoloadEquip(EquipType.Head)]
    public class JoesHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Joe's Feathered Fedora");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.vanity = true;
        }
    }
}