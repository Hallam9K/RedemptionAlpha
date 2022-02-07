using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class ParticleManager : ModSystem
    {
        private static List<Particle> particles;
        private int screenX;
        private int screenY;
        public override void OnModLoad()
        {
            Redemption.Particles = this;
            particles = new List<Particle>();
            screenX = Main.screenWidth;
            screenY = Main.screenHeight;
        }
        public override void Unload()
        {
            Redemption.Particles = null;
            particles.Clear();
            particles = null;
        }
        public static void Dispose()
        {
            particles.Clear();
        }
        public static void PreUpdate(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles?.Count; i++)
            {
                particles[i].PreAI();
            }
        }
        public static void Update(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles?.Count; i++)
            {
                Particle particle = particles[i];
                if (particle.tileCollide)
                {
                    if (!Collision.SolidCollision(particles[i].position + (new Vector2(particles[i].width / 2f, particles[i].height / 2f) * particles[i].scale), 1, 1))
                    {
                        // Apply gravity.
                        particle.velocity.Y += particles[i].gravity;
                        // Apply velocity to position.
                        particle.position += particles[i].velocity;
                        UpdateOldPos(particle);
                    }
                }
                else
                {
                    // Apply gravity.
                    particle.velocity.Y += particles[i].gravity;
                    // Apply velocity to position.
                    particle.position += particles[i].velocity;
                    UpdateOldPos(particle);
                }

                // Run AI.
                particle.AI();
                // Draw particle.
                bool draw = particle.PreDraw(spriteBatch, Main.screenPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
                if (draw)
                {
                    particle.Draw(spriteBatch, Main.screenPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
                }
                // Time left check.
                if (particle.timeLeft-- == 0 || !particles[i].active)
                {
                    // Do death action.
                    particle.DeathAction?.Invoke();
                    // Deactivate particle.
                    particles.RemoveAt(i);
                    particles.TrimExcess();
                }
            }
        }
        public static void PostUpdate(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles?.Count; i++)
            {
                particles[i].PostAI();
                particles[i].PostDraw(spriteBatch, Main.screenPosition, Lighting.GetColor((int)(particles[i].position.X / 16), (int)(particles[i].position.Y / 16)));
            }
        }
        public static void NewParticle(Vector2 Position, Vector2 Velocity, Particle Type, Color Color, float Scale, float AI0 = 0, float AI1 = 0, float AI2 = 0, float AI3 = 0, float AI4 = 0, float AI5 = 0, float AI6 = 0, float AI7 = 0)
        {
            if (Type.texture == null)
                throw new NullReferenceException();
            Type.position = Position;
            Type.velocity = Velocity;
            Type.color = Color;
            Type.scale = Scale;
            Type.active = true;
            Type.ai = new float[] { AI0, AI1, AI2, AI3, AI4, AI5, AI6, AI7 };
            if (particles?.Count > 6000)
                particles.TrimExcess();
            if (particles?.Count < 6000)
            {
                Type.SpawnAction?.Invoke();
                particles?.Add(Type);
            }
            RedeDetours.NewParticle(Position, Velocity, Type, Color, Scale, AI0, AI1, AI2, AI3, AI4, AI5, AI6, AI7);
        }
        public static void UpdateOldPos(Particle particle)
        {
            if (particle.oldPosLength > 0)
            {
                for (int i = particle.oldPos.Length - 1; i >= 0; i--)
                {
                    particle.oldPos[i] = i == 0 ? particle.position : particle.oldPos[i - 1];
                    if (i == 0)
                        break;
                }
            }
            if (particle.oldRotLength > 0)
            {
                for (int i = particle.oldRot.Length - 1; i >= 0; i--)
                {
                    particle.oldRot[i] = i == 0 ? particle.rotation : particle.oldRot[i - 1];
                    if (i == 0)
                        break;
                }
            }
            if (particle.oldVelLength > 0)
            {
                for (int i = particle.oldVel.Length - 1; i >= 0; i--)
                {
                    particle.oldVel[i] = i == 0 ? particle.velocity : particle.oldVel[i - 1];
                    if (i == 0)
                        break;
                }
            }
        }
    }
}
