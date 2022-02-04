using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals.RenderTargets
{
    public class ShieldLayer
    {
        public RenderTarget2D EffectTarget;
        public RenderTarget2D Target;
        public List<IShieldSprite> Sprites;
        public Effect ShieldEffect;
        public Texture2D HexagonTexture;
        public Texture2D NegativeHexagons;
        public ShieldLayer()
        {
            Sprites = new List<IShieldSprite>();
            ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            HexagonTexture = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            NegativeHexagons = ModContent.Request<Texture2D>("Redemption/Textures/NegativeHexagons", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public interface IShieldSprite
        {
            void Draw(SpriteBatch spriteBatch);
        }
        // Prepare the layer to be drawn.
        public void PreDrawLayer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            // Set the RenderTarget to the main target.
            graphicsDevice.SetRenderTarget(Target);
            graphicsDevice.Clear(Color.Transparent);

            // Draw our sprites.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (IShieldSprite s in Sprites)
                s.Draw(spriteBatch);
            spriteBatch.End();
        }
        // Draw the layer.
        public void DrawLayer(SpriteBatch spriteBatch)
        {
            // Setup shader params.
            float c = 1f / 255f;

            ShieldEffect.Parameters["mask"].SetValue(new Vector4(0f, 1f, 0f, 1f));
            ShieldEffect.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 10f, Main.GlobalTimeWrappedHourly * 10f) / new Vector2(HexagonTexture.Width, HexagonTexture.Height));
            ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(Main.screenWidth / 2 / HexagonTexture.Width, Main.screenHeight / 2 / HexagonTexture.Height));
            ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (Main.screenWidth / 2), 1f / (Main.screenHeight / 2)));
            ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
            ShieldEffect.Parameters["border"].SetValue(new Vector4(215 * c, 79 * c, 214 * c, 1f));
            ShieldEffect.Parameters["inner"].SetValue(new Vector4(150 * c * 0.5f, 20 * c * 0.5f, 54 * c * 0.5f, 0.5f));
            ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
            ShieldEffect.Parameters["sinMult"].SetValue(30f);

            // Draw the main RenderTarget.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            ShieldEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(Target, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
