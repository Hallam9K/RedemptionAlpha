using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Redemption.Globals.Players
{
    public class PlayerPhysChainTextureContent : ARenderTargetContentByRequest, ILoadable
    {
        public static PlayerPhysChainTextureContent PlayerPhysChain;

        private PlayerDrawSet _drawInfo;

        public float Priority => 1f;

        public bool LoadOnDedServer => true;

        public void Load()
        {
            PlayerPhysChain = new();
            Main.ContentThatNeedsRenderTargets.Add(PlayerPhysChain);
        }

        public void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(PlayerPhysChain);
            PlayerPhysChain = null;
        }

        public void ProvideInfo(PlayerDrawSet drawInfo)
        {
            _drawInfo = drawInfo;
        }

        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (_drawInfo.drawPlayer == null)
            {
                return;
            }

            // Setup data
            Terraria.Player player = _drawInfo.drawPlayer;
            DrawEffectsPlayer drawEffectsPlayer = player.GetModPlayer<DrawEffectsPlayer>();
            IPhysChain chain = drawEffectsPlayer.bodyPhysChain;

            if (chain == null)
            {
                return;
            }

            // Get data
            List<DrawData> dataSet = RenderLayerHelper.DrawSegments(ref _drawInfo);

            // Setup target
            Point dimensions = GetDimensionsFomChain(chain);
            PrepareARenderTarget_AndListenToEvents(ref _target, device, dimensions.X, dimensions.Y, RenderTargetUsage.PreserveContents);
            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, Main.DefaultSamplerState, null, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

            // Draw!
            for (int i = 0; i < dataSet.Count; i++)
            {
                DrawData data = dataSet[i];
                data.position += _target.Bounds.Size() / 2f;
                data.Draw(spriteBatch);
            }

            // Clean up
            spriteBatch.End();
            device.SetRenderTarget(null);

            _wasPrepared = true;
        }

        private static Point GetDimensionsFomChain(IPhysChain chain)
        {
            int totalLength = 0;
            for (int i = 0; i < (chain?.NumberOfSegments ?? 0); i++)
            {
                totalLength += chain.Length(i);
            }

            totalLength *= 3;

            return new(totalLength, totalLength);
        }
    }
}