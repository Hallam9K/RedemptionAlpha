using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PostML.Vorti
{
    [AutoloadEquip(EquipType.Body)]
    public class VortiRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("+100 max life"
                + "\n15% increased magic damage"
                + "\n5% increased magic critical strike chance"); */

            ArmorIDs.Body.Sets.HidesArms[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.sellPrice(7, 95, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 36;
        }
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 100;
            player.GetDamage<MagicDamageClass>() += .15f;
            player.GetCritChance<MagicDamageClass>() += 5;
        }
    }
}