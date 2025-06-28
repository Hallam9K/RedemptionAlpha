using Redemption.Buffs;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class SoulCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SingleUseInGamepad[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToTorch(TileType<SoulCandleTile>(), 0, false);
            Item.width = 16;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 1);
        }
        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, DustID.DungeonSpirit);
            }

            player.AddBuff(BuffType<SoulboundBuff>(), 10);
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 0.8f, 0.9f, 0.9f);
        }

        public override void PostUpdate()
        {
            if (!Item.wet)
                Lighting.AddLight(Item.Center, 0.8f, 0.9f, 0.9f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(2)
                .AddIngredient<LostSoul>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}