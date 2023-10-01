using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using System;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Projectiles.Magic;
using Redemption.NPCs.Minibosses.Calavia;
using Terraria.Graphics.Shaders;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class Calavia_SS_BladeOfTheMountain : Calavia_BladeOfTheMountain
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/BladeOfTheMountain_Slash";
        public override void SetStaticDefaults() => base.SetSafeDefaults();
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanHitNPC(NPC target) => Projectile.frame is 5 ? null : false;
        public override bool PreAI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.ai[0] is 4 or 9 or 10 || npc.type != ModContent.NPCType<Calavia_SS>())
                Projectile.Kill();

            Projectile.Redemption().swordHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 100 : Projectile.Center.X), (int)(Projectile.Center.Y - 70), 100, 136);

            if (npc.ModNPC is Calavia_SS calavia)
            {
                if (Projectile.localAI[0] == 0)
                {
                    calavia.BodyFrame = 5 * frameHeight;
                    if (Projectile.localAI[1]++ >= Projectile.ai[1])
                    {
                        Projectile.localAI[1] = 0;
                        Projectile.localAI[0] = 1;
                    }
                }
                if (Projectile.localAI[0] >= 1)
                {
                    Projectile.localAI[1]++;
                    if (Projectile.frame is 4)
                        calavia.BodyFrame = 2 * frameHeight;
                    else if (Projectile.frame is 5)
                        calavia.BodyFrame = 3 * frameHeight;
                    else if (Projectile.frame > 5)
                        calavia.BodyFrame = 4 * frameHeight;
                    else
                        calavia.BodyFrame = 5 * frameHeight;
                    if (++Projectile.frameCounter >= SwingSpeed / 10)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame is 5)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            npc.velocity.X += 2 * npc.spriteDirection;
                        }
                        if (Projectile.frame >= 5 && Projectile.frame <= 6)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile target = Main.projectile[i];
                                if (!target.active)
                                    continue;

                                if (target.ai[0] is 0 && (target.type == ModContent.ProjectileType<Icefall_Proj>() || target.type == ModContent.ProjectileType<Calavia_Icefall>()) && Projectile.Redemption().swordHitbox.Intersects(target.Hitbox))
                                {
                                    DustHelper.DrawCircle(target.Center, DustID.IceTorch, 1, 2, 2, dustSize: 2, nogravity: true);
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.CrystalHit, Projectile.position);
                                    target.velocity.Y = Main.rand.NextFloat(-2, 0);
                                    target.velocity.X = npc.spriteDirection * 18f;
                                    target.ai[0] = 1;
                                    continue;
                                }

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
                            }
                        }
                        if (Projectile.frame > 9)
                            Projectile.Kill();
                    }
                }
            }

            Projectile.spriteDirection = npc.spriteDirection;
            Projectile.Center = npc.Center;
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 4;
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            float tipBonus;
            tipBonus = npc.Distance(target.Center) / 3;
            tipBonus = MathHelper.Clamp(tipBonus, 0, 20);

            modifiers.FlatBonusDamage += (int)tipBonus;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (target.DistanceSQ(npc.Center) > 100 * 100 && target.knockBackResist > 0 && !target.RedemptionNPCBuff().iceFrozen)
            {
                SoundEngine.PlaySound(SoundID.Item30, target.position);
                DustHelper.DrawDustImage(target.Center, DustID.Frost, 0.5f, "Redemption/Effects/DustImages/Flake", 2, true, RedeHelper.RandomRotation());
                target.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(target.lifeMax, 60, 1780)));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            NPC npc = Main.npc[(int)Projectile.ai[0]];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 10;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int offset = Projectile.frame > 4 ? 16 : 0;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(40 * npc.spriteDirection, 50 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Texture2D slash = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Melee/BladeOfTheMountain_SlashProj").Value;
            int height2 = slash.Height / 6;
            int y2 = height2 * (Projectile.frame - 5);
            Rectangle rect2 = new(0, y2, slash.Width, height2);
            Vector2 drawOrigin2 = new(slash.Width / 2, slash.Height / 2);

            if (Projectile.frame >= 5 && Projectile.frame <= 9)
                Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition - new Vector2(0 * npc.spriteDirection, -331 - offset) + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class Calavia_SS_BladeOfTheMountain2 : Calavia_BladeOfTheMountain2
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/BladeOfTheMountain";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blade of the Mountain");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjIce[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanHitNPC(NPC target) => Projectile.ai[1] > 1 && Projectile.ai[1] != 3 && Projectile.ai[1] != 4 ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        private Vector2 startVector;
        private Vector2 vector;
        private float speed;
        private float glow;
        Entity npcTarget;
        public override bool PreAI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.Redemption().attacker != null)
                npcTarget = npc.Redemption().attacker;
            if (!npc.active || npc.type != ModContent.NPCType<Calavia_SS>())
                Projectile.Kill();

            if (npc.ModNPC is Calavia_SS calavia && npcTarget != null)
            {
                switch (Projectile.ai[1])
                {
                    case 0:
                        Projectile.scale = 0.1f;
                        calavia.customArmRot = (npc.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() + ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                        vector = startVector * Length;
                        Projectile.ai[1] = 1;
                        Projectile.netUpdate = true;
                        break;
                    case 1:
                        calavia.customArmRot = (npc.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
                        speed = (float)Math.Sin(Timer / 3) / 6;
                        if (Timer < 20)
                            startVector = RedeHelper.PolarVector(1, (npcTarget.Center - npc.Center).ToRotation() + speed);
                        vector = startVector * Length;
                        if (Timer++ >= 2)
                            Length += 1f;
                        if (Projectile.scale < 1)
                            Projectile.scale += .06f;
                        if (Timer == 5)
                            npc.velocity = new Vector2(-3 * npc.spriteDirection, -2);
                        if (Timer >= 25)
                        {
                            Projectile.scale = 1;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Timer = 0;
                            Projectile.ai[1] = 2;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        npc.ai[2] = 1;
                        if (Timer++ <= 10)
                            npc.velocity = vector / 3;

                        vector = startVector * Length;
                        bool tileBonk = Collision.SolidCollision(Projectile.Center - new Vector2(6, 6), 12, 12);
                        if (Timer >= 20 || tileBonk)
                        {
                            npc.velocity *= .5f;
                            if (tileBonk && Projectile.owner == Main.myPlayer)
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;

                                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Pitch = .1f, Volume = .5f }, Projectile.position);
                                Collision.HitTiles(Projectile.Center - new Vector2(6, 6), vector / 3, 12, 12);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vector / 2, ModContent.ProjectileType<Calavia_SS_BladeStab>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                            }
                            npc.ai[2] = 2;
                            Projectile.Kill();
                        }
                        break;
                    case 3:
                        Projectile.scale = 0.1f;
                        calavia.customArmRot = (npc.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
                        startVector = RedeHelper.PolarVector(1, -MathHelper.PiOver2);
                        Length = 40;
                        vector = startVector * Length;
                        Projectile.ai[1] = 4;
                        Projectile.netUpdate = true;
                        break;
                    case 4:
                        calavia.customArmRot = (npc.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
                        speed = (float)Math.Sin(Timer / 20) / 5;
                        startVector = RedeHelper.PolarVector(1, -MathHelper.PiOver2 + speed);
                        vector = startVector * Length;
                        if (Timer++ >= 2 && Timer < 20)
                            Length += 1f;
                        if (Projectile.scale < 1)
                            Projectile.scale += .06f;
                        glow += 0.01f;
                        glow = MathHelper.Min(glow, 1);
                        if (Timer == 60)
                        {
                            RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                            RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f);
                            SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                        }
                        if (Timer >= 80)
                        {
                            speed = MathHelper.ToRadians(6);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Calavia_ArcticWind>(), 0, 0, Main.myPlayer, npc.whoAmI);
                            Projectile.scale = 1;
                            Timer = 0;
                            Projectile.ai[1] = 5;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 5:
                        if (Timer++ < 2)
                        {
                            Rot += speed * Projectile.spriteDirection;
                            speed += 0.25f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer > 80)
                            speed *= 0.9f;
                        else
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile target = Main.projectile[i];
                                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 100)
                                    continue;

                                if (Projectile.DistanceSQ(target.Center) > 140 * 140)
                                    continue;

                                if (target.velocity.Length() == 0 || (!target.HasElement(ElementID.Ice) && target.alpha > 0) || target.ProjBlockBlacklist(true))
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
                            }
                        }
                        if (Timer >= 120)
                            Projectile.Kill();
                        break;
                }
            }
            Projectile.Center = npc.Center + vector;

            Projectile.spriteDirection = npc.spriteDirection;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - npc.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - npc.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Timer > 1)
                Projectile.alpha = 0;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(10, (Projectile.Center - npc.Center).ToRotation());

            if (glow > 0)
            {
                float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Color.LightBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos - v, null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
                }
            }
            else
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Color.LightBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos - v, null, color * .5f, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
                }
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class Calavia_SS_BladeStab : Calavia_BladeStab
    {
        public override string Texture => "Redemption/NPCs/Minibosses/Calavia/Calavia_BladeStab";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.frame < 2;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 120);
    }
}
