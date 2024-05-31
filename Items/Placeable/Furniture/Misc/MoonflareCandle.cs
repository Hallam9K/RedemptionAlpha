using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Dusts;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class MoonflareCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SingleUseInGamepad[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToTorch(ModContent.TileType<MoonflareCandleTile>(), 0, false);
            Item.width = 16;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 1);
        }
        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<MoonflareDust>());
            }

            player.AddBuff(ModContent.BuffType<MoonflareCandleBuff>(), 10);
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, .95f, .8f, .6f);
        }

        public override void PostUpdate()
        {
            if (!Item.wet)
                Lighting.AddLight(Item.Center, .95f, .8f, .6f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MoonflareFragment>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}