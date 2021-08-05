using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class CoastScarabShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Glistens in water'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 75);
            Item.rare = ItemRarityID.White;
        }
        public override void PostUpdate()
        {
            if (Item.wet && !Item.lavaWet && !Item.honeyWet && Main.rand.NextBool(20))
            {
                int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height / 2, DustID.SilverCoin, 0, 0, 20);
                Main.dust[sparkle].velocity *= 0;
                Main.dust[sparkle].noGravity = true;
            }
        }
    }
}