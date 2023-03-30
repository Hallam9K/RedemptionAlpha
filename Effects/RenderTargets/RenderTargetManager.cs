using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Effects.RenderTargets
{
    public class RenderTargetManager : ModSystem
    {
        public static RenderTargetManager Instance;
        public static Vector2 lastViewSize;
        public BasicLayer BasicLayer;
        public EmberLayer EmberLayer;
        public ShieldLayer ShieldLayer;
        public override void OnModLoad()
        {
            Instance = this;
            Redemption.Targets = this;
            BasicLayer = new BasicLayer();
            EmberLayer = new EmberLayer();
            ShieldLayer = new ShieldLayer();
            Terraria.On_Main.DrawNPCs += (orig, self, behindTiles) =>
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
            BasicLayer = null;
            EmberLayer = null;
            ShieldLayer = null;
            Terraria.On_Main.DrawNPCs -= (orig, self, behindTiles) =>
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
            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    BasicLayer.Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    BasicLayer.EffectTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    EmberLayer.Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    EmberLayer.EffectTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    ShieldLayer.Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    ShieldLayer.EffectTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            }
        }
        public void PreDrawLayers(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            RenderTargetBinding[] previousTargets = graphicsDevice.GetRenderTargets();

            if (BasicLayer?.Sprites.Count > 0)
                BasicLayer.PreDrawLayer(spriteBatch, graphicsDevice);
            if (EmberLayer?.Sprites.Count > 0)
                EmberLayer.PreDrawLayer(spriteBatch, graphicsDevice);
            if (ShieldLayer?.Sprites.Count > 0)
                ShieldLayer.PreDrawLayer(spriteBatch, graphicsDevice);

            graphicsDevice.SetRenderTargets(previousTargets);
        }
        private void DrawLayers(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            if (BasicLayer?.Sprites.Count > 0)
                BasicLayer.DrawLayer(spriteBatch);
            if (EmberLayer?.Sprites.Count > 0)
                EmberLayer.DrawLayer(spriteBatch);
            if (ShieldLayer?.Sprites.Count > 0)
                ShieldLayer.DrawLayer(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
