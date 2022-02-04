using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals.RenderTargets
{
    public class RenderTargetManager : ModSystem
    {
        public static RenderTargetManager Instance;
        public static Vector2 lastViewSize;
        public EmberLayer EmberLayer;
        public ShieldLayer ShieldLayer;
        public override void OnModLoad()
        {
            Instance = this;
            Redemption.Targets = this;
            EmberLayer = new EmberLayer();
            ShieldLayer = new ShieldLayer();
            On.Terraria.Main.DrawNPCs += (orig, self, behindTiles) =>
            {
                DrawLayers(Main.spriteBatch);
                orig(self, behindTiles);
            };
            Main.OnResolutionChanged += CheckScreenSize;
            Main.OnPreDraw += (gameTime) => { PreDrawLayers(Main.spriteBatch, Main.graphics.GraphicsDevice); };
            UpdateRenderTargets();
        }
        public override void Unload()
        {
            Instance = null;
            Redemption.Targets = null;
            EmberLayer = null;
            ShieldLayer = null;
            On.Terraria.Main.DrawNPCs -= (orig, self, behindTiles) =>
            {
                DrawLayers(Main.spriteBatch);
                orig(self, behindTiles);
            };
            Main.OnResolutionChanged -= CheckScreenSize;
            Main.OnPreDraw -= (gameTime) => { PreDrawLayers(Main.spriteBatch, Main.graphics.GraphicsDevice); };
        }
        public void CheckScreenSize(Vector2 obj)
        {
            if (!Main.dedServ && lastViewSize != Main.ViewSize)
                UpdateRenderTargets();
        }
        public void UpdateRenderTargets()
        {
            Main.QueueMainThreadAction(() =>
            {
                EmberLayer.Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                EmberLayer.EffectTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                ShieldLayer.Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                ShieldLayer.EffectTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }
        public void PreDrawLayers(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            RenderTargetBinding[] previousTargets = graphicsDevice.GetRenderTargets();

            if (EmberLayer?.Sprites.Count > 0)
                EmberLayer.PreDrawLayer(spriteBatch, graphicsDevice);
            if (ShieldLayer?.Sprites.Count > 0)
                ShieldLayer.PreDrawLayer(spriteBatch, graphicsDevice);

            graphicsDevice.SetRenderTargets(previousTargets);
        }
        private void DrawLayers(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            if (EmberLayer?.Sprites.Count > 0)
                EmberLayer.DrawLayer(spriteBatch);
            if (ShieldLayer?.Sprites.Count > 0)
                ShieldLayer.DrawLayer(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
