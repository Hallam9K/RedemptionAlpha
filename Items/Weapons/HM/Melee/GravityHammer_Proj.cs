using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.ModLoader;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.BaseExtension;

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
            Projectile.Redemption().IsHammer = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public float SwingSpeed;
        int directionLock = 0;
        private bool miss = false;
        private Vector2 SlamOrigin;
        private float dist;
        private float tilePosY;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Rectangle projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 96 : Projectile.Center.X), (int)(Projectile.Center.Y - 66), 96, 94);

            SwingSpeed = SetSwingSpeed(52);

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
                            tilePosY = BaseWorldGen.GetFirstTileFloor((int)(player.Center.X / 16), (int)(player.Center.Y / 16));
                            dist = (tilePosY * 16) - (int)player.Center.Y;
                            player.velocity.X += 2 * player.direction;
                            player.velocity.Y += 10;
                        }
                        if (Projectile.frame > 11)
                            Projectile.Kill();
                    }
                    if (Projectile.frame >= 8 && Projectile.frame <= 10 && SlamOrigin == Vector2.Zero)
                    {
                        player.velocity.Y = 20;
                        player.position.Y += dist / 10;
                        dist = MathHelper.Clamp(dist, 0, 500);
                        if (player.position.Y >= tilePosY * 16 - 32)
                            player.position.Y = tilePosY * 16 - 32;
                        Point tileBelow = new Vector2(projHitbox.Center.X + (30 * Projectile.spriteDirection), projHitbox.Bottom).ToTileCoordinates();
                        Point tileBelow2 = new Vector2(projHitbox.Center.X + (16 * Projectile.spriteDirection), projHitbox.Bottom).ToTileCoordinates();
                        Tile tile = Main.tile[tileBelow.X, tileBelow.Y];
                        Tile tile2 = Main.tile[tileBelow2.X, tileBelow2.Y];
                        if ((tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType]) || (tile2 is { HasUnactuatedTile: true } && Main.tileSolid[tile2.TileType]))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/GravityHammerSlam").WithVolume(0.6f), Projectile.position);
                            player.RedemptionScreen().ScreenShakeIntensity = 20;
                            SlamOrigin = new(Projectile.Center.X + (70 * Projectile.spriteDirection), Projectile.Center.Y);
                            miss = false;
                        }
                        else if (!miss)
                        {
                            miss = true;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        }
                    }
                }
            }
            if (Projectile.frame >= 8 && !miss)
            {
                if (Projectile.localAI[1]++ % 2 == 0 && Projectile.frame <= 11)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 origin = SlamOrigin;
                        origin.X += Projectile.localAI[1] * 16 * i;
                        int numtries = 0;
                        int x = (int)(origin.X / 16);
                        int y = (int)(origin.Y / 16);
                        while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                        {
                            y++;
                            origin.Y = y * 16;
                        }
                        while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                        {
                            numtries++;
                            y--;
                            origin.Y = y * 16;
                        }
                        if (numtries >= 20)
                            break;

                        if (Main.netMode != NetmodeID.Server && Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), origin + new Vector2(0, -20), Vector2.Zero, ModContent.ProjectileType<GravityHammer_GroundShock>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, SlamOrigin.X);
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
            int offset = Projectile.frame >= 8 ? 8 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(-8 * player.direction, 48 - offset) + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Effect").Value;
            int height2 = slash.Height / 10;
            int y2 = height2 * (Projectile.frame - 2);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 2 && Projectile.frame <= 12 && !player.channel)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition + new Vector2(38 * player.direction, 471 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 96 : Projectile.Center.X), (int)(Projectile.Center.Y - 66), 96, 94);
            return Projectile.frame is 8 && projHitbox.Intersects(targetHitbox);
        }
    }
}