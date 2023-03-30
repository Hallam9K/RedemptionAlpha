using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_Crosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crosshair");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            DrawOffsetX = -18;
            DrawOriginOffsetY = -18;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 1 * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.4f * Projectile.Opacity);

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<OO>())
                Projectile.Kill();
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] == 20 && Main.myPlayer == Projectile.owner)
            {
                float[] armRot = (Main.npc[npc.whoAmI].ModNPC as OO).ArmRot;
                int[] rocketFrame = (Main.npc[npc.whoAmI].ModNPC as OO).ArmRFrameY;
                int side = 0;
                if (rocketFrame[0] > 4)
                    side = 1;

                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.MissileFire1, npc.position);

                Vector2 x = RedeHelper.PolarVector(npc.spriteDirection == -1 ? -60 : 20, armRot[side]);
                Vector2 y = RedeHelper.PolarVector(npc.spriteDirection == -1 ? 8 : 28, armRot[side] - (float)Math.PI / 2);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center + x + y + new Vector2((side == 0 ? 0 : -36) * npc.spriteDirection, 0) + RedeHelper.Spread(10), RedeHelper.PolarVector(15, armRot[side] + (npc.spriteDirection == -1 ? MathHelper.Pi : 0)), ModContent.ProjectileType<OO_BarrageMissile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);

                if (rocketFrame[0] <= 4)
                    (Main.npc[npc.whoAmI].ModNPC as OO).ArmRFrameY[0]++;
                else
                    (Main.npc[npc.whoAmI].ModNPC as OO).ArmRFrameY[1]++;

                if (rocketFrame[1] > 4)
                {
                    Projectile.localAI[0] = 0;
                    (Main.npc[npc.whoAmI].ModNPC as OO).ArmRFrameY[0] = 0;
                    (Main.npc[npc.whoAmI].ModNPC as OO).ArmRFrameY[1] = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(texture.Width / 2f, texture.Width / 2f);

            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(Color.White) * 0.7f, -Projectile.rotation, origin, Projectile.scale + 0.4f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}