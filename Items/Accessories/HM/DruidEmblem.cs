using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.DamageClasses;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Accessories.HM
{
    public class DruidEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Druid Emblem");
            Tooltip.SetDefault("15% increased druidic damage");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<DruidClass>() *= 1.15f;
        }
    }
}