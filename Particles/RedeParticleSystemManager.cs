using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    /// <summary>
    /// This class demonstrates a good way to manage your GPU particle systems.
    /// It's a good idea to centralize your GPU particle systems, as you'll want to reuse them as much as possible.
    /// </summary>
    public class RedeParticleSystemManager : ModSystem
	{
		public static QuadParticleSystem RedeQuadSystem { get; private set; }
		public static QuadParticleSystem RedeQuadSystem2 { get; private set; }
		public static QuadParticleSystemSettings RedeQuadSettings { get; private set; }
		public static QuadParticleSystemSettings RedeQuadSettings2 { get; private set; }
		public static QuadParticle RedeQuadParticle { get; private set; }

		public static PointParticleSystem RedePointSystem { get; private set; }
		public static PointParticleSystemSettings RedePointSettings { get; private set; }
		public static PointParticle RedePointParticle { get; private set; }

		public static RedeParticleSystemWrapper<QuadParticle> RedeWrappedQuadParticleSystem { get; private set; }

		public override void OnModLoad()
        {
            if (Main.netMode is NetmodeID.Server)
                return;

            // Demonstrates creating a Quad particle system.
            RedeQuadSettings = new(ModContent.Request<Texture2D>("Redemption/Particles/Star", AssetRequestMode.ImmediateLoad).Value, 4000, 300, blendState: BlendState.AlphaBlend);
            RedeQuadSettings2 = new(ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow", AssetRequestMode.ImmediateLoad).Value, 4000, 300, blendState: BlendState.AlphaBlend);
            RedeQuadSystem = new QuadParticleSystem(RedeQuadSettings);
            RedeQuadSystem2 = new QuadParticleSystem(RedeQuadSettings2);
            RedeQuadParticle = new()
			{
                StartColor = Color.White.WithAlpha(0f),
                EndColor = Color.Black.WithAlpha(0f),
                Scale = new Vector2(1f),
                Rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi + float.Epsilon),
                RotationVelocity = Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
                Depth = 1f + Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
                DepthVelocity = Main.rand.NextFloat(-0.001f, 0.001f + float.Epsilon)
            };

            // Demonstrates creating a Point particle system.
            RedePointSettings = new(500, 300);
            RedePointSystem = new PointParticleSystem(RedePointSettings);
            RedePointParticle = new()
			{
                StartColor = Color.White.WithAlpha(0f),
                EndColor = Color.Black.WithAlpha(0f),
                Depth = 1f + Main.rand.NextFloat(-0.1f, 0.1f + float.Epsilon),
                DepthVelocity = Main.rand.NextFloat(-0.001f, 0.001f + float.Epsilon)
            };

			// Demonstrates creating a wrapped particle system from a Quad particle system.
			// This can be useful for implementing custom functionality, such as embedding your system into a RenderTarget2D.
			RedeWrappedQuadParticleSystem = new(RedeQuadSystem, RedeQuadSettings);
		}

		public override void Unload()
		{
            // Always make sure to dispose GPU particle systems when you're done with them!
            RedeQuadSystem?.Dispose();
            RedeQuadSystem = null;
            RedeQuadSystem2?.Dispose();
            RedeQuadSystem2 = null;
            RedeQuadSettings = null;
            RedeQuadSettings2 = null;
            RedeQuadParticle = null;

            RedePointSystem?.Dispose();
            RedePointSystem = null;
            RedePointSettings = null;
            RedePointParticle = null;

            RedeWrappedQuadParticleSystem = null;
        }
    }
}