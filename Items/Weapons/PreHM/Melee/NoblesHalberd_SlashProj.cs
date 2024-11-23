using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class NoblesHalberd_SlashProj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Noble's Halberd");
            Main.projFrames[Projectile.type] = 6;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.Redemption().IsAxe = true;
        }
        public override bool? CanCutTiles() => Projectile.frame is 4;
        public override bool? CanHitNPC(NPC target) => Projectile.frame is 4 ? null : false;
        public float SwingSpeed;
        int directionLock = 0;
        public int pauseTimer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.Redemption().swordHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 78 : Projectile.Center.X), (int)(Projectile.Center.Y - 66), 78, 94);
            Point tileBelow = new Vector2(Projectile.Redemption().swordHitbox.Center.X + (30 * Projectile.spriteDirection), Projectile.Redemption().swordHitbox.Bottom).ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

            SwingSpeed = SetSwingSpeed(30);

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
                    if (++Projectile.frameCounter >= SwingSpeed / 6)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 2)
                        {
                            Projectile.frame = 2;
                        }
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    if (--pauseTimer <= 0)
                        Projectile.ai[0]++;
                    if (Projectile.frame > 2)
                        player.itemRotation -= MathHelper.ToRadians(-8f * player.direction);
                    else
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (pauseTimer <= 0 && ++Projectile.frameCounter >= SwingSpeed / 6)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 4)
                        {
                            if (tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
                            {
                                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                                player.RedemptionScreen().ScreenShakeIntensity += 5;
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            }
                            player.velocity.X += 2 * player.direction;
                        }
                        if (Projectile.frame > 5)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            Projectile.spriteDirection = player.direction;

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true);
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Redemption().swordHitbox;
        }
        private bool paused;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(CustomSounds.Slice4, Projectile.position);
            player.RedemptionScreen().ScreenShakeIntensity += 5;
            if (!paused)
            {
                pauseTimer = 6;
                paused = true;
            }
            Vector2 directionTo = target.DirectionTo(player.Center);
            for (int i = 0; i < 5; i++)
                Dust.NewDustPerfect(target.Center + directionTo + new Vector2(0, 35) + player.velocity, ModContent.DustType<DustSpark>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f) - MathHelper.PiOver2 + (Projectile.spriteDirection == 1 ? MathHelper.Pi : 0)) * -Main.rand.NextFloat(4f, 5f) + (player.velocity / 2), 0, new Color(188, 188, 152) * 0.8f, 3f);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit, 80);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0, 24),
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}