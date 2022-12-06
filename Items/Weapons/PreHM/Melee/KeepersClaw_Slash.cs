using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class KeepersClaw_Slash : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keeper's Claw");
            Main.projFrames[Projectile.type] = 6;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => Projectile.frame is 2;
        public override bool? CanHitNPC(NPC target) => Projectile.frame is 2 ? null : false;
        public float SwingSpeed;
        int directionLock = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;

            SwingSpeed = SetSwingSpeed(30);

            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                    player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    Projectile.ai[0] = 1;
                    directionLock = player.direction;
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    Projectile.ai[0]++;
                    if (Projectile.frame > 1)
                        player.itemRotation -= MathHelper.ToRadians(-25f * player.direction);
                    else
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (++Projectile.frameCounter >= SwingSpeed / 6)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 2)
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);

                        if (Projectile.frame > 5)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            Projectile.spriteDirection = player.direction;

            Projectile.Center = player.Center;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 63 : Projectile.Center.X), (int)(Projectile.Center.Y - 55), 63, 104);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (NPCLists.Undead.Contains(target.type) || NPCLists.Skeleton.Contains(target.type))
                damage = (int)(damage * 2f);

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<NecroticGougeDebuff>(), 600);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int offset = Projectile.frame > 1 ? 14 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(-1 * player.direction, 6 - offset) + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Melee/KeepersClaw_SlashProj").Value;
            int height2 = slash.Height / 3;
            int y2 = height2 * (Projectile.frame - 2);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 2)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition - new Vector2(-31 * player.direction, -99 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}