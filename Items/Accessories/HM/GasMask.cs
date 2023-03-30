using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Face)]
    public class GasMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gas Mask");
            /* Tooltip.SetDefault("Grants immunity to Radioactive Fallout\n" +
                "'Hudda hudda!'"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<RadioactiveFalloutDebuff>()] = true;
        }
    }
}