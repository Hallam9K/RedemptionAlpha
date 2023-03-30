using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
    public class KabucraShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kabucra Shell");
            /* Tooltip.SetDefault("25% damage reduction and knockback immunity towards falling entities\n" +
                "'Time for crab'"); */
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = 550;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<BuffPlayer>().shellCap = true;
        }
    }
}