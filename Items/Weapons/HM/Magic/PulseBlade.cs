using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Items.Materials.HM;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class PulseBlade : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.HandsOn}", EquipType.HandsOn, Item.ModItem, null, new EquipTexture());
        }
        private void SetupDrawing()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.GetEquipSlot(Mod, Name, EquipType.HandsOn);
        }
        private int cooldown = 0;
        public override void SetStaticDefaults()
        {
            SetupDrawing();
        }
        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.height = 28;
            Item.width = 28;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.UseSound = null;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<KineticMine>();
        }
        public override void HoldItem(Player player)
        {
            var p = player.GetModPlayer<PulseBladePlayer>();
            p.VanityOn = true;
            cooldown--;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = Item.useAnimation = 15;
                Item.channel = false;
                int max = 8;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.type != Item.shoot || proj.owner != player.whoAmI || proj.localAI[1] is 0)
                        continue;
                    max++;
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<KineticMine>()] < max && cooldown <= 0)
                {
                    return true;
                }
            }
            else
            {
                Item.channel = true;
                return player.ownedProjectileCounts[ModContent.ProjectileType<PulseBlade_Proj>()] <= 0;
            }
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                cooldown = 15;
                Projectile.NewProjectile(source, position, velocity * 1.2f, ModContent.ProjectileType<KineticMine>(), damage, knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PulseBlade_Proj>(), damage, knockback, player.whoAmI, 0, 0, player.itemAnimationMax);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 4)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class PulseBladePlayer : ModPlayer
    {
        public bool VanityOn;

        public override void ResetEffects()
        {
            VanityOn = false;
        }
        public override void FrameEffects()
        {
            if (VanityOn)
            {
                var item = ModContent.GetInstance<PulseBlade>();
                Player.handon = (sbyte)EquipLoader.GetEquipSlot(Mod, item.Name, EquipType.HandsOn);
            }
        }
    }
    public class KineticMine : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OmegaPlasmaBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Plasma Orb");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ElementID.ProjFire[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2400;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public Vector2 strikePos;
        public Vector2 minePos;
        public float newVelocity;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1 * Projectile.Opacity, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity);
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.localAI[0]++ == 0)
            {
                Player player = Main.player[Projectile.owner];
                SoundEngine.PlaySound(CustomSounds.BallCreate with { Volume = .5f }, player.position);
                RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed);
                Projectile.scale = 0.1f;
            }
            if (Projectile.localAI[0] < 180 && Projectile.localAI[1] == 0)
            {
                Projectile.scale += 0.01f;
                Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.1f, 1);
                Projectile.velocity *= 0.95f;
            }
            if (Projectile.localAI[0] >= 60)
            {
                if (Projectile.velocity.Length() >= 5)
                    Projectile.friendly = true;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.alpha > 200 || !(proj.type == ModContent.ProjectileType<KineticSlash>()))
                        continue;

                    if (Helper.CheckCircularCollision(Projectile.Center, (int)(80 * proj.scale), proj.Hitbox))
                    {
                        if (Projectile.localAI[1]++ == 0)
                        {
                            SoundEngine.PlaySound(CustomSounds.BallFire, Projectile.position);
                            AdjustMagnitude(ref Projectile.velocity);
                            strikePos = proj.Center;
                            newVelocity = 4 + proj.velocity.Length();
                            Projectile.velocity = RedeHelper.PolarVector(newVelocity, proj.velocity.ToRotation());
                            float damage = Projectile.velocity.Length() / 5;
                            damage = MathHelper.Max(damage, 1);
                            Projectile.damage *= (int)damage;
                        }
                    }
                }
                if (Projectile.localAI[1] > 0)
                {
                    Projectile.localAI[1]++;
                    Vector2 move = Vector2.Zero;
                    float distance = 900f;
                    bool target = false;
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC npc = Main.npc[k];
                        if (npc.active && npc.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 0, 0, npc.Center, 0, 0) && !npc.Redemption().invisible)
                        {
                            Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                            float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                            if (distanceTo < distance)
                            {
                                move = newMove;
                                distance = distanceTo;
                                target = true;
                            }
                        }
                    }
                    if (target && Projectile.localAI[1] % 4 == 0)
                    {
                        AdjustMagnitude(ref move);
                        Projectile.velocity = (newVelocity * Projectile.velocity + move) / newVelocity;
                        AdjustMagnitude(ref Projectile.velocity);
                    }
                }
            }
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 2f)
            {
                vector *= newVelocity / magnitude;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;

            if (Projectile.localAI[1] > 0)
            {
                BlastSpawn(Projectile.velocity / 3);
                Projectile.Kill();
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BlastSpawn(Projectile.velocity / 3);
            Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(4, 4), ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: 3);
            dust2.noGravity = true;
            Color dustColor = new(Color.IndianRed.R, Color.IndianRed.G, Color.IndianRed.B) { A = 0 };
            dust2.color = dustColor;

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain);
                dust.velocity = -Projectile.DirectionTo(dust.position) * 2f;
            }
        }
        private void BlastSpawn(Vector2 vel)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel, Vector2.Zero, ModContent.ProjectileType<KineticMine_Explosion>(), Projectile.damage / 3, Projectile.knockBack, Main.myPlayer);

            DustHelper.DrawCircle(Projectile.Center + vel, DustID.OrangeTorch, 1, 2, 2, nogravity: true);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast, Projectile.position);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 8;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 24;
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f) * Projectile.Opacity;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(22, 22) + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Red * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), Projectile.GetAlpha(color) * 0.3f, Projectile.rotation, origin, Projectile.scale, 0, 0);
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, position, new Rectangle?(rect), RedeColor.RedPulse * 0.3f, Projectile.rotation, origin, Projectile.scale, 0);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class PulseBlade_Proj : TrueMeleeProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Length = 30;
            Rot = MathHelper.ToRadians(2);
        }
        private Vector2 startVector;
        private Vector2 vector;
        private Vector2 mouseOrig;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float glow;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Vector2 armCenter = player.RotatedRelativePoint(player.MountedCenter, true) + new Vector2(-player.direction * 3, -3);
            Projectile.Center = armCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case -1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 3)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = (glow / 2) + .1f, Pitch = .4f }, Projectile.position);
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, RedeHelper.PolarVector(20 * (glow + .4f), (mouseOrig - armCenter).ToRotation()), ModContent.ProjectileType<KineticSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Main.projectile[p].scale += glow;
                            Main.projectile[p].netUpdate = true;
                        }
                        if (Timer < 5)
                        {
                            Rot += speed * Projectile.spriteDirection;
                            speed += 0.25f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed * Projectile.spriteDirection;
                            speed *= 0.7f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 20)
                            Projectile.Kill();
                        break;

                    case 0:
                        if (Main.MouseWorld.X < player.Center.X)
                            player.direction = -1;
                        else
                            player.direction = 1;
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        speed = MathHelper.ToRadians(10);
                        startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - armCenter).ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                        vector = startVector * Length;
                        if (!player.channel)
                        {
                            Timer = 0;
                            Projectile.ai[0] = -1;
                            Projectile.netUpdate = true;
                            mouseOrig = Main.MouseWorld;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1 with { Volume = (glow / 4) + .1f, Pitch = .2f }, player.position);
                            SoundEngine.PlaySound(SoundID.Item71 with { Volume = (glow / 4) + .1f }, player.position);
                        }
                        if (Timer++ >= 15 && glow < 1)
                        {
                            glow += 0.2f;
                            Timer = 0;
                            RedeDraw.SpawnRing(Projectile.Center, Color.Red, 0.5f, 0.5f, 1.5f + (glow / 5));
                            RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed, 0.5f, 0.5f, glow / 5);
                            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Pitch = glow - .1f }, Projectile.position);
                        }
                        break;
                }
            }
        }
    }
    public class KineticSlash : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Melee/MythrilsBaneSlash_Proj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.scale = .5f;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, 0f, 0f);

            Projectile.velocity *= 0.98f;
            Projectile.alpha += 2;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition - drawOrigin + new Vector2(55, 71);
                Color color = Color.Red * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Red), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.IndianRed) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            return false;
        }
    }
    public class KineticMine_Explosion : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Ranged/PlasmaRound_Blast";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 6;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 104;
            Projectile.height = 104;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 2f;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 5;
            target.immune[Projectile.owner] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale * .5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.6f, Projectile.rotation, origin, Projectile.scale * 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}