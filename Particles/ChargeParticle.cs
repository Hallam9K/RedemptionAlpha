using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Particles
{
    public class ChargeParticle : Particle
    {
        public Player Player;

        public Projectile Projectile;


        public float StartRotationGap;

        public float StartDistance;

        public float EndDistance;

        public float Timer;

        public float Extension;

        public float ScaleX;

        public float ScaleY;

        public override string Texture => "Redemption/Particles/EmberParticle";
        public ChargeParticle(Player player, Projectile projectile, Vector2 startVector, float endDistance, float extension = 30, float scale = 1)
        {
            Player = player;
            Projectile = projectile;
            Scale = scale;
            Rotation = startVector.ToRotation() + 3.14f;
            StartDistance = startVector.Length();
            EndDistance = endDistance;
            Extension = extension;
        }
        public override void Spawn()
        {
            StartRotationGap = Projectile.velocity.ToRotation() - Rotation;
            Velocity = Vector2.Zero;
            TileCollide = false;
            TimeLeft = 11;
            Timer = 0;
        }
        public override void Update()
        {
            Timer++;
            float progress = Timer / 10;
            float modifiedProgress = MathF.Pow(progress, 3f);
            ScaleX = Scale + Extension * progress;
            ScaleY = Scale - Scale * progress;
            if (Player != null && Player.active && !Player.dead)
            {
                Rotation = Projectile.velocity.ToRotation() + StartRotationGap;
                Vector2 setPosition = Player.RotatedRelativePoint(Player.MountedCenter, true);
                Vector2 movePos = Rotation.ToRotationVector2() * MathHelper.Lerp(StartDistance, EndDistance, modifiedProgress);
                Position = setPosition + movePos;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Color a = Color.Multiply(Color with { A = 0 }, 1);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Particles/GlowParticle").Value, location, new Rectangle(0, 0, 128, 128), a, Rotation, new Vector2(64, 64), new Vector2(ScaleX, ScaleY) * 0.02f, 0, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Particles/GlowParticle").Value, location, new Rectangle(0, 0, 128, 128), a, Rotation, new Vector2(64, 64), new Vector2(ScaleX, ScaleY) * 0.025f, 0, 0f);
        }
    }
}
