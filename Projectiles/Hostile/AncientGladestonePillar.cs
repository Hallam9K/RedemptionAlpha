using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PreHM.Druid.Staves;
using Redemption.Items.Weapons.PreHM.Magic;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.PreHM
{
    public class AncientGladestonePillar : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 110;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }
        Player clearCheck;
        public override void AI()
        {
            if (Main.rand.Next(2) == 0 && projectile.localAI[0] < 30)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<SlateDust>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, Scale: 2);
            }
            if (projectile.velocity.Length() != 0) { projectile.hostile = true; }
            else { projectile.hostile = false; }
            projectile.localAI[0]++;
            if (projectile.localAI[0] < 30)
                projectile.alpha -= 10;
            if (projectile.localAI[0] == 30)
            {
                projectile.hostile = true;
                if (!Main.dedServ) { Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EarthBoom").WithVolume(.3f), (int)projectile.position.X, (int)projectile.position.Y); }
                projectile.velocity.Y -= 10;
            }
            if (projectile.localAI[0] == 40)
            {
                projectile.velocity.Y = 0;
            }
            if (projectile.localAI[0] > 45)
            {
                projectile.alpha += 10;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                }
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                clearCheck = Main.player[p];
                if (projectile.velocity.Length() != 0 && Collision.CheckAABBvAABBCollision(projectile.position, projectile.Size, clearCheck.position, clearCheck.Size))
                {
                    clearCheck.velocity.Y = projectile.velocity.Y * 1.5f;
                }
            }
        }
    }
}