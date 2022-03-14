using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;
using Terraria.DataStructures;

namespace Redemption.Items.Accessories.PreHM
{
    public class TrappedSoulBauble : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("The player occasionally emits a strong force, causing every enemy caught in the blast to give a small magic damage boost" +
                "\n10% increased Arcane elemental resistance" +
                 "\n10% increased Arcane elemental damage" +
                 "\n+20 max mana");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 10));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().trappedSoul = true;
            player.RedemptionPlayerBuff().ElementalDamage[1] += 0.10f;
            player.RedemptionPlayerBuff().ElementalResistance[1] += 0.10f;

            player.statManaMax2 += 20;
        }
    }
}
