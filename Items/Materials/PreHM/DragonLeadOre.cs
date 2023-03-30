using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Tiles.Ores;

namespace Redemption.Items.Materials.PreHM
{
    public class DragonLeadOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon-Lead Ore");
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 70;
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 11, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<DragonLeadOreTile>();
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
