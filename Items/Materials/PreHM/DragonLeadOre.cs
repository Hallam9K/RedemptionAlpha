using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Materials.PreHM
{
    public class DragonLeadOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon-Lead Ore");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 11, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.Torch, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
    }
}
