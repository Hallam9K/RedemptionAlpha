using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class WardensKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warden's Key");
            Tooltip.SetDefault("Used to open a gate in the temple");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 24;
            Item.value = 0;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}