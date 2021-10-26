﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class Scan_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scan");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<KS3_ScannerDrone>())
                Projectile.Kill();

            Vector2 Pos = new(npc.Center.X + 5 * npc.spriteDirection, npc.Center.Y - 3);
            Projectile.Center = Pos;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.localAI[0] -= 0.03f;
                    if (Projectile.localAI[0] <= -0.6f)
                    {
                        Projectile.localAI[0] = -0.6f;
                        Projectile.localAI[1] = 1;
                    }
                    break;
                case 1:
                    Projectile.localAI[0] += 0.03f;
                    if (Projectile.localAI[0] >= 0.6f)
                    {
                        Projectile.localAI[0] = 0.6f;
                        Projectile.localAI[1] = 0;
                    }
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (1.57f * npc.spriteDirection) + Projectile.localAI[0];
        }
    }
}