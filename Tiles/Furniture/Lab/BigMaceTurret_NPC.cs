using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.NPCs.Lab.MACE;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BigMaceTurret_NPC : ModProjectile
    {
        public Tile Parent;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory MACE Turret");
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (!Parent.HasTile || Parent.TileType != ModContent.TileType<BigMaceTurretTile>() || !Main.LocalPlayer.InModBiome<LabBiome>())
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            if (Parent.TileFrameX == 18)
                Projectile.spriteDirection = -1;
            else
                Projectile.spriteDirection = 1;

            if (Projectile.frame > 0)
            {
                if (Projectile.frameCounter++ >= 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 0;
                }
            }
            Projectile.velocity *= 0;
            int Mace = NPC.FindFirstNPC(ModContent.NPCType<MACEProject>());

            if (Mace >= 0)
            {
                NPC npc = Main.npc[Mace];
                switch (Projectile.localAI[0])
                {
                    case 0:
                        Projectile.rotation.SlowRotation(-MathHelper.PiOver2, (float)Math.PI / 180f);
                        if (npc.ai[0] == 2 && npc.ai[3] == 5)
                        {
                            Projectile.localAI[0] = 1;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        Projectile.localAI[1]++;
                        Vector2 ShootPos = Projectile.Center + RedeHelper.PolarVector(16 * Projectile.spriteDirection, Projectile.rotation - MathHelper.PiOver2) - RedeHelper.PolarVector(28, Projectile.rotation);
                        Projectile.rotation.SlowRotation(Projectile.spriteDirection == -1 ? (float)Math.PI - 1 : 1, (float)Math.PI / 210f);
                        if (Projectile.localAI[1] % 15 == 0 && Main.myPlayer == Projectile.owner)
                        {
                            Projectile.frame = 1;
                            SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), ShootPos, RedeHelper.PolarVector(18, Projectile.rotation + -MathHelper.Pi), ModContent.ProjectileType<MACE_FlakBullet>(), npc.damage / 4, 0, Main.myPlayer);
                        }
                        if (npc.ai[3] != 5)
                        {
                            Projectile.localAI[0] = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                }
            }
            else
            {
                Projectile.rotation = -MathHelper.PiOver2;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 - 8);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0), drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}