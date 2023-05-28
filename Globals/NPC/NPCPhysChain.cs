using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class NPCPhysChain : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private static readonly int maxChains = 6;
        private int frameCounter;

        /// <summary>
        /// The physchain.
        /// </summary>
        public IPhysChain[] npcPhysChain;

        /// <summary>
        /// The positions of all segments of the chain
        /// </summary>
        public Vector3[][] bodyPhysChainPositions;

        public Vector2[] npcPhysChainOffset;

        public int[] npcPhysChainDir;
        public override void OnSpawn(Terraria.NPC npc, IEntitySource source)
        {
            npcPhysChain ??= new IPhysChain[maxChains];
            bodyPhysChainPositions ??= new Vector3[maxChains][];
            npcPhysChainOffset ??= new Vector2[maxChains];
            npcPhysChainDir ??= new int[maxChains];
        }
        public override void ResetEffects(Terraria.NPC npc)
        {
            if (npcPhysChain != null)
            {
                for (int i = 0; i < npcPhysChain.Length; i++)
                {
                    if (npcPhysChain[i] == null)
                    { ResetPhysics(i); }
                    else
                    {
                        if (bodyPhysChainPositions[i].Length != npcPhysChain[i].NumberOfSegments)
                        {
                            bodyPhysChainPositions[i] = new Vector3[npcPhysChain[i].NumberOfSegments];
                        }
                    }
                }
            }
        }

        private void ResetPhysics(int chain)
        {
            bodyPhysChainPositions[chain] = Array.Empty<Vector3>();
        }

        public override bool PreDraw(Terraria.NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npcPhysChain != null)
            {
                for (int i = 0; i < npcPhysChain.Length; i++)
                {
                    if (npcPhysChain[i] != null)
                    {
                        // Get npc entities
                        NPCPhysChain globalNPC = npc.GetGlobalNPC<NPCPhysChain>();

                        // Get chain type
                        IPhysChain segType = globalNPC.npcPhysChain[i];

                        // Position, from waist
                        Vector2 anchor = NPCChainHelper.GetNPCDrawAnchor(npcPhysChainOffset[i], npc);
                        int dir = globalNPC.npcPhysChainDir[i]; //npc.direction;
                        float gravDir = -1f;
                        float scale = 1f;
                        anchor = NPCChainHelper.SetSegmentAnchor(anchor, segType, dir, gravDir, scale, true);

                        Color rendercolor = Color.White;

                        if (!Main.gameMenu)
                        {
                            Point lightOrigin = new((int)((npc.Center.X - 16 * dir) / 16), (int)(npc.Center.Y / 16));
                            rendercolor = npc.GetAlpha(Lighting.GetColor(lightOrigin.X, lightOrigin.Y, rendercolor));
                        }

                        if (npcPhysChain[i].MaxFrames > 1)
                            NPCChainHelper.DrawSegmentsAnimated(spriteBatch, npc, globalNPC.Mod, segType, ref globalNPC.bodyPhysChainPositions[i], anchor, dir, gravDir, 1f, npcPhysChain[i].Shader, rendercolor, ref frameCounter, npcPhysChain[i].MaxFrames, npcPhysChain[i].FrameCounterMax);
                        else
                            NPCChainHelper.DrawSegments(spriteBatch, npc, globalNPC.Mod, segType, ref globalNPC.bodyPhysChainPositions[i], anchor, dir, gravDir, 1f, npcPhysChain[i].Shader, rendercolor);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Modifies the chain externally
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="physChain"></param>
        /// <param name="segments"></param>
        /// <param name="anchor"></param>
        /// 
        public static void ModifyChainPhysics(Terraria.NPC npc, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, Vector2 force)
        {
            bool staticDisplay = Main.gameMenu;

            // Manage trailing/snapping
            Vector2 parentPos = anchor;
            float time = Main.gameMenu ? 0 : Main.GlobalTimeWrappedHourly;
            for (int i = 0; i < segments.Length; i++)
            {
                // Either from current pos, or if in menu just force parent
                Vector2 currentPos = staticDisplay ? parentPos : new Vector2(segments[i].X, segments[i].Y);

                // Apply force
                currentPos += ModifyForce(npc, i, time, segments.Length, force);

                // get angle to parent node
                float angle = (float)Math.Atan2(
                    parentPos.Y - currentPos.Y,
                    parentPos.X - currentPos.X);

                // snap to length, using angle
                Vector2 angleVec = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                currentPos = parentPos - angleVec * physChain.Length(i);

                // move work
                segments[i].X = currentPos.X;
                segments[i].Y = currentPos.Y;
                segments[i].Z = angle;

                // Set new anchor point at the tip
                parentPos = currentPos;
            }
        }

        public static Vector2 ModifyForce(Terraria.NPC npc, int index, float time, int segmentNum, Vector2 forceMod)
        {
            Vector2 force = new();

            if (!Main.gameMenu)
            {
                force.Y += forceMod.Y;
                force.X += forceMod.X;
            }
            return force;
        }

        public static void ModifySegmentPhysics(Terraria.NPC npc, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, Vector2 force)
        {

        }
    }

    public static class NPCChainHelper
    {
        /// <summary>
        /// Draw each segment of a chain to a player's drawData, using the chain positions
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="mod"></param>
        /// <param name="physChain"></param>
        /// <param name="chainPositions"></param>
        /// <param name="anchor"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="shader"></param>
        /// <param name="chaincolor"></param>
        public static void DrawSegments(SpriteBatch spriteBatch, Terraria.NPC npc, Mod mod, IPhysChain physChain, ref Vector3[] chainPositions, Vector2 anchor, int dir, float gravDir, float scale, int shader, Color chaincolor)
        {
            // Run physics on each segment node
            int segmentLength = RunSegmentPhysics(npc, physChain, ref chainPositions, anchor, dir, gravDir, scale,
                Main.gameMenu);

            // Input textures in reverse (tip to base)
            Texture2D texture = physChain.GetTexture(mod);
            Texture2D glowMask = null;
            if (physChain.HasGlowmask)
                glowMask = physChain.GetGlowmaskTexture(mod);
            for (int i = segmentLength - 1; i >= 0; i--)
            {
                Vector2 drawPos;
                // Base should pixel snap to match player position
                if (i == 0)
                { drawPos = new Vector2((int)(chainPositions[i].X), (int)chainPositions[i].Y); }
                else
                { drawPos = new Vector2(chainPositions[i].X - 0.5f, chainPositions[i].Y); }

                Vector2 origin = physChain.OriginOffset(i);

                // Flip if facing left, invert if upside down
                SpriteEffects spriteEffect = SpriteEffects.None;
                if (dir < 0 == gravDir > 0)
                {
                    spriteEffect = SpriteEffects.FlipVertically;
                    origin.Y *= -1f;
                }

                // Draw it!
                Rectangle frame = physChain.GetSourceRect(texture, i);
                if (shader != 0)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                }
                spriteBatch.Draw(texture, drawPos - Main.screenPosition, frame, physChain.Glow ? Color.White : chaincolor, chainPositions[i].Z, frame.Size() / 2 + origin, 1f, spriteEffect, 0);
                if (physChain.HasGlowmask)
                {
                    if (physChain.GlowmaskShader != 0)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        GameShaders.Armor.ApplySecondary(physChain.GlowmaskShader, Main.player[Main.myPlayer], null);
                    }
                    spriteBatch.Draw(glowMask, drawPos - Main.screenPosition, frame, Color.White, chainPositions[i].Z, frame.Size() / 2 + origin, 1f, spriteEffect, 0);
                }
                if (shader != 0 || physChain.GlowmaskShader != 0)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
        }
        private static int OnFrame;
        public static void DrawSegmentsAnimated(SpriteBatch spriteBatch, Terraria.NPC npc, Mod mod, IPhysChain physChain, ref Vector3[] chainPositions, Vector2 anchor, int dir, float gravDir, float scale, int shader, Color chaincolor, ref int frameCounter, int frames = 0, int frameCounterMax = 5)
        {
            // Run physics on each segment node
            int segmentLength = RunSegmentPhysics(npc, physChain, ref chainPositions, anchor, dir, gravDir, scale,
                Main.gameMenu);

            // Input textures in reverse (tip to base)
            Texture2D texture = physChain.GetTexture(mod);
            Texture2D glowMask = null;
            if (physChain.HasGlowmask)
                glowMask = physChain.GetGlowmaskTexture(mod);
            for (int i = segmentLength - 1; i >= 0; i--)
            {
                Vector2 drawPos;
                // Base should pixel snap to match player position
                if (i == 0)
                { drawPos = new Vector2((int)(chainPositions[i].X), (int)chainPositions[i].Y); }
                else
                { drawPos = new Vector2(chainPositions[i].X - 0.5f, chainPositions[i].Y); }

                Vector2 origin = physChain.OriginOffset(i);

                // Flip if facing left, invert if upside down
                SpriteEffects spriteEffect = SpriteEffects.None;
                if (dir < 0 == gravDir > 0)
                {
                    spriteEffect = SpriteEffects.FlipVertically;
                    origin.Y *= -1f;
                }

                // Draw it!
                Rectangle frame = physChain.GetSourceRect(texture, i);
                if (frameCounter++ >= frameCounterMax * physChain.NumberOfSegments)
                {
                    frameCounter = 0;
                    OnFrame += frame.Height;
                    if (OnFrame >= frames * frame.Height)
                        OnFrame = 0;
                }
                frame.Y += OnFrame;
                if (shader != 0)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                }
                spriteBatch.Draw(texture, drawPos - Main.screenPosition, frame, physChain.Glow ? Color.White : chaincolor, chainPositions[i].Z, frame.Size() / 2 + origin, 1f, spriteEffect, 0);
                if (physChain.HasGlowmask)
                {
                    if (physChain.GlowmaskShader != 0)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        GameShaders.Armor.ApplySecondary(physChain.GlowmaskShader, Main.player[Main.myPlayer], null);
                    }
                    spriteBatch.Draw(glowMask, drawPos - Main.screenPosition, frame, Color.White, chainPositions[i].Z, frame.Size() / 2 + origin, 1f, spriteEffect, 0);
                }
                if (shader != 0 || physChain.GlowmaskShader != 0)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
        }

        /// <summary>
        /// Attach chains with offsets and apply forces to each segment according to their force method.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="physChain"></param>
        /// <param name="segments"></param>
        /// <param name="anchor"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="staticDisplay"></param>
        /// <returns></returns>
        public static int RunSegmentPhysics(Terraria.NPC npc, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, int dir, float gravDir, float scale, bool staticDisplay)
        {
            // Manage trailing/snapping
            Vector2 parentPos = anchor;
            float time = Main.gameMenu ? 0 : Main.GlobalTimeWrappedHourly;
            for (int i = 0; i < segments.Length; i++)
            {
                // Either from current pos, or if in menu just force parent
                Vector2 currentPos = staticDisplay ? parentPos : new Vector2(segments[i].X, segments[i].Y);

                // Apply force
                currentPos += physChain.Force(null, i, dir, gravDir, time, npc);

                // get angle to parent node
                float angle = (float)Math.Atan2(
                    parentPos.Y - currentPos.Y,
                    parentPos.X - currentPos.X);

                // snap to length, using angle
                Vector2 angleVec = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                currentPos = parentPos - angleVec * physChain.Length(i) * scale;

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
        /// <param name="npcPosition"></param>
        /// <param name="physChain"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="backOffset"></param>
        /// <returns></returns>
        public static Vector2 SetSegmentAnchor(Vector2 npcPosition, IPhysChain physChain, int dir, float gravDir, float scale, bool backOffset = false)
        {
            // Set the anchor offset and direction
            Vector2 anchorOffset = physChain.AnchorOffset;
            npcPosition.X += dir; // Weird offset

            if (gravDir < 0)
            { anchorOffset.Y += 6 * scale; }

            if (backOffset)
            { anchorOffset += new Vector2(6, 0) * scale; }

            return npcPosition + new Vector2(
                (anchorOffset.X) * dir,
                (anchorOffset.Y) * gravDir * scale
            );
        }

        /// <summary>
        /// Get the anchor position of the body.
        /// </summary>
        /// <param name="drawOffset">Offset from the NPC's center</param>
        /// <param name="npc">The NPC</param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector2 GetNPCDrawAnchor(Vector2 drawOffset, Terraria.NPC npc, float scale = 1f)
        {
            Vector2 anchor = npc.Center + drawOffset;
            return anchor;
        }
    }
}