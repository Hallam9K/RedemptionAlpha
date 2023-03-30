using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class BlastBattery_Crosshair : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OO_Crosshair";
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
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            DrawOffsetX = -18;
            DrawOriginOffsetY = -18;
        }
        NPC target;
        NPC locked;
        private int side;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 1f * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.4f * Projectile.Opacity);
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || Projectile.localAI[0] >= 5)
                Projectile.Kill();

            if (Projectile.ai[0] == 0)
            {
                switch (Projectile.ai[1])
                {
                    case 0:
                        if (RedeHelper.ClosestNPC(ref target, 300, Projectile.Center, true, player.MinionAttackTargetNPC))
                        {
                            locked = Main.npc[target.whoAmI];
                            Projectile.ai[1] = 1;
                        }
                        else
                        {
                            CombatText.NewText(player.getRect(), Color.Red, "No targets found!");
                            Projectile.Kill();
                        }
                        break;
                    case 1:
                        if (!locked.active)
                            Projectile.Kill();

                        Projectile.Center = locked.Center;
                        Projectile.alpha = 0;

                        Projectile.localAI[1]++;
                        if (Projectile.localAI[1] >= 30 && Projectile.localAI[1] % 5 == 0 && Projectile.localAI[1] < 60)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.MissileFire1, player.position);

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(player.Center.X + ((12 + side) * player.direction), player.Center.Y - 10), new Vector2(10 * player.direction, -5), ModContent.ProjectileType<BlastBattery_Missile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, 1);
                            
                            if (side == 0)
                                side = -30;
                            else
                                side = 0;
                        }
                        if (Projectile.localAI[1] >= 60 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<BlastBattery_Missile>()))
                            Projectile.Kill();
                        break;
                }
            }
            else
            {
                Projectile.alpha = 0;
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] == 30)
                {
                    if (Main.rand.NextBool(2))
                        side = -30;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.MissileFire1, player.position);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(player.Center.X + ((12 + side) * player.direction), player.Center.Y - 10), new Vector2(10 * player.direction, -5), ModContent.ProjectileType<BlastBattery_Missile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
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