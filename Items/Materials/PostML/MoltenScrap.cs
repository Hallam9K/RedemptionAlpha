using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Usable;

namespace Redemption.Items.Materials.PostML
{
    public class MoltenScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Purple; 
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(5))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.Torch, 0, 0, 20);
            Main.dust[sparkle].velocity *= 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AIChip>())
                .AddTile(ModContent.TileType<XeniumSmelterTile>())
                .Register();
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<CyberPlating>())
                .AddTile(ModContent.TileType<XeniumSmelterTile>())
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddTile(ModContent.TileType<XeniumSmelterTile>())
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Plating>())
                .AddTile(ModContent.TileType<XeniumSmelterTile>())
                .Register();
            CreateRecipe(6)
                .AddIngredient(ModContent.ItemType<ScrapMetal>())
                .AddTile(ModContent.TileType<XeniumSmelterTile>())
                .Register();
        }
    }
}
