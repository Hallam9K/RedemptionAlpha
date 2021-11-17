using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class HazmatSuit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Grants immunity to the Abandoned Lab and Wasteland water"
                + "\nGreatly extends underwater breathing"
                + "\nGrants protection against low-level radiation");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
            player.accDivingHelm = true;
            player.GetModPlayer<BuffPlayer>().hazmatSuit = true;
            player.GetModPlayer<BuffPlayer>().WastelandWaterImmune = true;
        }
    }
}