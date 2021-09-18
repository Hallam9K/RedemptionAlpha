using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class GathicCryoCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            Tooltip.SetDefault("'A freezing cold crystal'"
                + "\nMakes the player Chilled when held");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(10))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.SilverCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.Chilled, 60);
        }
    }
}
