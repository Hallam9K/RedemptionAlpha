using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.NPCs.Friendly.TownNPCs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public abstract class BaseSwordSlicer_Slash : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/SwordSlicer_Slash";
        protected abstract Entity Owner { get; }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            if (Owner is NPC)
                Projectile.npcProj = true;
        }

        public override bool? CanCutTiles() => Projectile.frame is 5;
        public override bool? CanHitNPC(NPC target)
        {
            if (Owner is Player)
                return !target.friendly && Projectile.frame is 5 ? null : false;
            return !target.friendly && target.type != NPCID.TargetDummy && Projectile.frame is 5 ? null : false;
        }

        private float SwingSpeed;
        int directionLock = 0;
        private bool parried;
        private int pauseTimer;
        public override void AI()
        {
            Player player = Owner as Player;
            NPC npc = Owner as NPC;

            if (player != null)
                player.heldProj = Projectile.whoAmI;
            Projectile.Redemption().swordHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 90 : Projectile.Center.X), (int)(Projectile.Center.Y - 67), 90, 126);
            SwingSpeed = player != null ? SetSwingSpeed(26) : 26;

            if (player != null && (player.noItems || player.CCed || player.dead || !player.active))
            {
                Projectile.Kill();
                return;
            }
            if (npc != null && (!npc.active || npc.type != ModContent.NPCType<Zephos>() || npc.ai[1] <= 1))
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    if (player != null)
                    {
                        player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    }
                    Projectile.ai[0] = 1;
                    directionLock = Owner.direction;
                }
                if (Projectile.ai[0] >= 1)
                {
                    Owner.direction = directionLock;
                    if (--pauseTimer <= 0)
                        Projectile.ai[0]++;
                    if (player != null)
                    {
                        if (Projectile.frame > 4)
                            player.itemRotation -= MathHelper.ToRadians(-25f * player.direction);
                        else
                            player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    }
                    if (pauseTimer <= 0 && ++Projectile.frameCounter >= SwingSpeed / 9)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 5)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile target = Main.projectile[i];
                                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile)
                                    continue;

                                if (RedeProjectile.SwordClashFriendly(Projectile, target, player, ref parried))
                                    continue;

                                if (target.damage > 100 / 4 || Projectile.alpha > 0 || target.width + target.height > Projectile.width + Projectile.height)
                                    continue;

                                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || target.alpha > 0 || target.ProjBlockBlacklist(true))
                                    continue;

                                SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                                DustHelper.DrawCircle(target.Center, DustID.SilverCoin, 1, 4, 4, nogravity: true);
                                target.Kill();
                            }
                        }
                        if (Projectile.frame > 8)
                            Projectile.Kill();
                    }
                }
            }

            Projectile.spriteDirection = Owner.direction;

            Projectile.Center = player != null ? player.MountedCenter : Owner.Center;
            if (player != null)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Redemption().swordHitbox;
        }
        private bool paused;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CustomSounds.Slice3 with { Pitch = -.1f }, Projectile.position);
            if (Owner is Player player)
                player.RedemptionScreen().ScreenShakeIntensity += 3;
            if (!paused)
            {
                pauseTimer = 5;
                paused = true;
            }
            Vector2 directionTo = target.DirectionTo(Owner.Center);
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 10 + new Vector2(0, 40) + Owner.velocity, ModContent.DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f) + 3.14f + Owner.direction * MathHelper.PiOver4) * Main.rand.NextFloat(4f, 5f) + (Owner.velocity / 2), 0, new Color(214, 239, 243) * .8f, 2f);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (NPCLists.Armed.Contains(target.type))
                target.AddBuff(ModContent.BuffType<DisarmedDebuff>(), 1800);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 9;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int offset = Projectile.frame > 4 ? -6 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(-1 * Owner.direction, 40 - offset) + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Melee/SwordSlicer_SlashProj").Value;
            int height2 = slash.Height / 2;
            int y2 = height2 * (Projectile.frame - 5);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 5)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition - new Vector2(-40 * Owner.direction, -63 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class SwordSlicer_Slash : BaseSwordSlicer_Slash
    {
        protected override Entity Owner => Main.player[Projectile.owner];
    }
}
