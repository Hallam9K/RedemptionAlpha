using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Globals.Player
{
    public class PhysChainPlayerLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if ((drawInfo.drawPlayer.merman || drawInfo.drawPlayer.forceMerman) && !drawInfo.drawPlayer.hideMerman)
            {
                return false;
            }

            return drawInfo.drawPlayer.GetModPlayer<DrawEffectsPlayer>().bodyPhysChain != null;
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.NeckAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            // Get player entities
            Terraria.Player player = drawInfo.drawPlayer;
            DrawEffectsPlayer mPlayer = player.GetModPlayer<DrawEffectsPlayer>();

            // Get chain type
            IPhysChain segType = mPlayer.bodyPhysChain;

            // Position, from waist
            Vector2 anchor = RenderLayerHelper.GetPlayerBodyDrawAnchor(drawInfo.Position, player);
            int dir = player.direction;
            float gravDir = player.gravDir;
            float scale = 1f;
            anchor = RenderLayerHelper.SetSegmentAnchor(anchor, segType, dir, gravDir, scale, RenderLayerHelper.GetBodyFrameOffset(player), true);

            // Calculate colour and shader from the item's dye
            Color baseColour = Color.White;
            int renderShader = mPlayer.cPhysChain;

            Color renderColour = segType.GetColor(drawInfo, baseColour);

            if (!Main.gameMenu)
            {
                Point lightOrigin = new((int)((player.Center.X - 16 * dir) / 16), (int)(player.Center.Y / 16));
                renderColour = player.GetImmuneAlpha(Lighting.GetColor(lightOrigin.X, lightOrigin.Y, renderColour), 0f);
            }

            PlayerPhysChainTextureContent physChainContent = PlayerPhysChainTextureContent.PlayerPhysChain;

            physChainContent.ProvideInfo(drawInfo);
            physChainContent.Request();
            if (physChainContent.IsReady)
            {
                RenderTarget2D target = physChainContent.GetTarget();
                Vector2 drawPosition = anchor - Main.screenPosition;
                DrawData data = new(
                    target,
                    drawPosition,
                    null,
                    renderColour,
                    0f,
                    target.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0)
                {
                    shader = renderShader,
                    ignorePlayerRotation = true
                };
                drawInfo.DrawDataCache.Add(data);
            }
        }
    }
    public static class RenderLayerHelper
    {
        /// <summary>
        /// Draw each segment of a chain to a player's drawData, using the chain positions
        /// </summary>
        /// <param name="drawInfo"></param>
        public static List<DrawData> DrawSegments(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player player = drawInfo.drawPlayer;
            DrawEffectsPlayer drawEffectsPlayer = player.GetModPlayer<DrawEffectsPlayer>();
            IPhysChain physChain = drawEffectsPlayer.bodyPhysChain;
            Vector3[] chainOffsets = drawEffectsPlayer.bodyPhysChainPositions;
            Vector2 anchor = GetPlayerBodyDrawAnchor(drawInfo.Position, player);
            anchor = SetSegmentAnchor(anchor, physChain, player.direction, player.gravDir, 1f, GetBodyFrameOffset(player), true);

            List<DrawData> dataSet = new();

            // Don't draw if there's nothing to draw.
            if (chainOffsets.Length == 0)
                return dataSet;

            // Run physics on each segment node
            int segmentLength = RunSegmentPhysics(player, physChain, ref chainOffsets, anchor, player.direction, player.gravDir, 1f,
                Main.gameMenu);

            // The first segment should be drawn on the anchor.
            // If it isn't on the anchor, offset it so that it is, and offset every other segment to follow it.
            // This fixes chains mysteriously offsetting when the game is paused.
            Vector2 anchorOffset = new Vector2(chainOffsets[0].X, chainOffsets[0].Y) - anchor;

            // Input textures in reverse (tip to base)
            Mod mod = Redemption.Instance;
            Texture2D texture = physChain.GetTexture(mod);
            for (int i = segmentLength - 1; i >= 0; i--)
            {
                Vector2 drawPos;
                // Base should pixel snap to match player position
                if (i == 0)
                { drawPos = new Vector2((int)chainOffsets[i].X, (int)chainOffsets[i].Y); }
                else
                { drawPos = new Vector2(chainOffsets[i].X - 0.5f, chainOffsets[i].Y); }

                Vector2 origin = physChain.OriginOffset(i);

                // Flip if facing left, invert if upside down
                SpriteEffects spriteEffect = SpriteEffects.None;
                if ((player.direction < 0) == (player.gravDir > 0))
                {
                    //spriteEffect = SpriteEffects.FlipVertically;
                    origin.Y *= -1f;
                }
                if (player.direction < 0)
                {
                    spriteEffect = SpriteEffects.FlipVertically;
                }

                // Draw it!
                Rectangle frame = physChain.GetSourceRect(texture, i);
                DrawData data = new(
                    texture,
                    drawPos - anchor - anchorOffset,
                    frame,
                    Color.White,
                    chainOffsets[i].Z, frame.Size() / 2 + origin, 1f, spriteEffect, 0);
                dataSet.Add(data);
            }

            return dataSet;
        }

        /// <summary>
        /// Attach chains with offsets and apply forces to each segment according to their force method.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="physChain"></param>
        /// <param name="segments"></param>
        /// <param name="anchor"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="staticDisplay"></param>
        /// <returns></returns>
        public static int RunSegmentPhysics(Terraria.Player player, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, int dir, float gravDir, float scale, bool staticDisplay)
        {
            if (Main.gamePaused)
            {
                return segments.Length;
            }

            // Manage trailing/snapping
            Vector2 parentPos = anchor;
            float time = Main.gameMenu ? 0 : Main.GlobalTimeWrappedHourly;
            for (int i = 0; i < segments.Length; i++)
            {
                if (!Main.hasFocus || Main.gamePaused)
                {
                    break;
                }

                // Either from current pos, or if in menu just force parent
                Vector2 currentPos = staticDisplay ? parentPos : new Vector2(segments[i].X, segments[i].Y);

                // Apply force
                currentPos += physChain.Force(player, i, dir, gravDir, time);

                // get angle to parent node
                float angle = (parentPos - currentPos).ToRotation();

                // snap to length, using angle
                Vector2 angleVec = angle.ToRotationVector2();

                // Ever so slightly reduce the distance between segments so that they appear more connected.
                currentPos = parentPos - (angleVec * (0.95f * physChain.Length(i)) * scale);

                // move work
                segments[i].X = currentPos.X;
                segments[i].Y = currentPos.Y;
                segments[i].Z = angle;

                // Set new anchor point at the tip
                parentPos = currentPos;
            }

            return segments.Length;
        }

        /// <summary>
        /// Get the anchor of the current chain segment.
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <param name="physChain"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="frameOffsetY"></param>
        /// <param name="backOffset"></param>
        /// <returns></returns>
        public static Vector2 SetSegmentAnchor(Vector2 playerPosition, IPhysChain physChain, int dir, float gravDir, float scale, int frameOffsetY, bool backOffset = false)
        {
            // Set the anchor offset and direction
            Vector2 anchorOffset = physChain.AnchorOffset;
            playerPosition.X += dir; // Weird offset

            if (gravDir < 0)
            { anchorOffset.Y += 6 * scale; }

            if (backOffset)
            { anchorOffset += new Vector2(6, 0) * scale; }

            return playerPosition + new Vector2(
                anchorOffset.X * dir,
                (anchorOffset.Y + frameOffsetY) * gravDir * scale
            );
        }

        /// <summary>
        /// Pixel offset from walking, from the waist upward.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetBodyFrameOffset(Terraria.Player player)
        {
            // body bob
            int frameOffsetY = player.bodyFrame.Y / player.bodyFrame.Height;
            frameOffsetY = frameOffsetY switch
            {
                7 or 8 or 9 or 14 or 15 or 16 => -2,
                _ => 0,
            };
            return frameOffsetY;
        }

        /// <summary>
        /// Get the anchor position of the body.
        /// </summary>
        /// <param name="drawPosition">The drawInfo position</param>
        /// <param name="player">The player</param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector2 GetPlayerBodyDrawAnchor(Vector2 drawPosition, Terraria.Player player, float scale = 1f)
        {
            // Position, with head offset
            Vector2 anchor = drawPosition + player.bodyPosition + player.DefaultSize / 2;
            if (player.mount != null && player.mount.Active) // Mount offset fix
            { anchor.Y += (player.Size.Y - player.DefaultSize.Y) * scale; }

            return anchor;
        }
    }
}