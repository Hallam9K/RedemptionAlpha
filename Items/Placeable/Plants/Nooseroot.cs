using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Plants
{
    public class Nooseroot : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
		{
			Item.maxStack = 99;
            Item.width = 18;
            Item.height = 32;
            Item.value = 200;
            Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}