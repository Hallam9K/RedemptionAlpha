
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_FireBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fire Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.scale = 0.1f;
                    Projectile.localAI[0] = 1;
                    break;
                case 1:
                    int mace = (int)Projectile.ai[0];
                    if (mace < 0 || mace >= 200 || !npc.active || npc.type != ModContent.NPCType<MACEProject>() || npc.ai[0] == 2)
                        Projectile.Kill();

                    Vector2 MouthPos = new(npc.Center.X, npc.Center.Y + 54);
                    Projectile.Center = MouthPos;

                    Projectile.scale += 0.01f;
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 10;
                    if (Projectile.scale > 2f)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                        Projectile.localAI[0] = 2;
                        Projectile.velocity = RedeHelper.PolarVector(20, (Main.player[npc.target].Center - npc.Center).ToRotation());
                        int pieCut = 32;
                        for (int m = 0; m < pieCut; m++)
                        {
                            int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.OrangeTorch, 0f, 0f, 100, Color.White, 5);
                            Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(14f, 0f), m / (float)pieCut * 6.28f);
                            Main.dust[dustID].noLight = false;
                            Main.dust[dustID].noGravity = true;
                        }
                        Projectile.timeLeft = 120;
                    }
                    break;
                case 2:
                    Projectile.hostile = true;
                    break;
            }
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.5f, Projectile.Opacity * 0.5f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 900);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.SolarDye);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Rectangle rect2 = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);
            if (Projectile.localAI[0] < 2)
            {
                Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.Orange) * 0.6f, Projectile.rotation, origin2, Projectile.scale * 2.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.Orange) * 0.6f, -Projectile.rotation, origin2, Projectile.scale * 2.5f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.OrangeTorch, 0f, 0f, 100, Color.White, 4);
                Main.dust[dustID].velocity *= 16;
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}
