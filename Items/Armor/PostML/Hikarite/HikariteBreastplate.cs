using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PostML.Hikarite
{
    [AutoloadEquip(EquipType.Body)]
    public class HikariteBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("14% increased damage"
                + "\n12% increased critical strike chance"); */

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 32;
            Item.sellPrice(7, 95, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 34;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += .14f;
            player.GetCritChance<GenericDamageClass>() += 12;
        }
    }
}