using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.Projectiles.Magic;
using Redemption.Items.Tools.PreHM;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using ParticleLibrary.Core;
using Redemption.Particles;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class BladeOfTheMountain_Slash : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blade of the Mountain");
            Main.projFrames[Projectile.type] = 10;
            ElementID.ProjIce[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 106;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => Projectile.frame is 5;
        public override bool? CanHitNPC(NPC target) => Projectile.frame is 5 ? null : false;
        public float SwingSpeed;
        int directionLock = 0;
        private bool parried;
        public int pauseTimer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            SwingSpeed = SetSwingSpeed(25);

            Projectile.Redemption().swordHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 100 : Projectile.Center.X), (int)(Projectile.Center.Y - 70), 100, 136);

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
                }
                if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;
                    if (--pauseTimer <= 0)
                        Projectile.ai[0]++;
                    if (Projectile.frame > 3)
                        player.itemRotation -= MathHelper.ToRadians(-20f * player.direction);
                    else
                        player.bodyFrame.Y = 5 * player.bodyFrame.Height;
                    if (pauseTimer <= 0 && ++Projectile.frameCounter >= SwingSpeed / 10)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 5)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            player.velocity.X += 2 * player.direction;
                        }
                        if (Projectile.frame >= 5 && Projectile.frame <= 6)
                        {
                            foreach (Projectile target in Main.ActiveProjectiles)
                            {
                                if (target.ai[0] is 0 && (target.type == ModContent.ProjectileType<Icefall_Proj>() || target.type == ModContent.ProjectileType<Calavia_Icefall>() || target.type == ModContent.ProjectileType<IceSpikeShard>()) && Projectile.Redemption().swordHitbox.Intersects(target.Hitbox))
                                {
                                    DustHelper.DrawCircle(target.Center, DustID.IceTorch, 1, 2, 2, dustSize: 2, nogravity: true);
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.CrystalHit, Projectile.position);
                                    target.velocity.Y = Main.rand.NextFloat(-2, 0);
                                    target.velocity.X = player.direction * 18f;
                                    target.damage *= 2;
                                    target.friendly = true;
                                    target.ai[0] = 1;
                                    continue;
                                }
                                if (RedeProjectile.SwordClashFriendly(Projectile, target, player, ref parried))
                                    continue;

                                if (target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                                    continue;

                                if (target.velocity.Length() == 0 || !Projectile.Redemption().swordHitbox.Intersects(target.Hitbox) || (!target.HasElement(ElementID.Ice) && target.alpha > 0) || target.ProjBlockBlacklist(true))
                                    continue;

                                SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                                DustHelper.DrawCircle(target.Center, DustID.IceTorch, 1, 4, 4, nogravity: true);
                                if (target.hostile || target.friendly)
                                {
                                    target.hostile = false;
                                    target.friendly = true;
                                }
                                target.Redemption().ReflectDamageIncrease = 4;
                                target.velocity.X = -target.velocity.X * 0.9f;
                                RedeDraw.SpawnExplosion(target.Center, new Color(214, 239, 243), shakeAmount: 0, scale: .5f, noDust: true, rot: RedeHelper.RandomRotation(), tex: "Redemption/Textures/SwordClash");
                            }
                        }
                        if (Projectile.frame > 9)
                            Projectile.Kill();
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float tipBonus;
            tipBonus = player.Distance(target.Center) / 3;
            tipBonus = MathHelper.Clamp(tipBonus, 0, 20);

            modifiers.FlatBonusDamage += (int)tipBonus;
        }
        public bool paused;
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
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 10 + new Vector2(0, 40) + player.velocity, ModContent.DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f) + 3.14f + player.direction * MathHelper.PiOver4) * Main.rand.NextFloat(4f, 5f) + (player.velocity / 2), 0, new Color(214, 239, 243) * .8f, 2f);

            Vector2 dir = target.DirectionTo(player.Center);
            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.9f);
            ParticleSystem.NewParticle(drawPos, dir.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f) + player.direction * MathHelper.PiOver4) * 50, new SlashParticle(), Color.LightCyan, 1.8f, layer: Layer.BeforePlayers);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (target.DistanceSQ(player.Center) > 100 * 100 && target.knockBackResist > 0 && !target.RedemptionNPCBuff().iceFrozen)
            {
                SoundEngine.PlaySound(SoundID.Item30, target.position);
                DustHelper.DrawDustImage(target.Center, DustID.Frost, 0.5f, "Redemption/Effects/DustImages/Flake", 2, true, RedeHelper.RandomRotation());
                target.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(target.lifeMax, 60, 1780)));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 10;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int offset = Projectile.frame > 4 ? 16 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(40 * player.direction, 50 - offset),
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Melee/BladeOfTheMountain_SlashProj").Value;
            int height2 = slash.Height / 6;
            int y2 = height2 * (Projectile.frame - 5);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 5 && Projectile.frame <= 9)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition - new Vector2(0 * player.direction, -331 - offset), new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
