using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Shade;
using Terraria.Localization;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadeTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Can be placed in the Soulless Caverns");
            ItemID.Sets.Torches[Type] = true;
            SacrificeTotal = 100;
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
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ShadeTorchTile>();
            Item.width = 10;
            Item.height = 12;
            Item.value = 50;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<WastelandTorchDust>());
            }

            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 0.7f, 0.7f, 0.8f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 0.7f, 0.7f, 0.8f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            dryTorch = true;
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.Torch, 3)
                .AddIngredient(ModContent.ItemType<Shadestone>())
                .Register();
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.TorchRecipeGroup)
                .AddCondition(new Recipe.Condition(NetworkText.FromLiteral("In the Soulless Caverns"), _ => Main.LocalPlayer.InModBiome<SoullessBiome>()))
                .Register();
        }
    }
}