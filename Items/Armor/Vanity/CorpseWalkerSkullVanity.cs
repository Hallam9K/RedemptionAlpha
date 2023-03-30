using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CorpseWalkerSkullVanity : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse-Walker Skull");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
        }   
    }
}