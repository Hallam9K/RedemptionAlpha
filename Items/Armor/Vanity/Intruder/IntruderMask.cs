using Redemption.Globals;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Intruder
{
    [AutoloadEquip(EquipType.Head)]
    public class IntruderMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Intruder's Mask");
            //Tooltip.SetDefault("'From Intrusion, with love'");
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
            ItemLists.HasPhysChain[Type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 14;
            Item.rare = ItemRarityID.LightRed;
            Item.vanity = true;
        }
        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.GetModPlayer<DrawEffectsPlayer>().SetPhysChain(new IntruderScarfPhysChain(), Item);
        }
    }
}
