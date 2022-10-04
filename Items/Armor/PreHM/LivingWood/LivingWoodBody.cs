using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.LivingWood
{
    [AutoloadEquip(EquipType.Body)]
    public class LivingWoodBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("+1 increased summon damage");

            SacrificeTotal = 1;
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
            player.GetDamage(DamageClass.Summon).Flat += 1;
        }
    }
}