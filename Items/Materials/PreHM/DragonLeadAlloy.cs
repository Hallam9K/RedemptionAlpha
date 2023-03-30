using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Tiles.Bars;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Materials.PreHM
{
    public class DragonLeadAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon-Lead Alloy");
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 70;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<DragonLeadAlloyTile>();
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

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<DragonLeadOre>(), 5)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}
