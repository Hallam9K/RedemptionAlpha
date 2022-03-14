using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Items.Armor.PreHM.LivingWood
{
    [AutoloadEquip(EquipType.Body)]
    public class LivingWoodBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("+1 increased druidic damage");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.sellPrice(0, 0, 0, 35);
            Item.rare = ItemRarityID.White;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.RedemptionPlayerBuff().DruidDamageFlat += 1;
        }       
    }
}