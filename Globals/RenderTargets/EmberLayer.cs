using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals.RenderTargets
{
  public class EmberLayer
  {
    public RenderTarget2D EffectTarget;
    public RenderTarget2D Target;
    public List<IEmberSprite> Sprites;
    public Effect EmbersParallax;
    public Texture2D EmbersTexture;
    public EmberLayer()
    {
      Sprites = new List<IEmberSprite>();
      EmbersParallax = ModContent.Request<Effect>("Redemption/Effects/EmbersParallax", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
      EmbersTexture = ModContent.Request<Texture2D>("Redemption/Textures/Embers", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    }
    public interface IEmberSprite
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
      foreach (IEmberSprite s in Sprites)
        s.Draw(spriteBatch);
      spriteBatch.End();
    }
    // Draw the layer.
    public void DrawLayer(SpriteBatch spriteBatch)
    {
      // Setup shader params.
      EmbersParallax.Parameters["border"].SetValue(Main.hslToRgb((1f / 360f) * 14, 0.76f, 0.41f, 0).ToVector4());
      EmbersParallax.Parameters["mask"].SetValue(new Vector4(0f, 1f, 0f, 1f));
      EmbersParallax.Parameters["offset"].SetValue(Main.player[Main.myPlayer].position * 0.21f / new Vector2(EmbersTexture.Width, EmbersTexture.Height));
      EmbersParallax.Parameters["spriteRatio"].SetValue(new Vector2(Main.screenWidth / 2 / EmbersTexture.Width, Main.screenHeight / 2 / EmbersTexture.Height));
      EmbersParallax.Parameters["conversion"].SetValue(new Vector2(1f / (Main.screenWidth / 2), 1f / (Main.screenHeight / 2)));
      EmbersParallax.Parameters["sampleTexture"].SetValue(EmbersTexture);

      // Draw the main RenderTarget.
      spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
      EmbersParallax.CurrentTechnique.Passes[0].Apply();
      spriteBatch.Draw(Target, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
      spriteBatch.End();
    }
  }
}
