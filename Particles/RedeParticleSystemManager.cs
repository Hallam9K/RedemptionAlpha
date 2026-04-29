using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Core.V3;
using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.Particles.ParticleBehaviors;
using SystemVector2 = System.Numerics.Vector2;

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

        public static Dictionary<Layer, ParticleCollection> _glowParticleCollections;
        public static Dictionary<Layer, ParticleCollection> _soullessParticleCollections;

        private static ParticleBuffer<BlackholeParticleBehavior> _blackholeParticleBuffer;
        public static Dictionary<Layer, ParticleCollection> _shadowParticleCollections;
        private static ParticleBuffer<SlashParticleBehavior> _slashParticleBuffer;
        private static ParticleBuffer<SlashAltParticleBehavior> _slashParticle2Buffer;
        public static Dictionary<Layer, ParticleCollection> _laserParticleCollections;
        private static ParticleBuffer<PulseParticleBehavior> _pulseParticleBuffer;
        private static ParticleBuffer<RainbowParticleBehavior> _rainbowParticleBuffer;
        private static ParticleBuffer<SpeedParticleBehavior> _speedParticleBuffer;

        private static ParticleBuffer<GlowParticleBehavior> _glowParticleAdditiveBuffer;
        private static ParticleBuffer<WhiteGlowParticleBehavior> _whiteGlowParticleBuffer;


        public static Color[] emberColors = [new(240, 149, 46, 0), new(187, 63, 25, 0), new(131, 23, 37, 0)];
        public static Color[] blueEmberColors = [new(30, 182, 228, 0), new(20, 91, 183, 0), new(13, 26, 139, 0)];
        public static Color[] purpleEmberColors = [new(208, 50, 232, 0), new(176, 43, 183, 0), new(95, 26, 141, 0)];
        public static Color[] lightningColors = [new(255, 255, 255, 0), new(161, 255, 253, 0), new(40, 186, 242, 0)];

        public static Color[] yellowLightningColors =
            [new(255, 255, 255, 0), new(255, 255, 174, 0), new(255, 189, 69, 0)];

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
                {
                    Layer.BeforeDust,
                    new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeDust)
                },
                {
                    Layer.BeforeProjectiles,
                    new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048),
                        Layer.BeforeProjectiles)
                },
                {
                    Layer.BeforeNPCs,
                    new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeNPCs)
                },
                {
                    Layer.BeforeItems,
                    new ParticleCollection().Add(new ParticleBuffer<SoullessParticleBehavior>(2048), Layer.BeforeItems)
                }
            };

            _glowParticleBuffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticleBuffer);

            _glowParticle3Buffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticle3Buffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticle3Buffer);

            _glowParticle4Buffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_glowParticle4Buffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticle4Buffer);

            _whiteFlareParticleBuffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_whiteFlareParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _whiteFlareParticleBuffer);

            _blackholeParticleBuffer = new(2048);
            ParticleManagerV3.RegisterUpdatable(_blackholeParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _blackholeParticleBuffer);

            _shadowParticleCollections = new()
            {
                {
                    Layer.BeforeDust,
                    new ParticleCollection().Add(new ParticleBuffer<BigFlareParticleBehavior>(2048), Layer.BeforeDust)
                },
                {
                    Layer.BeforeProjectiles,
                    new ParticleCollection().Add(new ParticleBuffer<BigFlareParticleBehavior>(2048),
                        Layer.BeforeProjectiles)
                },
                {
                    Layer.BeforeNPCs,
                    new ParticleCollection().Add(new ParticleBuffer<BigFlareParticleBehavior>(2048), Layer.BeforeNPCs)
                },
                {
                    Layer.BeforeItems,
                    new ParticleCollection().Add(new ParticleBuffer<BigFlareParticleBehavior>(2048), Layer.BeforeItems)
                }
            };

            _slashParticleBuffer = new(512);
            ParticleManagerV3.RegisterUpdatable(_slashParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _slashParticleBuffer);

            _slashParticle2Buffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_slashParticle2Buffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _slashParticle2Buffer);

            _laserParticleCollections = new()
            {
                {
                    Layer.BeforeDust,
                    new ParticleCollection().Add(new ParticleBuffer<LaserParticleBehavior>(2048), Layer.BeforeDust)
                },
                {
                    Layer.BeforeProjectiles,
                    new ParticleCollection().Add(new ParticleBuffer<LaserParticleBehavior>(2048),
                        Layer.BeforeProjectiles)
                },
                {
                    Layer.BeforeNPCs,
                    new ParticleCollection().Add(new ParticleBuffer<LaserParticleBehavior>(2048), Layer.BeforeNPCs)
                },
                {
                    Layer.BeforeItems,
                    new ParticleCollection().Add(new ParticleBuffer<LaserParticleBehavior>(2048), Layer.BeforeItems)
                }
            };

            _pulseParticleBuffer = new();
            _pulseParticleBuffer.SetBlendState(BlendState.Additive);
            ParticleManagerV3.RegisterUpdatable(_pulseParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _pulseParticleBuffer);

            _rainbowParticleBuffer = new(1024);
            ParticleManagerV3.RegisterUpdatable(_rainbowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _rainbowParticleBuffer);

            _speedParticleBuffer = new(8192);
            ParticleManagerV3.RegisterUpdatable(_speedParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _speedParticleBuffer);

            _glowParticleAdditiveBuffer = new(16384);
            _glowParticleAdditiveBuffer.SetBlendState(BlendState.Additive);
            ParticleManagerV3.RegisterUpdatable(_glowParticleAdditiveBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _glowParticleAdditiveBuffer);

            _whiteGlowParticleBuffer = new(16384);
            ParticleManagerV3.RegisterUpdatable(_whiteGlowParticleBuffer);
            ParticleManagerV3.RegisterRenderable(Layer.BeforeDust, _whiteGlowParticleBuffer);
        }
        public override void Unload()
        {
            emberColors = null;
            blueEmberColors = null;
            purpleEmberColors = null;
            lightningColors = null;
            yellowLightningColors = null;
            redColors = null;
            purpleColors = null;
            goldColors = null;
            greenColors = null;
            spiritColors = null;
            infernalSpiritColors = null;
            redThrusterColors = null;
            blueThrusterColors = null;
            soulColors = null;

            if (Main.netMode is NetmodeID.Server)
                return;

            _glowParticleCollections = null;
            _soullessParticleCollections = null;
            _glowParticleBuffer = null;
            _glowParticle3Buffer = null;
            _glowParticle4Buffer = null;
            _whiteFlareParticleBuffer = null;
            _blackholeParticleBuffer = null;
            _shadowParticleCollections = null;
            _slashParticleBuffer = null;
            _slashParticle2Buffer = null;
            _laserParticleCollections = null;
            _pulseParticleBuffer = null;
            _rainbowParticleBuffer = null;
            _speedParticleBuffer = null;
            _glowParticleAdditiveBuffer = null;
            _whiteGlowParticleBuffer = null;
        }

        #region QuadParticles

        // These just have the same behaviour and settings as the 2.0 QuadParticles, these aren't actual QuadParticles
        public static void CreateQuadParticle(Vector2 position, Vector2 velocity, Vector2 scale, Color color,
            Color endColor, int duration, float velChange = 1, float scaleChange = 1, float depth = 1,
            ParticleFlags style = ParticleFlags.Quad)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(128) * scale).ToNumerics();

            _glowParticle3Buffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration,
                (float)style, BitConverter.UInt32BitsToSingle(endColor.PackedValue), velChange, scaleChange));
        }

        public static void CreateQuadParticle2(Vector2 position, Vector2 velocity, Vector2 scale, Color color,
            Color endColor, int duration, float velChange = 1, float scaleChange = 1, float depth = 1,
            ParticleFlags style = ParticleFlags.Quad)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(64) * scale).ToNumerics();

            _glowParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration,
                (float)style, BitConverter.UInt32BitsToSingle(endColor.PackedValue), velChange, scaleChange));
        }

        #endregion

        public static void CreateShadowParticle(Vector2 position, Vector2 velocity, Vector2 scale, int duration,
            float velChange = 1, float scaleChange = 1, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = (new Vector2(682) * scale).ToNumerics();
            Color endColor = new(0, 0, 0);

            if (_shadowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(new ParticleInfo(Position, Velocity, velocity.ToRotation(), Scale, new(0, 0, 0),
                    duration, (float)ParticleFlags.Quad, BitConverter.UInt32BitsToSingle(endColor.PackedValue),
                    velChange, scaleChange));
            }
        }

        #region Glow Particles

        public static void CreateGlowParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int duration, ParticleFlags style = ParticleFlags.Fading, float fadeAdd = .45f,
            Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, color.WithAlpha(0), duration, (float)style, fadeAdd),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale,
                        Color.White.WithAlpha(0), duration, (float)style, fadeAdd));
            }
        }

        public static void CreateGlowParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors,
            int duration, float velChange = 0.9f, float opacityChange = 1, float scaleChange = 0.9f,
            Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

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
                    new ParticleInfo(Position, Velocity, 0, Scale, mid, duration, flag, darkPacked, velChange,
                        opacityChange, scaleChange, 0),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, bright,
                        duration, flag, darkPacked, velChange, opacityChange, scaleChange, 0));
            }
        }

        public static void CreateAdditiveGlowParticle(Vector2 position, Vector2 velocity, Vector2 scale, Color color,
            int duration, float velChange = .91f, ParticleFlags style = ParticleFlags.Basic)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateAdditiveGlowParticle(position.ToNumerics(), velocity.ToNumerics(), scale, color, duration, velChange,
                style);
        }

        public static void CreateAdditiveGlowParticle(SystemVector2 position, SystemVector2 velocity, Vector2 scale,
            Color color, int duration, float velChange = .91f, ParticleFlags style = ParticleFlags.Basic)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Scale = (new Vector2(64) * scale).ToNumerics();

            _glowParticleAdditiveBuffer.Create(new ParticleInfo(position, velocity, velocity.ToRotation(), Scale, color,
                duration, (float)style, velChange));
        }

        public static void CreateWhiteGlowParticle(Vector2 position, Vector2 velocity, Vector2 scale, Color color,
            int duration, float velChange = .91f, ParticleFlags style = ParticleFlags.Basic)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateWhiteGlowParticle(position.ToNumerics(), velocity.ToNumerics(), scale, color, duration, velChange,
                style);
        }

        public static void CreateWhiteGlowParticle(SystemVector2 position, SystemVector2 velocity, Vector2 scale,
            Color color, int duration, float velChange = .91f, ParticleFlags style = ParticleFlags.Basic)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Scale = (new Vector2(192) * scale).ToNumerics();

            _whiteGlowParticleBuffer.Create(new ParticleInfo(position, velocity, velocity.ToRotation(), Scale, color,
                duration, (float)style, velChange));
        }

        #endregion

        public static void CreateBlackholeParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int projID, int duration = 120)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(128 * scale);

            _blackholeParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * 0.2f, Color.Transparent,
                duration, projID, BitConverter.UInt32BitsToSingle(color.PackedValue), 0));
            _blackholeParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * 0.3f, Color.Transparent,
                duration, projID, BitConverter.UInt32BitsToSingle(color.PackedValue), 0));
        }

        #region Ember Particles

        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, Color brightColor,
            Color midColor, Color darkColor, int duration = 120, int timeBeforeMoving = 0,
            Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(64 * scale * size);
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.WithAlpha(0).PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor.WithAlpha(0), duration,
                        (float)ParticleFlags.Ember, timeBeforeMoving, darkPacked),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale,
                        brightColor.WithAlpha(0), duration, (float)ParticleFlags.Ember, timeBeforeMoving, darkPacked));
            }
        }

        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors,
            int duration = 120, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateEmberParticle(position, velocity, scale, colors[0].WithAlpha(0), colors[1].WithAlpha(0),
                colors[2].WithAlpha(0), duration, timeBeforeMoving, layer);
        }

        public static void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120,
            int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateEmberParticle(position, velocity, scale, emberColors, duration, timeBeforeMoving, layer);
        }

        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, Color brightColor,
            Color midColor, Color darkColor, int duration = 120, float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Scale = new SystemVector2(64 * scale * size);
            var darkPacked = BitConverter.UInt32BitsToSingle(darkColor.WithAlpha(0).PackedValue);

            float pixelRatio = 1f / 64f;
            if (_glowParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor.WithAlpha(0), duration,
                        (float)ParticleFlags.EmberBurst, darkPacked, velChange),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale,
                        brightColor.WithAlpha(0), duration, (float)ParticleFlags.EmberBurst, darkPacked, velChange));
            }
        }

        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors,
            int duration = 120, float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateEmberBurstParticle(position, velocity, scale, colors[0].WithAlpha(0), colors[1].WithAlpha(0),
                colors[2].WithAlpha(0), duration, velChange, layer);
        }

        public static void CreateEmberBurstParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120,
            float velChange = 1, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

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

        public static void CreateSpiritParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors,
            int duration = 120, int timeBeforeMoving = 0, float depth = 1, float opacityScale = 1,
            Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

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
                    new ParticleInfo(Position, Velocity, 0, Scale, depth, midColor, duration,
                        (float)ParticleFlags.Spirit, timeBeforeMoving, timer, speedX, multi, darkPacked, opacityScale),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale, depth,
                        brightColor, duration, (float)ParticleFlags.Spirit, timeBeforeMoving, timer, speedX, multi,
                        darkPacked, opacityScale));
            }
        }

        public static void CreateSpiritParticle(Vector2 position, Vector2 velocity, float scale, int duration = 120,
            int timeBeforeMoving = 0, float depth = 1, float opacityScale = 1, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateSpiritParticle(position, velocity, scale, spiritColors, duration, timeBeforeMoving, depth,
                opacityScale, layer);
        }

        #endregion

        public static void CreateSoullessParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int duration = 20, int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust, float depth = 1,
            float scaleFade = 1)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float size = Main.rand.NextFloat(5f, 11f) / 10f;
            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(22, 24) * scale * size;

            if (_soullessParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(new ParticleInfo(Position, Velocity, 0, Scale, depth, color, duration,
                    timeBeforeMoving, 1, scaleFade));
            }
        }

        public static void CreateSoullessParticle(Vector2 position, Vector2 velocity, float scale,
            int timeBeforeMoving = 0, Layer layer = Layer.BeforeDust, float depth = 1, float scaleFade = 1)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateSoullessParticle(position, velocity, scale, new Color(24, 28, 38), Main.rand.Next(10, 40),
                timeBeforeMoving, layer, depth, scaleFade);
        }

        public static void CreateLightningParticle(Vector2 position, Vector2 velocity, float scale, Color[] colors,
            bool clean = false, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

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
                    new ParticleInfo(Position, Velocity, 0, Scale, midColor, duration, (float)ParticleFlags.EmberBurst,
                        darkPacked, 1),
                    new ParticleInfo(Position - new SystemVector2(1.5f), Velocity, 0, pixelRatio * 3f * Scale,
                        brightColor, duration, (float)ParticleFlags.EmberBurst, darkPacked, 1));
            }
        }

        public static void CreateLaserParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            float squish = 5, int duration = 8, Layer layer = Layer.BeforeDust)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(scale);

            if (_laserParticleCollections.TryGetValue(layer, out ParticleCollection collection))
            {
                collection.Create(
                    new ParticleInfo(Position, SystemVector2.Zero, Velocity.ToRotation() + MathHelper.PiOver2, Scale,
                        color, duration, 0, squish),
                    new ParticleInfo(Position, SystemVector2.Zero, Velocity.ToRotation() + MathHelper.PiOver2, Scale,
                        color, duration, 0, squish));
            }
        }

        public static void CreateChargeParticle(float scale, Color color, int playerID, int projID, Vector2 startVector,
            float endDistance, float extension = 30, int duration = 11)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float startDistance = startVector.Length();
            Color a = Color.Multiply(color.WithAlpha(0), 1);

            float rotation = startVector.ToRotation() + 3.14f;
            float rotationGap = Main.projectile[projID].velocity.ToRotation() - rotation;

            _glowParticle4Buffer.Create(new ParticleInfo(default, SystemVector2.Zero, rotation,
                new SystemVector2(scale), a, duration, (float)ParticleFlags.Charge, 0, playerID, projID, startDistance,
                endDistance, extension, rotationGap));
            _glowParticle4Buffer.Create(new ParticleInfo(default, SystemVector2.Zero, rotation,
                new SystemVector2(scale), a, duration, (float)ParticleFlags.Charge, 0, playerID, projID, startDistance,
                endDistance, extension, rotationGap));
        }

        public static void CreatePulseParticle(Vector2 position, Vector2 velocity, float rotation, Vector2 scale,
            Color color, int intensity = 1, float scaleSpeed = 1, int duration = 6)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(200) * scale.ToNumerics();

            for (int i = 0; i < intensity; i++)
                _pulseParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale, Color.Transparent, duration,
                    scaleSpeed, BitConverter.UInt32BitsToSingle(color.PackedValue)));
        }

        public static void CreateRainbowParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            float opacityScale = 1, int duration = 50)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            float rotation = Main.rand.NextFloat(0f, 360f);
            float flag = (float)ParticleFlags.Rainbow;

            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * 0.75f,
                color.WithAlpha(0), duration, flag, opacityScale, rotation, 0));
            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation, Scale * 0.75f,
                color.WithAlpha(0), duration, flag, opacityScale, rotation, MathHelper.PiOver2));

            Scale = new SystemVector2(144) * scale;
            _whiteFlareParticleBuffer.Create(new ParticleInfo(Position, Velocity, 0, Scale, color.WithAlpha(0),
                duration, flag, opacityScale, -1, 0));
            Scale = new SystemVector2(128) * scale;
            _glowParticle4Buffer.Create(new ParticleInfo(Position, Velocity, 0, Scale * .3f * opacityScale,
                color.WithAlpha(0), duration, flag, 0.5f, -1, 0));
        }

        public static void CreateRainbowParticle(Vector2 position, Vector2 velocity, float scale,
            float opacityScale = 1, int duration = 50)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            CreateRainbowParticle(position, velocity, scale, Color.White.WithAlpha(0), opacityScale, duration);
        }

        public static void CreateSimpleStarParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            float opacityScale = 1, float deceleration = 0.98f, int duration = 50)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            float rotation = Main.rand.NextFloat(0f, 360f);
            float flag = (float)ParticleFlags.SimpleStar;

            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation,
                Scale * new SystemVector2(0.75f, 0.5f), color.WithAlpha(0) * opacityScale, duration, flag, deceleration,
                rotation, 0));
            _rainbowParticleBuffer.Create(new ParticleInfo(Position, Velocity, rotation,
                Scale * new SystemVector2(0.75f, 0.5f), color.WithAlpha(0) * opacityScale, duration, flag, deceleration,
                rotation, MathHelper.PiOver2));
        }

        public static void CreateSpeedParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            float deAcc = 0.91f, int duration = 21, int earlyKillTimer = 0, float extension = 0)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();

            if (extension == 0)
                extension = Main.rand.NextFloat(28, 32);
            else
                extension *= Main.rand.NextFloat(0.95f, 1.05f);

            _speedParticleBuffer.Create(new ParticleInfo(Position, Velocity * 0.5f, Velocity.ToRotation(),
                new SystemVector2(scale) * 0.02f, color, duration, deAcc, earlyKillTimer, extension));
            _speedParticleBuffer.Create(new ParticleInfo(Position, Velocity * 0.5f, Velocity.ToRotation(),
                new SystemVector2(scale) * 0.025f, color, duration, deAcc, earlyKillTimer, extension));
        }

        public static void CreateDaggerSlashParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int dustType, int duration = 6)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            for (int i = -1; i < 2; i += 2)
                _slashParticleBuffer.Create(new ParticleInfo(Position, Velocity, MathHelper.PiOver4 * i, Scale * 0.4f,
                    color.WithAlpha(0), duration, (float)ParticleFlags2.DaggerSlash, dustType));
        }

        public static void CreateEmeraldCutterParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int dustType, int duration = 6)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(142, 42) * scale;

            for (int i = -1; i < 2; i++)
            {
                float ScaleY = 2f - i * i * 0.5f;
                _slashParticleBuffer.Create(new ParticleInfo(Position, Velocity,
                    MathHelper.PiOver2 + MathHelper.Pi / 4 * i, new SystemVector2(Scale.X, Scale.Y * ScaleY) * 0.3f,
                    color.WithAlpha(0) * 2, duration, (float)ParticleFlags2.EmeraldCutter, dustType));
            }
        }

        public static void CreateDevilsPactParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int dustType = -1, int duration = 10)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = new SystemVector2(72) * scale;

            for (int i = -1; i < 2; i++)
            {
                float ScaleY = 2f - i * i * 0.5f;
                _slashParticle2Buffer.Create(new ParticleInfo(Position, Velocity,
                    velocity.ToRotation() + MathHelper.Pi / 4 * i, new SystemVector2(Scale.X, Scale.Y * ScaleY) * 0.3f,
                    color * 2, duration, (float)ParticleFlags2.DevilsPact, dustType));
            }
        }

        public static void CreateSlashParticle(Vector2 position, Vector2 velocity, float scale, Color color,
            int duration = 12, byte alpha = 0)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var Position = position.ToNumerics();
            var Velocity = velocity.ToNumerics();
            var Scale = scale;
            float Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            color.A = alpha;

            SystemVector2 leftEnd = Position + Velocity;
            SystemVector2 rightEnd = Position - Velocity;

            for (float i = 0f; i <= 1f; i += 0.1f)
            {
                SystemVector2 drawPos1 = SystemVector2.Lerp(Position, rightEnd, i * 0.25f);
                SystemVector2 drawPos2 = SystemVector2.Lerp(Position, leftEnd, i * 0.25f);
                _slashParticle2Buffer.Create(new ParticleInfo(drawPos1, Velocity, Rotation,
                    new SystemVector2(Scale * (2 - 1 * i), Scale * 1.65f), color * 0.2f, duration,
                    (float)ParticleFlags2.Slash, -i));
                _slashParticle2Buffer.Create(new ParticleInfo(drawPos2, Velocity, Rotation,
                    new SystemVector2(Scale * (2 - 1 * i), Scale * 1.65f), color * 0.2f, duration,
                    (float)ParticleFlags2.Slash, i));
                _slashParticle2Buffer.Create(new ParticleInfo(drawPos1, Velocity, Rotation, new SystemVector2(Scale),
                    color * 0.25f, duration, (float)ParticleFlags2.Slash, -i));
                _slashParticle2Buffer.Create(new ParticleInfo(drawPos2, Velocity, Rotation, new SystemVector2(Scale),
                    color * 0.25f, duration, (float)ParticleFlags2.Slash, i));
            }
        }
    }
}