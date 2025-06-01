using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Core.V3.Particles;
using Redemption.Base;
using Redemption.Textures;
using System;
using System.Collections.Generic;
using Terraria;

namespace Redemption.Particles
{
    // Recommend this is changed, maybe something like DrawParticleElectricity in DustHelper.cs, if it is different from it enough
    public class ElectricParticle : Particle
    {
        public override string Texture => "Redemption/Particles/EmberParticle";
        public List<Vector2> Cache = new();
        public List<float> Alpha = new();
        public float[] Speed;
        public Vector2 StartPos;
        public Vector2 EndPos;
        public float Density;
        public float ArmLength;
        public float MaxSlope;
        public int MaxTime;
        public ElectricParticle(Vector2 startPos, Vector2 endPos, int maxTime, float[] speed, float desnity = 0.1f, float armLength = 30, float slope = 1)
        {
            StartPos = startPos;
            EndPos = endPos;
            Density = desnity;
            ArmLength = armLength;
            MaxTime = maxTime;
            Speed = speed;
            MaxSlope = slope;
        }
        public float Progress;
        public float Timer;
        public override void Spawn()
        {
            int nodeCount = (int)(Vector2.Distance(StartPos, EndPos) / ArmLength);
            Vector2[] nodes = new Vector2[nodeCount + 1];

            nodes[nodeCount] = EndPos;

            for (int k = 1; k < nodes.Length; k++)
            {
                nodes[k] = Vector2.Lerp(StartPos, EndPos, k / (float)nodeCount) + (k == nodes.Length - 1 ? Vector2.Zero : Vector2.Normalize(StartPos - EndPos).RotatedBy(1.58f) * Main.rand.NextFloat(-ArmLength / 2, ArmLength / 2));
                Vector2 prevPos = k == 1 ? StartPos : nodes[k - 1];
                for (float i = 0; i < 1; i += Density)
                {
                    Vector2 lerp = Vector2.Lerp(prevPos, nodes[k], i);
                    Cache.Add(lerp);
                }
            }

            TimeLeft = MaxTime;
            Progress = Timer / (float)(Cache.Count + 1); // 0 ~ 2

            for (int k = 0; k < Cache.Count; k++)
            {
                float x = (float)k / (float)Cache.Count;
                float lerp = MathHelper.Clamp(BaseUtility.MultiLerp(Progress + x - 1f, MaxSlope, 0), 0, 1);
                Alpha.Add(lerp);
            }
        }
        public override void Update()
        {
            TimeLeft = MaxTime;
            Progress = Timer / (float)(Cache.Count + 1); // 0 ~ 2

            Alpha = new();
            for (int k = 0; k < Cache.Count; k++)
            {
                float x = (float)k / (float)Cache.Count; // 0 ~ 1
                float lerp = MathHelper.Clamp(BaseUtility.MultiLerp(Progress + x - 1f, MaxSlope, 0), 0, 1);
                Alpha.Add(lerp);
            }

            int index = (int)MathF.Min(Progress * 0.5f * Speed.Length, Speed.Length - 1);
            Timer += Speed[index] * (float)Cache.Count / (float)MaxTime;

            if (Progress > 2)
                Kill();
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            for (int k = 0; k < Cache.Count; k++)
            {
                Color c = Color * Alpha[k];
                Vector2 pos = location + Cache[k];
                spriteBatch.Draw(CommonTextures.WhiteGlow.Value, pos, new Rectangle(0, 0, 192, 192), c, Rotation, new Vector2(96, 96), Scale * Alpha[k], 0, 0f);

            }
        }
    }
}
