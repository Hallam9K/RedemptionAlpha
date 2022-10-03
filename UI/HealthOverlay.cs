using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.UI
{
    public class HealthOverlay : ModResourceOverlay
    {
        private readonly Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private Asset<Texture2D> heartTexture, barsFillingTexture;

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            int style = (Main.LocalPlayer.Redemption().medKit ? 1 : 0) + (Main.LocalPlayer.Redemption().galaxyHeart ? 1 : 0);

            if (style == 0)
                return;

            // NOTE: CompareAssets is defined below this method's body
            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
            {
                // Draw over the Classic hearts
                DrawClassicFancyOverlay(context, style);
            }
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
            {
                // Draw over the Fancy hearts
                DrawClassicFancyOverlay(context, style);
            }
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
            {
                // Draw over the Bars life bars
                DrawBarsOverlay(context, style);
            }
        }

        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }

        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context, int style)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HeartMed");
            if (style == 2)
                tex = ModContent.Request<Texture2D>("Redemption/Textures/HeartGal");

            context.texture = heartTexture ??= tex;
            context.Draw();

        }
        private void DrawBarsOverlay(ResourceOverlayDrawContext context, int style)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillMed");
            if (style == 2)
                tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillGal");

            context.texture = barsFillingTexture ??= tex;
            context.Draw();
        }
    }
}