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
            if (Main.LocalPlayer.Redemption().heartStyle != 1)
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

            if (!Main.LocalPlayer.RedemptionPlayerBuff().shieldGenerator || Main.LocalPlayer.RedemptionPlayerBuff().shieldGeneratorCD > 0)
                return;

            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("Redemption/Textures/OHeartKSShield");
            context.texture = tex2;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();
        }
        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillMed");
            context.texture = barsFillingTexture ??= tex;
            context.Draw();

            if (!Main.LocalPlayer.RedemptionPlayerBuff().shieldGenerator || Main.LocalPlayer.RedemptionPlayerBuff().shieldGeneratorCD > 0)
                return;

            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("Redemption/Textures/OHPFillKSShield");
            context.texture = tex2;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();
        }
    }
    public class GalHealthOverlay : ModResourceOverlay
    {
        private readonly Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private Asset<Texture2D> heartTexture, barPanelRightTexture;

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            if (Main.LocalPlayer.Redemption().heartStyle != 2)
                return;

            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            // NOTE: CompareAssets is defined below this method's body
            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
                DrawClassicFancyOverlay(context, 0);
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                DrawClassicFancyOverlay(context, 1);
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
            {
                if (context.resourceNumber % 2 == 0)
                    DrawBarsOverlay(context, 0);
                else
                    DrawBarsOverlay(context, 1);
            }
            else if (CompareAssets(asset, barsFolder + "HP_Panel_Right"))
                DrawBarsPanelOverlay(context);
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
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HeartGal");
            Asset<Texture2D> texFancy = ModContent.Request<Texture2D>("Redemption/Textures/HeartGal_Fancy");

            if (style == 1)
            {
                context.texture = texFancy;
                context.Draw();
            }
            else
            {
                context.texture = heartTexture ??= tex;
                context.Draw();
            }

            if (!Main.LocalPlayer.RedemptionPlayerBuff().shieldGenerator || Main.LocalPlayer.RedemptionPlayerBuff().shieldGeneratorCD > 0)
                return;

            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("Redemption/Textures/OHeartKSShield");
            context.texture = tex2;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();

        }
        private static void DrawBarsOverlay(ResourceOverlayDrawContext context, int style)
        {
            if (style == 0)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillGal");
                context.texture = tex;
                context.Draw();
            }
            else
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillGal2");
                context.texture = tex;
                context.Draw();
            }

            if (!Main.LocalPlayer.RedemptionPlayerBuff().shieldGenerator || Main.LocalPlayer.RedemptionPlayerBuff().shieldGeneratorCD > 0)
                return;

            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("Redemption/Textures/OHPFillKSShield");
            context.texture = tex2;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();
        }
        private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/HPFillGal_Panel_Right");
            context.texture = barPanelRightTexture ??= tex;
            context.Draw();
        }
    }
    public class KSShieldHealthOverlay : ModResourceOverlay
    {
        private readonly Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            if (!Main.LocalPlayer.RedemptionPlayerBuff().shieldGenerator || Main.LocalPlayer.RedemptionPlayerBuff().shieldGeneratorCD > 0)
                return;
            if (Main.LocalPlayer.Redemption().heartStyle != 0)
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
        private static void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/OHeartKSShield");
            context.texture = tex;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();
        }
        private static void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Redemption/Textures/OHPFillKSShield");
            context.texture = tex;
            context.color = new Color(255, 255, 255, 150) * 0.6f;
            context.Draw();
        }
    }
}