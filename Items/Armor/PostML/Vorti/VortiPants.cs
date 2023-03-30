using Terraria;
using Terraria.ModLoader;
using Redemption.Rarities;
using Terraria.ID;

namespace Redemption.Items.Armor.PostML.Vorti
{
    [AutoloadEquip(EquipType.Legs)]
    public class VortiPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("50% increased movement speed"
                + "\n15% increased magic damage and critical strike chance"); */

            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            ArmorIDs.Legs.Sets.IncompatibleWithFrogLeg[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 30;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .5f;
            player.GetDamage<MagicDamageClass>() += .15f;
            player.GetCritChance<MagicDamageClass>() += 15;
        }
    }
}