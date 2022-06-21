using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Effects.RenderTargets
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
            bool Active { get; set; }
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
            for (int i = 0; i < Sprites.Count; i++)
            {
                IShieldSprite sprite = Sprites[i];
                if (!sprite.Active)
                    Sprites.RemoveAt(i);
                sprite.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        // Draw the layer.
        public void DrawLayer(SpriteBatch spriteBatch)
        {
            // Draw the main RenderTarget.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(Target, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        public void Push(IShieldSprite item) => Sprites.Insert(0, item);
    }
}



