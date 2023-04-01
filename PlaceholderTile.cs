using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption
{
    public class PlaceholderTile : ModItem
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("null");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            SetSafeStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
        }
    }
}