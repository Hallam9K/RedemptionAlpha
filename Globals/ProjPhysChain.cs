using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class ProjPhysChain : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private static readonly int maxChains = 6;
        private int frameCounter;

        /// <summary>
        /// The physchain.
        /// </summary>
        public IPhysChain[] projPhysChain;

        /// <summary>
        /// The positions of all segments of the chain
        /// </summary>
        public Vector3[][] bodyPhysChainPositions;

        public Vector2[] projPhysChainOffset;

        public int[] projPhysChainDir;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            projPhysChain ??= new IPhysChain[maxChains];
            bodyPhysChainPositions ??= new Vector3[maxChains][];
            projPhysChainOffset ??= new Vector2[maxChains];
            projPhysChainDir ??= new int[maxChains];
        }
        public override bool PreAI(Projectile projectile)
        {
            if (projPhysChain != null)
            {
                for (int i = 0; i < projPhysChain.Length; i++)
                {
                    if (projPhysChain[i] == null)
                    { ResetPhysics(i); }
                    else
                    {
                        if (bodyPhysChainPositions[i].Length != projPhysChain[i].NumberOfSegments)
                        {
                            bodyPhysChainPositions[i] = new Vector3[projPhysChain[i].NumberOfSegments];
                        }
                    }
                }
            }
            return base.PreAI(projectile);
        }

        private void ResetPhysics(int chain)
        {
            bodyPhysChainPositions[chain] = Array.Empty<Vector3>();
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projPhysChain != null)
            {
                for (int i = 0; i < projPhysChain.Length; i++)
                {
                    if (projPhysChain[i] != null)
                    {
                        // Get projectile entities
                        ProjPhysChain globalProj = projectile.GetGlobalProjectile<ProjPhysChain>();

                        // Get chain type
                        IPhysChain segType = globalProj.projPhysChain[i];

                        // Position, from waist
                        Vector2 anchor = ProjectileChainHelper.GetProjDrawAnchor(projPhysChainOffset[i], projectile);
                        int dir = globalProj.projPhysChainDir[i]; //projectile.direction;
                        float gravDir = -1f;
                        float scale = 1f;
                        anchor = ProjectileChainHelper.SetSegmentAnchor(anchor, segType, dir, gravDir, scale, true);

                        Color rendercolor = Color.White;

                        if (!Main.gameMenu)
                        {
                            Point lightOrigin = new((int)((projectile.Center.X - 16 * dir) / 16), (int)(projectile.Center.Y / 16));
                            rendercolor = projectile.GetAlpha(Lighting.GetColor(lightOrigin.X, lightOrigin.Y, rendercolor));
                        }

                        if (projPhysChain[i].MaxFrames > 1)
                            ProjectileChainHelper.DrawSegmentsAnimated(Main.spriteBatch, projectile, globalProj.Mod, segType, ref globalProj.bodyPhysChainPositions[i], anchor, dir, gravDir, 1f, projPhysChain[i].Shader, rendercolor, ref frameCounter, projPhysChain[i].MaxFrames, projPhysChain[i].FrameCounterMax);
                        else
                            ProjectileChainHelper.DrawSegments(Main.spriteBatch, projectile, globalProj.Mod, segType, ref globalProj.bodyPhysChainPositions[i], anchor, dir, gravDir, 1f, projPhysChain[i].Shader, rendercolor);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Modifies the chain externally
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="physChain"></param>
        /// <param name="segments"></param>
        /// <param name="anchor"></param>
        /// 
        public static void ModifyChainPhysics(Projectile proj, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, Vector2 force)
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
                currentPos += ModifyForce(proj, i, time, segments.Length, force);

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

        public static Vector2 ModifyForce(Projectile proj, int index, float time, int segmentNum, Vector2 forceMod)
        {
            Vector2 force = new();

            if (!Main.gameMenu)
            {
                force.Y += forceMod.Y;
                force.X += forceMod.X;
            }
            return force;
        }

        public static void ModifySegmentPhysics(Projectile proj, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, Vector2 force)
        {

        }
    }

    public static class ProjectileChainHelper
    {
        /// <summary>
        /// Draw each segment of a chain to a player's drawData, using the chain positions
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="mod"></param>
        /// <param name="physChain"></param>
        /// <param name="chainPositions"></param>
        /// <param name="anchor"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="shader"></param>
        /// <param name="chaincolor"></param>
        public static void DrawSegments(SpriteBatch spriteBatch, Projectile proj, Mod mod, IPhysChain physChain, ref Vector3[] chainPositions, Vector2 anchor, int dir, float gravDir, float scale, int shader, Color chaincolor)
        {
            // Run physics on each segment node
            int segmentLength = RunSegmentPhysics(proj, physChain, ref chainPositions, anchor, dir, gravDir, scale,
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
        public static void DrawSegmentsAnimated(SpriteBatch spriteBatch, Projectile proj, Mod mod, IPhysChain physChain, ref Vector3[] chainPositions, Vector2 anchor, int dir, float gravDir, float scale, int shader, Color chaincolor, ref int frameCounter, int frames = 0, int frameCounterMax = 5)
        {
            // Run physics on each segment node
            int segmentLength = RunSegmentPhysics(proj, physChain, ref chainPositions, anchor, dir, gravDir, scale,
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
        /// <param name="proj"></param>
        /// <param name="physChain"></param>
        /// <param name="segments"></param>
        /// <param name="anchor"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="staticDisplay"></param>
        /// <returns></returns>
        public static int RunSegmentPhysics(Projectile proj, IPhysChain physChain, ref Vector3[] segments, Vector2 anchor, int dir, float gravDir, float scale, bool staticDisplay)
        {
            // Manage trailing/snapping
            Vector2 parentPos = anchor;
            float time = Main.gameMenu ? 0 : Main.GlobalTimeWrappedHourly;
            for (int i = 0; i < segments.Length; i++)
            {
                // Either from current pos, or if in menu just force parent
                Vector2 currentPos = staticDisplay ? parentPos : new Vector2(segments[i].X, segments[i].Y);

                // Apply force
                currentPos += physChain.Force(null, i, dir, gravDir, time, null, proj);

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
        /// <param name="projPosition"></param>
        /// <param name="physChain"></param>
        /// <param name="dir"></param>
        /// <param name="gravDir"></param>
        /// <param name="scale"></param>
        /// <param name="backOffset"></param>
        /// <returns></returns>
        public static Vector2 SetSegmentAnchor(Vector2 projPosition, IPhysChain physChain, int dir, float gravDir, float scale, bool backOffset = false)
        {
            // Set the anchor offset and direction
            Vector2 anchorOffset = physChain.AnchorOffset;
            projPosition.X += dir; // Weird offset

            if (gravDir < 0)
            { anchorOffset.Y += 6 * scale; }

            if (backOffset)
            { anchorOffset += new Vector2(6, 0) * scale; }

            return projPosition + new Vector2(
                (anchorOffset.X) * dir,
                (anchorOffset.Y) * gravDir * scale
            );
        }

        /// <summary>
        /// Get the anchor position of the body.
        /// </summary>
        /// <param name="drawOffset">Offset from the NPC's center</param>
        /// <param name="proj">The NPC</param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector2 GetProjDrawAnchor(Vector2 drawOffset, Projectile proj, float scale = 1f)
        {
            Vector2 anchor = proj.Center + drawOffset;
            return anchor;
        }
    }
}