using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class GravityHammer_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gravity Hammer");
            Main.projFrames[Projectile.type] = 11;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<RedeProjectile>().IsHammer = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public float SwingSpeed;
        int directionLock = 0;
        private bool miss = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Rectangle projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 96 : Projectile.Center.X), (int)(Projectile.Center.Y - 66), 96, 94);
            Point tileBelow = new Vector2(projHitbox.Center.X + (30 * Projectile.spriteDirection), projHitbox.Bottom).ToTileCoordinates();
            Tile tile = Main.tile[tileBelow.X, tileBelow.Y];

            SwingSpeed = SetSwingSpeed(40);

            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                    player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        directionLock = player.direction;
                    }
                    if (++Projectile.frameCounter >= SwingSpeed / 11)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 3)
                            Projectile.frame = 3;
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    Projectile.ai[0]++;
                    if (Projectile.frame > 2)
                        player.itemRotation -= MathHelper.ToRadians(-4f * player.direction);
                    else
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (++Projectile.frameCounter >= SwingSpeed / 11)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 8)
                        {
                            if (tile is { IsActiveUnactuated: true } && Main.tileSolid[tile.type])
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AdamStrike").WithVolume(0.6f), Projectile.position);
                                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 20;
                            }
                            else
                            {
                                miss = true;
                                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            }
                            player.velocity.X += 2 * player.direction;
                        }
                        if (Projectile.frame > 11)
                            Projectile.Kill();
                    }
                }
            }

            Projectile.spriteDirection = player.direction;

            Projectile.Center = player.Center;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 11;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(-8 * player.direction, 48) + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Effect").Value;
            int height2 = slash.Height / 10;
            int y2 = height2 * (Projectile.frame - 2);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 2 && Projectile.frame <= 12 && !player.channel && !miss)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition + new Vector2(38 * player.direction, 471) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 96 : Projectile.Center.X), (int)(Projectile.Center.Y - 66), 96, 94);
            return Projectile.frame is 8 && projHitbox.Intersects(targetHitbox);
        }
    }
}