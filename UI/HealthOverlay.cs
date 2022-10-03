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
    public class MedHealthOverlay : ModResourceOverlay
    {
        private readonly Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private Asset<Texture2D> heartTexture, barsFillingTexture;

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            if (!Main.LocalPlayer.Redemption().medKit || Main.LocalPlayer.Redemption().galaxyHeart)
                return;

            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            // NOTE: CompareAssets is defined below this method's body
            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
                DrawClassicFancyOverlay(context);
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                DrawClassicFancyOverlay(context);
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                DrawBarsOverlay(context);
        }
        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }
        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HeartMed");
            context.texture = heartTexture ??= tex;
            context.Draw();
        }
        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillMed");
            context.texture = barsFillingTexture ??= tex;
            context.Draw();
        }
    }
    public class GalHealthOverlay : ModResourceOverlay
    {
        private readonly Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private Asset<Texture2D> heartTexture, barsFillingTexture;

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            if (!Main.LocalPlayer.Redemption().medKit && !Main.LocalPlayer.Redemption().galaxyHeart)
                return;

            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            // NOTE: CompareAssets is defined below this method's body
            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
                DrawClassicFancyOverlay(context);
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                DrawClassicFancyOverlay(context);
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                DrawBarsOverlay(context);
        }
        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }
        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HeartGal");
            context.texture = heartTexture ??= tex;
            context.Draw();
        }
        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillGal");
            context.texture = barsFillingTexture ??= tex;
            context.Draw();
        }
    }
}