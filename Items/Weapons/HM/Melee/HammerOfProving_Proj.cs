using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class HammerOfProving_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/HammerOfProving";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer of Proving");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.GetGlobalProjectile<RedeProjectile>().IsHammer = true;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] >= 1 ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;
        private float swordRotation;

        public override void AI()
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                    Projectile.ai[0] = 1;
                    oldRotation = swordRotation;
                    directionLock = player.direction;
                }
                if (Projectile.ai[0] >= 1)
                {
                    if (Projectile.ai[0] > 4)
                        Projectile.alpha = 0;
                    player.direction = directionLock;

                    Projectile.ai[0]++;

                    float timer = Projectile.ai[0] - 1;
                    if (Projectile.ai[0] > 5)
                        Projectile.alpha = 0;

                    if (Projectile.ai[1] == 0)
                    {
                        if (Projectile.ai[0] < 29 * SwingSpeed)
                            swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / 30f / SwingSpeed);

                        if (Projectile.ai[0] >= 26 * SwingSpeed)
                        {
                            player.velocity.Y += 1;
                            Point tileBelow = new Vector2(Projectile.Center.X, Projectile.Bottom.Y).ToTileCoordinates();
                            Point tileBelow2 = new Vector2(player.Center.X, player.Bottom.Y).ToTileCoordinates();
                            Tile tile = Main.tile[tileBelow.X, tileBelow.Y];
                            Tile tile2 = Main.tile[tileBelow2.X, tileBelow2.Y];
                            if (((tile is { IsActiveUnactuated: true } && Main.tileSolid[tile.type]) || (tile2 is { IsActiveUnactuated: true } && Main.tileSolid[tile2.type])) && player.velocity.Y >= 0)
                            {
                                float volume = MathHelper.Lerp(0.1f, 1f, player.velocity.Y / 40);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/EarthBoom2").WithVolume(volume), Projectile.position);
                                for (int i = 0; i < 10; i++)
                                    Dust.NewDust(new Vector2(Projectile.position.X, Projectile.Bottom.Y), Projectile.width, 2, DustID.Stone,
                                        -player.velocity.X * 0.6f, -player.velocity.Y * 0.6f, Scale: 2);

                                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 2 * player.velocity.Y;
                                Projectile.ai[1] = 1;
                            }
                            if ((!player.channel || player.velocity.Y <= 0) && Projectile.ai[0] >= 34 && Projectile.ai[1] == 0)
                                Projectile.Kill();
                        }
                    }
                    else
                    {
                        Projectile.friendly = false;
                        if (Projectile.ai[1]++ >= 30)
                            Projectile.Kill();
                    }
                }
            }

            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 60f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else
                player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[Projectile.owner];
            if (player.velocity.Y > 0)
                damage = (int)(damage * ((player.velocity.Y / 10) + 1));
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.velocity.Y >= 20 && target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}