using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Neck)]
    public class SacredCross : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("12% increased Holy elemental damage and resistance\n" +
                "6% increased Holy elemental critical strike chance\n" +
                "Critical strikes with a Holy elemental weapon has a chance to release homing lightmass\n" +
                "Increases length of invincibility after taking damage");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 46;
            Item.value = Item.sellPrice(0, 2, 80, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrossNecklace)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient<LostSoul>(10)
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.ElementalDamage[7] += 0.12f;
            modPlayer.ElementalResistance[7] += 0.12f;
            player.longInvince = true;
            modPlayer.sacredCross = true;
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (slot < 10)
            {
                int maxAccessoryIndex = 5 + player.extraAccessorySlots;
                for (int i = 3; i < 3 + maxAccessoryIndex; i++)
                {
                    if (slot != i && player.armor[i].type == ModContent.ItemType<GracesGuidance>())
                        return false;
                }
            }
            return true;
        }
    }
}
