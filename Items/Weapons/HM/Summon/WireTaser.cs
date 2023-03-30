using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class WireTaser : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("9 summon tag damage\n" +
                "Your summons will focus struck enemies\n" +
                "Strike enemies with the tip of the taser to stun them for a second\n" +
                "Cannot stun enemies with knockback immunity\n" +
                "Inflicts electrified"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 34;
            Item.DefaultToWhip(ModContent.ProjectileType<WireTaser_Proj>(), 60, 2, 12, 30);
            Item.shootSpeed = 12;
            Item.rare = ItemRarityID.LightPurple;
            Item.channel = true;
            Item.value = Item.sellPrice(0, 9, 55, 0);
        }
        public override bool MeleePrefix() => true;
    }
    public class WireTaser_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wire Taser");
            ProjectileID.Sets.IsAWhip[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 26;
            Projectile.WhipSettings.RangeMultiplier = .6f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        private int FrameX;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool())
                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 180);

            if (player.whoAmI == Main.myPlayer && target.DistanceSQ(player.Center) > 320 * 320 && target.life > 0)
            {
                SoundEngine.PlaySound(CustomSounds.ElectricNoise with { Pitch = 1.5f }, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<WireTaser_Proj2>(), Projectile.damage / 2, Projectile.knockBack, player.whoAmI, target.whoAmI);
                Projectile.Kill();
            }
            target.AddBuff(BuffID.SwordWhipNPCDebuff, 180);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
        private int soundTimer;
        public override void PostAI()
        {
            if (soundTimer++ == 26)
                SoundEngine.PlaySound(CustomSounds.Spark1, Projectile.position);
            if (soundTimer > 26 && ++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++FrameX >= 9)
                    FrameX = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            //DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int width = texture.Width / 10;
            int x = width * FrameX;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(x, 0, width, 26);
                Vector2 origin = new(width / 2, 12);
                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 116;
                    frame.Height = 24;
                }
                else if (i % 3 == 0)
                {
                    frame.Y = 92;
                    frame.Height = 16;
                }
                else if ((i + 2) % 3 == 0)
                {
                    frame.Y = 64;
                    frame.Height = 16;
                }
                else if (i > 0 && (i + 1) % 3 == 0)
                {
                    frame.Y = 36;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                Main.EntitySpriteDraw(glow, pos - Main.screenPosition, frame, Color.White, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
    public class WireTaser_Proj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wire Taser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.ownedProjectileCounts[Type] > 1)
                Projectile.Kill();

            float num = MathHelper.ToRadians(0f);
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
                num = MathHelper.ToRadians(180f);

            if (Main.myPlayer == Projectile.owner)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == Projectile.type)
                {
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                }
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                {
                    vector13 = Vector2.UnitX * player.direction;
                }
                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = vector13;
                if (player.noItems || player.CCed || player.dead || !player.active)
                {
                    Projectile.Kill();
                }
                Projectile.netUpdate = true;
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(4, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Projectile.localAI[0]++ == 0 && Projectile.owner == Main.myPlayer)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                Projectile.alpha = 0;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WireTaser_Proj2_End>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.whoAmI, target.whoAmI);
            }
            else if (Projectile.localAI[0] >= 2 && player.ownedProjectileCounts[ModContent.ProjectileType<WireTaser_Proj2_End>()] <= 0)
                Projectile.Kill();
        }
    }
    public class WireTaser_Proj2_End : ModProjectile
    {
        private static Asset<Texture2D> chainTexture;
        private static Asset<Texture2D> chainGlow;
        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Summon/WireTaser_Proj2_Chain");
            chainGlow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Summon/WireTaser_Proj2_Chain_Glow");
        }
        public override void Unload()
        {
            chainTexture = null;
            chainGlow = null;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wire Taser");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++FrameX >= 9)
                    FrameX = 0;
                if (++ChainFrameX >= 9)
                    ChainFrameX = 0;
            }
            Projectile handle = Main.projectile[(int)Projectile.ai[0]];
            NPC target = Main.npc[(int)Projectile.ai[1]];
            if (!handle.active)
                Projectile.Kill();

            Projectile.rotation = (handle.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                switch (Projectile.localAI[0])
                {
                    case 0:
                        if (Projectile.localAI[1]++ >= 60 || !target.active || Projectile.DistanceSQ(handle.Center) >= 900 * 900)
                        {
                            Projectile.localAI[0] = 1;
                            break;
                        }
                        Projectile.Center = target.Center;
                        break;
                    case 1:
                        Projectile.Move(handle.Center, 40, 1);
                        if (Projectile.DistanceSQ(handle.Center) < 20 * 20)
                            Projectile.Kill();
                        break;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 20);
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 180);

            Projectile.localNPCImmunity[target.whoAmI] = 5;
            target.immune[Projectile.owner] = 0;
        }
        private int FrameX;
        private int ChainFrameY;
        private int ChainFrameX;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int width = texture.Width / 10;
            int x = width * FrameX;
            Rectangle rect = new(x, 0, width, Projectile.height);
            Vector2 drawOrigin = new(width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override bool PreDrawExtras()
        {
            int height = chainTexture.Value.Height / 3;
            int width = chainTexture.Value.Width / 10;
            int y = height * ChainFrameY;
            int x = width * ChainFrameX;

            Vector2 handleCenter = Main.projectile[(int)Projectile.ai[0]].Center;
            Vector2 center = Projectile.Center;
            Vector2 directionToHandle = handleCenter - Projectile.Center;
            float chainRotation = directionToHandle.ToRotation() - MathHelper.PiOver2;
            float distanceToHandle = directionToHandle.Length();

            while (distanceToHandle > 20f && !float.IsNaN(distanceToHandle))
            {
                ChainFrameY++;
                if (ChainFrameY > 2)
                    ChainFrameY = 0;
                directionToHandle /= distanceToHandle; //get unit vector
                directionToHandle *= height - 2; //multiply by chain link length

                center += directionToHandle; //update draw position
                directionToHandle = handleCenter - center; //update distance
                distanceToHandle = directionToHandle.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                Rectangle frame = new(x, y, width, height);
                Vector2 origin = new(width / 2, height / 2);
                //Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition, frame, drawColor, chainRotation, origin, 1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(chainGlow.Value, center - Main.screenPosition, frame, Color.White, chainRotation, origin, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}