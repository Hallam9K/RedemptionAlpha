using Redemption.Biomes;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    [AutoloadEquip(EquipType.Face)]
    public class MaskOfGrief : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Mask of Grief");
            Tooltip.SetDefault("Decreases enemy aggro while in Soulless Caverns"
                + "\n20% increased damage while in the Soulless Cavern");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.InModBiome(ModContent.GetInstance<SoullessBiome>()))
            {
                player.aggro -= 30;
                player.GetDamage(DamageClass.Generic) += 0.2f;
            }
        }
    }
}
