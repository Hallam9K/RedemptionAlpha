using Redemption.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class NebuleusMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus Mask");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
    }
}