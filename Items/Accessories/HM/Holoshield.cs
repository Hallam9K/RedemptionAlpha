using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Shield)]
    public class Holoshield : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("6% damage reduction"
                + "\nDouble tap a direction to dash" +
                "\nDashing into projectiles will reflect them" +
                "\nCan't reflect projectiles exceeding 200 damage");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 22;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DashPlayer modPlayer = player.GetModPlayer<DashPlayer>();
            modPlayer.plasmaShield = true;
            player.endurance += 0.06f;
        }
    }
}
