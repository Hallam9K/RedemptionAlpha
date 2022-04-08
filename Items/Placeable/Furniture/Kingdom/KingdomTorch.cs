using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Furniture.Kingdom;

namespace Redemption.Items.Placeable.Furniture.Kingdom
{
    public class KingdomTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.flame = true;
            Item.noWet = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.autoReuse = true;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<KingdomTorchTile>();
            Item.width = 10;
            Item.height = 12;
            Item.value = 50;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<WastelandTorchDust>());
            }

            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 1f, 0.6f, 0.1f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 1f, 0.6f, 0.1f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            dryTorch = true;
        }

        /*public override void AddRecipes() // TODO: Kingdom torch recipe
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.Torch, 3)
                .AddIngredient(ModContent.ItemType<Shadestone>())
                .Register();
        }*/
    }
}