using Terraria;
using Terraria.ModLoader;
using Redemption.Rarities;

namespace Redemption.Items.Armor.PostML.Hikarite
{
    [AutoloadEquip(EquipType.Legs)]
    public class HikariteGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("13% increased damage"
                + "\n8% increased critical strike chance" +
                "\n20% increased movement speed"); */

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 28;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .2f;
            player.GetDamage<GenericDamageClass>() += .13f;
            player.GetCritChance<GenericDamageClass>() += 8;
        }
    }
}