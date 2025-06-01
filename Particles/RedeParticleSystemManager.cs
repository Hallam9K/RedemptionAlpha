using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Core.V3;
using ParticleLibrary.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SystemVector2 = System.Numerics.Vector2;
using static Redemption.Particles.ParticleBehaviors;
using System;
using System.Collections.Generic;

namespace Redemption.Particles
{
    public class RedeParticleManager : ModSystem
    {
        public static FastNoiseLite Perlin { get; private set; }
        //public static ParticleBuffer<GlowParticleBehavior> ExampleParticleBuffer { get; private set; }

        private static ParticleBuffer<GlowParticleBehavior> _glowParticleBuffer;
        private static ParticleBuffer<GlowParticle3Behavior> _glowParticle3Buffer;
        private static ParticleBuffer<GlowParticle4Behavior> _glowParticle4Buffer;
        private static ParticleBuffer<WhiteFlareParticleBehavior> _whiteFlareParticleBuffer;
        private static ParticleBuffer<GlowParticleBehavior> _glowParticleAdditiveBuffer;

        public static Dictionary<Layer, ParticleCollection> _glowParticleCollections;
        public static Dictionary<Layer, ParticleCollection> _soullessParticleCollections;

        private static ParticleBuffer<BlackholeParticleBehavior> _blackholeParticleBuffer;
        private static ParticleBuffer<BigFlareParticleBehavior> _shadowParticleBuffer;
        private static ParticleBuffer<SlashParticleBehavior> _slashParticleBuffer;
        private static ParticleBuffer<SlashAltParticleBehavior> _slashAltParticleBuffer;
        private static ParticleBuffer<LaserParticleBehavior> _laserParticleBuffer;
        private static ParticleBuffer<PulseParticleBehavior> _pulseParticleBuffer;
        private static ParticleBuffer<RainbowParticleBehavior> _rainbowParticleBuffer;
        private static ParticleBuffer<SpeedParticleBehavior> _speedParticleBuffer;

        public static Color[] emberColors = [new(240, 149, 46, 0), new(187, 63, 25, 0), new(131, 23, 37, 0)];
        public static Color[] blueEmberColors = [new(30, 182, 228, 0), new(20, 91, 183, 0), new(13, 26, 139, 0)];
        public static Color[] purpleEmberColors = [new(208, 50, 232, 0), new(176, 43, 183, 0), new(95, 26, 141, 0)];
        public static Color[] lightningColors = [new(255, 255, 255, 0), new(161, 255, 253, 0), new(40, 186, 242, 0)];
        public static Color[] yellowLightningColors = [new(255, 255, 255, 0), new(255, 255, 174, 0), new(255, 189, 69, 0)];
        public static Color[] redColors = [new(255, 146, 135, 0), new(223, 62, 55, 0), new(150, 20, 54, 0)];
        public static Color[] purpleColors = [new(158, 57, 248, 0), new(158, 57, 248, 0), new(104, 45, 237, 0)];
        public static Color[] goldColors = [new(255, 182, 49, 0), new(255, 182, 49, 0), new(255, 105, 43, 0)];
        public static Color[] greenColors = [new(186, 255, 185, 0), new(76, 240, 107, 0), new(23, 165, 107, 0)];
        public static Color[] spiritColors = [new(238, 251, 255, 0), new(195, 231, 231, 0), new(87, 189, 229, 0)];
        public static Color[] infernalSpiritColors = [new(252, 194, 5, 0), new(255, 162, 17, 0), new(242, 55, 19, 0)];
        public static Color[] redThrusterColors = [new(235, 255, 255, 0), new(255, 146, 135, 0), new(150, 20, 54, 0)];
        public static Color[] blueThrusterColors = [new(200, 255, 255, 0), new(94, 221, 255, 0), new(106, 121, 124, 0)];
        public static Color[] soulColors = [new(255, 255, 255, 0), new(238, 251, 255, 0), new(195, 231, 244, 0)];

        public override void PostSetupContent()
        {
            Perlin = new(0xAAAAAA);
            Perlin.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        }
        public override void OnModLoad()
        {
            if (Main.netMode is NetmodeID.Server)
                return;

            //ExampleParticleBuffer = new(512);
            //ParticleManagerV3.RegisterUpdatable(ExampleParticleBuffer);
            //ParticleManagerV3.RegisterRenderable(Layer.BeforeSolidTiles, ExampleParticleBuffer);

            _glowParticleCollections = new()
            {
                {
                    Layer.BeforeDust,
                    new ParticleCollection()
              .Add(new ParticleBuffer<GlowParticleBehavior>(8192), Layer.BeforeDust)
              .Add(new ParticleBuffer<GlowParticle2Behavior>(2048), Layer.BeforeDust)
                },
                {
                    Layer.BeforeNPCs,
                    new ParticleCollection()
              .Add(new ParticleBuffer<GlowParticleBehavior>(8192), Layer.BeforeNPCs)
              .Add(new ParticleBuffer<GlowParticle2Behavior>(2048), Layer.BeforeNPCs)
                },
                {
                    Layer.BeforePlayers,
                    new ParticleCollection()
              .Add(new ParticleBuffer<GlowParticleBehavior>(8192), Layer.BeforePlayers)
              .Add(new ParticleBuffer<GlowParticle2Behavior>(2048), Layer.BeforePlayers)
                },
                {
                    Layer.BeforeSolidTiles,
                    new ParticleCollection()
              .Add(new ParticleBuffer<GlowParticleBehavior>(8192), Layer.BeforeSolidTiles)
              .Add(new ParticleBuffer<GlowParticle2Behavior>(2048), Layer.BeforeSolidTiles)
                }
            };

            _soullessParticleCollections = new()
            {
                { Layer.BeforeDust, new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeDust) },
                { Layer.BeforeProjectiles, new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeProjectiles) },
                { Layer.BeforeNPCs, new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeNPCs) },
                { Layer.BeforeItems, new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeItems) }
            };

            _glowParticleBuffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticleBuffer);

            _glowParticleAdditiveBuffer = new(512);
            _glowParticleAdditiveBuffer.SetBlendState(BlendState.Additive);
            ParticleManagerV3.RegisterUpdatable(_glowParticleAdditiveBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticleAdditiveBuffer);

            _glowParticle3Buffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticle3Buffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticle3Buffer);

            _glowParticle4Buffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticle4Buffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticle4Buffer);

            _whiteFlareParticleBuffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_whiteFlareParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _whiteFlareParticleBuffer);

            _blackholeParticleBuffer = new(256);
            ParticleManagerV3.RegisterUpdatable(_blackholeParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _blackholeParticleBuffer);

            _shadowParticleBuffer = new(2048);
            ParticleManagerV3.RegisterUpdatable(_shadowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeNPCs, _shadowParticleBuffer);

            _slashParticleBuffer = new(512);
            ParticleManagerV3.RegisterUpdatable(_slashParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _slashParticleBuffer);

            _slashAltParticleBuffer = new(512);
            ParticleManagerV3.RegisterUpdatable(_slashAltParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _slashAltParticleBuffer);

            _laserParticleBuffer = new(2048);
            ParticleManagerV3.RegisterUpdatable(_laserParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _laserParticleBuffer);

            _pulseParticleBuffer = new();
            _pulseParticleBuffer.SetBlendState(BlendState.Additive);
            ParticleManagerV3.RegisterUpdatable(_pulseParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _pulseParticleBuffer);

            _rainbowParticleBuffer = new(1024);
            ParticleManagerV3.RegisterUpdatable(_rainbowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _rainbowParticleBuffer);

            _speedParticleBuffer = new(1024);
            ParticleManagerV3.RegisterUpdatable(_speedParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _speedParticleBuffer);
        }

        #region QuadParticles
        // These just have the same behaviour and settings as the 2.0 QuadParticles, these aren't actual QuadParticles
        public static void CreateQuadParticle(Vector2 position, Vector2 velocity, Vector2 scale, Color color, Color endColor, int duration, float velChange = 1, float scaleChange = 1, float depth = 1, ParticleFlags style = ParticleFlags.Quad)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(128) * scale).ToNumerics();

            _glowParticle3Buffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration, (float)style, BitConverter.UInt32BitsToSingle(endColor.PackedValue), velChange, scaleChange));
        }
        public static void CreateQuadParticle2(Vector2 position, Vector2 velocity, Vector2 scale, Color color, Color endColor, int duration, float velChange = 1, float scaleChange = 1, float depth = 1, ParticleFlags style = ParticleFlags.Quad)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(64) * scale).ToNumerics();

            _glowParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration, (float)style, BitConverter.UInt32BitsToSingle(endColor.PackedValue), velChange, scaleChange));
        }
        #endregion

        public static void CreateShadowParticle(Vector2 position, Vector2 velocity, Vector2 scale, int duration, float velChange = 1, float scaleChange = 1)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(682) * scale).ToNumerics();
            Color endColor = new(0, 0, 0);

            _shadowParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, new(0, 0, 0), duration, (float)ParticleFlags.Quad, BitConverter.UInt32BitsToSingle(endColor.PackedValue), velChange, scaleChange));
        }

        #region Glow Particles
        public static void CreateAdditiveGlowParticle(Vector2 position, Vector2 velocity, float scale, Color color, int duration, float velChange = .94f, ParticleFlags style = ParticleFlags.Basic)
        {
            CreateAdditiveGlowParticle(position.ToNumerics(), velocity.ToNumerics(), scale, color, duration, velChange, style);
        }
        public static void CreateAdditiveGlowParticle(SystemVector2 position, SystemVector2 velocity, float scale, Color color, int duration, float velChange = .94f, ParticleFlags style = ParticleFlags.Basic)
        {
            var Scale = new SystemVector2(64 * scale);

            for (int i = 0; i < 5; i++)
                _glowParticleAdditiveBuffer.Create(new ParticleInfo(position, velocity, 0, new SystemVector2(1.3f, 0.7f) * Scale, color, duration, (float)style, velChange));
        }
        public static void CreateGlowParticle(Vector2 position, Vector2 velocity, float scale, Color color, int duration, ParticleFlags style = ParticleFlags.Fading, float fadeAdd = .45f, Layer layer = Layer.BeforeDust)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, color.WithAlpha(0), duration, (float)style, fadeAdd),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, Color.White.WithAlpha(0), duration, (float)style, fadeAdd));
            }
        }
        public static void CreateGlowParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors, int duration, float velChange = 0.9f, float opacityChange = 1, float scaleChange = 0.9f, Layer layer = Layer.BeforeDust)
        {
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale * size);

            float flag = (float)(ParticleFlags.Custom | ParticleFlags.ColorTransition);

            Color bright = colors[0];
            Color mid = colors[1];
            Color dark = colors[2];
            var darkPacked = BitConverter.UInt32BitsToSingle(dark.PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, mid, duration, flag, darkPacked, velChange, opacityChange, scaleChange, 0),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, bright, duration, flag, darkPacked, velChange, opacityChange, scaleChange, 0));
            }
        }
        #endregion

        public static void CreateBlackholeParticle(Vector2 position, Vector2 velocity, float scale, Color color, int projID, int duration = 120)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(128 * scale);

            _blackholeParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * 0.2f, color, duration, 0, projID, 0));
            _blackholeParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * 0.25f, color, duration, 0, projID, 0));
        }

        #region Ember Particles
        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, Color brightColor, Color midColor, Color darkColor, int duration = 120, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust)
        {
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale * size);
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor, duration, (float)ParticleFlags.Ember, timeBeforeMoving, darkPacked),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, brightColor, duration, (float)ParticleFlags.Ember, timeBeforeMoving, darkPacked));
            }
        }
        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors, int duration = 120, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust)
        {
            CreateEmberParticle(position, velocity, scale, colors[0], colors[1], colors[2], duration, timeBeforeMoving, layer);
        }
        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust)
        {
            CreateEmberParticle(position, velocity, scale, emberColors, duration, timeBeforeMoving, layer);
        }
        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, Color brightColor, Color midColor, Color darkColor, int duration = 120, float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Scale = new SystemVector2(64 * scale * size);
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor, duration, (float)ParticleFlags.EmberBurst, darkPacked, velChange),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, brightColor, duration, (float)ParticleFlags.EmberBurst, darkPacked, velChange));
            }
        }
        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors, int duration = 120, float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            CreateEmberBurstParticle(position, velocity, scale, colors[0], colors[1], colors[2], duration, velChange, layer);
        }
        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120, float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            CreateEmberBurstParticle(position, velocity, scale, emberColors, duration, velChange, layer);
        }
        /*public static void CreateOldEmberParticle(Vector2 position, Vector2 velocity, float scale, Color brightColor, Color midColor, Color darkColor, int duration = 120, int timeBeforeMoving = 0)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Scale = new SystemVector2(64 * scale * size);
            uint darkPacked = (uint)BitConverter.UInt32BitsToSingle(darkColor.PackedValue);

            int timer = Main.rand.Next(50, 100);
            float speedX = Main.rand.NextFloat(4f, 9f);
            float multi = Main.rand.NextFloat(10f, 31f) / 200f;

            _glowParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, midColor, duration, (float)ParticleFlags.Ember, timeBeforeMoving, timer, speedX, multi, darkPacked));
            float pixelRatio = 1f / 64f;
            _glowParticle2Buffer.Create(new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, brightColor, duration, (float)ParticleFlags.Ember, timeBeforeMoving, timer, speedX, multi, darkPacked));
        }*/
        #endregion

        #region Spirit Particles
        public static void CreateSpiritParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors, int duration = 120, int timeBeforeMoving = 0, float depth = 1, float opacityScale = 1, Layer layer = Layer.BeforeDust)
        {
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale * size);

            Color brightColor = colors[0];
            Color midColor = colors[1];
            Color darkColor = colors[2];
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.PackedValue);

            float timer = Main.rand.Next(50, 100);
            float speedX = Main.rand.NextFloat(4f, 9f);
            float multi = Main.rand.NextFloat(10f, 31f) / 200f;

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, depth, midColor, duration, (float)ParticleFlags.Spirit, timeBeforeMoving, timer, speedX, multi, darkPacked, opacityScale),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, depth, brightColor, duration, (float)ParticleFlags.Spirit, timeBeforeMoving, timer, speedX, multi, darkPacked, opacityScale));
            }
        }
        public static void CreateSpiritParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120, int timeBeforeMoving = 0, float depth = 1, float opacityScale = 1, Layer layer = Layer.BeforeDust)
        {
            CreateSpiritParticle(position, velocity, scale, spiritColors, duration, timeBeforeMoving, depth, opacityScale, layer);
        }
        #endregion

        public static void CreateSoullessParticle(Vector2 position, Vector2 velocity, float scale, Color color, int duration = 20, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust, float depth = 1)
        {
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(22, 24) * scale * size;

            if (_soullessParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration, timeBeforeMoving, 1));
            }
        }
        public static void CreateSoullessParticle(Vector2 position, Vector2 velocity, float scale, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust, float depth = 1)
        {
            CreateSoullessParticle(position, velocity, scale, new Color(24, 28, 38), Main.rand.Next(10, 40), timeBeforeMoving, layer, depth);
        }

        public static void CreateLightningParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors, bool clean = false, Layer layer = Layer.BeforeDust)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            float size = Main.rand.NextFloat(2f, 4f) / 10f;
            int duration = Main.rand.Next(5, 7);
            if (clean)
            {
                size = 3f / 10f;
                duration = 6;
            }
            var Scale = new SystemVector2(64 * scale * size);

            Color brightColor = colors[0];
            Color midColor = colors[1];
            Color darkColor = colors[2];
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor, duration, (float)ParticleFlags.EmberBurst, darkPacked, 1),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, brightColor, duration, (float)ParticleFlags.EmberBurst, darkPacked, 1));
            }
        }

        public static void CreateLaserParticle(Vector2 position, Vector2 velocity, float scale, Color color, float squish = 5, int duration = 18)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(scale);

            _laserParticleBuffer.Create(new ParticleInfo(Position, SystemVector2.Zero, Velocity.ToRotation() + MathHelper.PiOver2, Scale, color, duration, 0, squish));
            _laserParticleBuffer.Create(new ParticleInfo(Position, SystemVector2.Zero, Velocity.ToRotation() + MathHelper.PiOver2, Scale, color, duration, 0, squish));
        }
        public static void CreateChargeParticle(float scale, Color color, int playerID, int projID, Vector2 startVector, float endDistance, float extension = 30, int duration = 11)
        {
            float startDistance = startVector.Length();
            Color a = Color.Multiply(color.WithAlpha(0), 1);

            float rotation = startVector.ToRotation() + 3.14f;
            float rotationGap = Main.projectile[projID].velocity.ToRotation() - rotation;

            _glowParticle4Buffer.Create(new ParticleInfo(default, SystemVector2.Zero, rotation, new SystemVector2(scale), a, duration, (float)ParticleFlags.Charge, 0, playerID, projID, startDistance, endDistance, extension, rotationGap));
            _glowParticle4Buffer.Create(new ParticleInfo(default, SystemVector2.Zero, rotation, new SystemVector2(scale), a, duration, (float)ParticleFlags.Charge, 0, playerID, projID, startDistance, endDistance, extension, rotationGap));
        }
        public static void CreatePulseParticle(Vector2 position, Vector2 velocity, float rotation, Vector2 scale, Color color, int intensity = 1, float scaleSpeed = 1, int duration = 6)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(200) * scale.ToNumerics();

            for (int i = 0; i < intensity; i++)
                _pulseParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale, color, duration, scaleSpeed));
        }

        public static void CreateRainbowParticle(Vector2 position, Vector2 velocity, float scale, Color color, float opacityScale = 1, int duration = 50)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            float rotation = Main.rand.NextFloat(0f, 360f);
            float flag = (float)ParticleFlags.Rainbow;

            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * 0.75f, color, duration, flag, opacityScale, rotation, 0));
            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * 0.75f, color, duration, flag, opacityScale, rotation, MathHelper.PiOver2));

            Scale = new SystemVector2(144) * scale;
            _whiteFlareParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, color, duration, flag, opacityScale, -1, 0));
            Scale = new SystemVector2(128) * scale;
            _glowParticle4Buffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * .3f * opacityScale, color, duration, flag, 0.5f, -1, 0));
        }
        public static void CreateRainbowParticle(Vector2 position, Vector2 velocity, float scale, float opacityScale = 1, int duration = 50)
        {
            CreateRainbowParticle(position, velocity, scale, Color.White, opacityScale, duration);
        }
        public static void CreateSimpleStarParticle(Vector2 position, Vector2 velocity, float scale, Color color, float opacityScale = 1, float deceleration = 0.98f, int duration = 50)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            float rotation = Main.rand.NextFloat(0f, 360f);
            float flag = (float)ParticleFlags.SimpleStar;

            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * new SystemVector2(0.75f, 0.5f), color * opacityScale, duration, flag, deceleration, rotation, 0));
            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * new SystemVector2(0.75f, 0.5f), color * opacityScale, duration, flag, deceleration, rotation, MathHelper.PiOver2));
        }
        public static void CreateSpeedParticle(Vector2 position, Vector2 velocity, float scale, Color color, float deAcc = 0.91f, int duration = 21, int earlyKillTimer = 0, float extension = 0)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();

            if (extension == 0)
                extension = Main.rand.NextFloat(28, 32);
            else
                extension *= Main.rand.NextFloat(0.95f, 1.05f);

            _speedParticleBuffer.Create(new ParticleInfo(Position, Velocity * 0.5f, Velocity.ToRotation(), new SystemVector2(scale) * 0.02f, color, duration, deAcc, earlyKillTimer, extension));
            _speedParticleBuffer.Create(new ParticleInfo(Position, Velocity * 0.5f, Velocity.ToRotation(), new SystemVector2(scale) * 0.025f, color, duration, deAcc, earlyKillTimer, extension));
        }

        public static void CreateDaggerSlashParticle(Vector2 position, Vector2 velocity, float scale, Color color, int dustType, int duration = 6)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            for (int i = -1; i < 2; i += 2)
                _slashParticleBuffer.Create(new ParticleInfo(Position, Velocity, MathHelper.PiOver4 * i, Scale * 0.4f, color.WithAlpha(0), duration, (float)ParticleFlags2.DaggerSlash, dustType));
        }
        public static void CreateEmeraldCutterParticle(Vector2 position, Vector2 velocity, float scale, Color color, int dustType, int duration = 6)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            for (int i = -1; i < 2; i++)
            {
                float ScaleY = 2f - i * i * 0.5f;
                _slashParticleBuffer.Create(new ParticleInfo(Position, Velocity, MathHelper.PiOver2 + MathHelper.Pi / 4 * i, new SystemVector2(Scale.X, Scale.Y * ScaleY) * 0.3f, color.WithAlpha(0) * 2, duration, (float)ParticleFlags2.EmeraldCutter, dustType));
            }
        }
        public static void CreateDevilsPactParticle(Vector2 position, Vector2 velocity, float scale, Color color, int dustType = -1, int duration = 10)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(72) * scale;

            for (int i = -1; i < 2; i++)
            {
                float ScaleY = 2f - i * i * 0.5f;
                _slashAltParticleBuffer.Create(new ParticleInfo(Position, Velocity, MathHelper.Pi / 4 * i, new SystemVector2(Scale.X, Scale.Y * ScaleY) * 0.3f, color * 2, duration, (float)ParticleFlags2.DevilsPact, dustType));
            }
        }
        public static void CreateSlashParticle(Vector2 position, Vector2 velocity, float scale, Color color, int duration = 8, byte alpha = 0)
        {
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = scale;
            float Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            color.A = alpha;

            SystemVector2 leftEnd = Position + Velocity;
            SystemVector2 rightEnd = Position - Velocity;

            for (float i = 0f; i <= 1f; i += 0.05f)
            {
                SystemVector2 drawPos1 = SystemVector2.Lerp(Position, rightEnd, i);
                SystemVector2 drawPos2 = SystemVector2.Lerp(Position, leftEnd, i);
                _slashAltParticleBuffer.Create(new ParticleInfo(drawPos1, Velocity, Rotation, new SystemVector2(Scale * (4 - 3 * i), Scale), color * 0.35f, duration, (float)ParticleFlags2.Slash));
                _slashAltParticleBuffer.Create(new ParticleInfo(drawPos2, Velocity, Rotation, new SystemVector2(Scale * (4 - 3 * i), Scale), color * 0.35f, duration, (float)ParticleFlags2.Slash));
                _slashAltParticleBuffer.Create(new ParticleInfo(drawPos1, Velocity, Rotation, new SystemVector2(Scale), color * 0.25f, duration, (float)ParticleFlags2.Slash));
                _slashAltParticleBuffer.Create(new ParticleInfo(drawPos2, Velocity, Rotation, new SystemVector2(Scale), color * 0.25f, duration, (float)ParticleFlags2.Slash));
            }
        }
    }
}