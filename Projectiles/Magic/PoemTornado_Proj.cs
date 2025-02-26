using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class PoemTornado_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TornadoTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tornado");
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            ElementID.ProjWind[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Redemption().ParryBlacklist = true;

            Projectile.width = 120;
            Projectile.height = 150;
            Projectile.alpha = 255;
            Projectile.hide = true;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.penetrate = -1;
            Projectile.scale = 2;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        private int Frame1;
        private int Frame2 = 1;
        private int Frame3 = 2;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.frameCounter++ % 4 == 0)
            {
                if (++Frame1 > 4)
                    Frame1 = 0;
                if (++Frame2 > 4)
                    Frame2 = 0;
                if (++Frame3 > 4)
                    Frame3 = 0;
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 90, 255);
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel)
                    Projectile.alpha -= 3;
                else
                    Projectile.alpha += 6;

                Projectile.Move(Main.MouseWorld, 24, 40);
                if (Projectile.localAI[0]++ % 360 == 0 && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.WindLong with { Pitch = -1 }, Projectile.position);
                if (Projectile.localAI[0] >= 20 && Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Rectangle left = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            Rectangle right = new((int)Projectile.position.X + (Projectile.width / 2), (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.knockBackResist <= 0 || npc.boss)
                    continue;

                float dist = npc.Distance(Projectile.Center);
                if (left.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X += 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
                if (right.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X -= 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
            }

            if (Projectile.alpha <= 90 && Projectile.localAI[0] % 10 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<PoemTornado_Rock>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new((int)Projectile.position.X + 60, (int)Projectile.position.Y, 88, Projectile.height);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.alpha <= 200 ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int width = texture.Width / 5;
            int x = width * Frame1;
            int x2 = width * Frame2;
            int x3 = width * Frame3;
            Rectangle rect = new(x, 0, width, texture.Height);
            Rectangle rect2 = new(x2, 0, width, texture.Height);
            Rectangle rect3 = new(x3, 0, width, texture.Height);
            Vector2 drawOrigin = new(width / 2, texture.Height / 2);

            Color c = lightColor * Projectile.Opacity * 0.3f;
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect), c, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect2), c, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect3), c, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(rect3), c, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);

            return false;
        }
    }
    public class PoemTornado_Rock : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private Projectile Tornado => Main.projectile[(int)Projectile.ai[0]];
        private ref float AttackState => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];

        private float progress;
        public Vector2[] oldPos = new Vector2[6];

        private float random;
        public override string Texture => "Redemption/Projectiles/Magic/Rockslide_Proj";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.alpha = 255;
            Projectile.rotation = RedeHelper.RandomRotation();
            Projectile.frame = Main.rand.Next(4);
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            Projectile.scale = Main.rand.NextFloat(0.5f, 1f);

            Projectile.hostile = false;
            Projectile.friendly = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            random = RedeHelper.RandomRotation();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            float period = random + progress * 6.28f * 2;
            float x = MathF.Cos(period) > 0 ? 1 : -1;
            if (x > 0 && AttackState == 0)
            {
                behindNPCsAndTiles.Add(index);
                return;
            }
            if (behindProjectiles.Contains(index))
                behindProjectiles.Remove(index);
            behindProjectiles.Add(index);
        }

        public override bool? CanCutTiles() => Projectile.friendly;
        public override void AI()
        {
            progress = Timer / 180f;
            Projectile.friendly = Projectile.alpha <= 0;
            Projectile.tileCollide = Projectile.alpha <= 0;
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            Projectile.rotation += 0.01f * Projectile.spriteDirection;

            if (AttackState is 0)
            {
                Projectile.timeLeft = 120;

                float moveX = progress * 200;
                float moveY = progress * 250;
                float period = random + progress * 6.28f * 2;
                Vector2 path = new Vector2(moveX * MathF.Sin(period), -moveY);

                if (progress <= 1)
                {
                    Projectile.Center = Tornado.Bottom + Vector2.UnitY * -40 + path;
                    AttackState = 0;
                }
                else
                {
                    NPC target = null;
                    if (RedeHelper.ClosestNPC(ref target, 800, Projectile.Center, true))
                    {
                        Projectile.velocity = Projectile.Center.DirectionTo(target.Center) * 16;
                        AttackState = 1;
                    }
                    else
                    {
                        float x = MathF.Cos(period) > 0 ? 1 : -1;
                        Projectile.velocity = new Vector2(x * 12, -2);
                        AttackState = 2;
                    }
                }

                if (!Owner.channel)
                {
                    float x = MathF.Cos(period) > 0 ? 1 : -1;
                    Projectile.velocity = new Vector2(x * 12, -2);
                    AttackState = 3;
                }

                Vector2 dustVelocity = path.SafeNormalize(Vector2.Zero);
                Dust d = Dust.NewDustDirect(Projectile.position, 4, 4, DustID.Stone, dustVelocity.X * 0, dustVelocity.Y * 0, Scale: 0.75f);
                d.noGravity = true;
            }
            if (AttackState == 2)
            {
                Projectile.velocity.Y += 0.1f;
            }

            if (AttackState == 3)
            {
                if (Projectile.localAI[0]++ < 20)
                    Projectile.velocity *= 0.9f;
                else
                    Projectile.velocity.Y += 0.2f;
            }
            Timer++;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldPos[k] = oldPos[k - 1];
            oldPos[0] = Projectile.Center;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f, Scale: 1.5f);
            }
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 drawOrigin = rect.Size() * 0.5f;
            for (int k = 0; k < oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                Color color = Projectile.GetAlpha(lightColor) * ((oldPos.Length - k) / (float)oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color * .5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    #region Alt
    /*
    public class PoemTornado_Proj : ModProjectile
    {
        private Player Player => Main.player[Projectile.owner];

        private float collisionLength;
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            ElementID.ProjWind[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Redemption().ParryBlacklist = true;

            Projectile.width = 200;
            Projectile.height = 250;
            Projectile.alpha = 255;
            Projectile.hide = true;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.penetrate = -1;

            collisionLength = 250;
        }
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 90, 255);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Player.channel)
                {
                    Projectile.alpha -= 3;
                }
                else
                {
                    Projectile.alpha += 6;
                }
                Projectile.Move(Main.MouseWorld, 24, 40);
                if (Projectile.localAI[0]++ % 180 == 0 && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.WindLong, Projectile.position);
                if (Projectile.localAI[0] >= 20 && Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Rectangle left = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            Rectangle right = new((int)Projectile.position.X + (Projectile.width / 2), (int)Projectile.position.Y, Projectile.width / 2, Projectile.height);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.knockBackResist <= 0 || npc.boss)
                    continue;

                float dist = npc.Distance(Projectile.Center);
                if (left.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X += 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
                if (right.Intersects(npc.Hitbox))
                {
                    npc.velocity *= 0.98f;
                    npc.velocity.X -= 0.8f * npc.knockBackResist;
                    npc.velocity.Y -= 0.6f * npc.knockBackResist * (dist / 70);
                }
            }
            if (Projectile.alpha <= 90 && Projectile.localAI[0] % 10 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<PoemTornado_Rock>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
            }
            if (!Main.dedServ)
            {
                TrailSetUp();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength = collisionLength;
            float maxAngle = MathHelper.ToRadians(15);
            float rotation = Projectile.velocity.ToRotation();
            Vector2 collisionStart = Projectile.Center;

            for (int i = -1; i < 2; i++)
            {
                if (!targetHitbox.IntersectsConeSlowMoreAccurate(collisionStart, MathF.Max(30, coneLength - MathF.Abs(i) * 25), rotation + MathHelper.ToRadians(i * 10), maxAngle))
                    continue;

                return true;
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target) => Projectile.alpha <= 200 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 offset = Player.MountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(default) * 34;
            int d = Dust.NewDust(target.Center + Main.rand.NextVector2Square(-10, 10), 2, 2, DustID.WhiteTorch, newColor: Color.White);
            Main.dust[d].velocity = (Projectile.Center + offset).DirectionFrom(target.Center) * (Projectile.Center - target.Center).Length() * 0.1f;
            Main.dust[d].noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            return false;
        }
            
        #region draw trail
        private List<Vector2> cache = new();
        private DanTrail trail;
        private int NumPoints = 200;
        public void TrailSetUp()
        {
            if (NumPoints > 0)
                ManageCache();
            if (cache.Count > 3)
                ManageTrail();
        }
        public void ManageCache()
        {
            Vector2 Center = Projectile.Bottom;

            cache = new List<Vector2>();

            for (float i = 0; i < NumPoints; i++)
            {
                Vector2 result = Vector2.UnitX * 10 + Vector2.UnitX * i * collisionLength / 200 * 1.1f;
                result = result.RotatedBy(-1.57f);
                cache.Add(Center + result);
            }
        }
        public void ManageTrail()
        {
            trail = new DanTrail(Main.instance.GraphicsDevice, cache.Count, new NoTip(),
            factor =>
            {
                float width = BaseUtility.MultiLerp(factor, 0.5f, 1.5f, 3);
                float wave = MathF.Sin(factor * MathHelper.Pi * 4 + Main.GameUpdateCount * .5f);
                float thickness = width * 100 + wave * 4;
                return thickness;
            },
            factor =>
            {
                Color color1 = new(255, 255, 255, 255);
                Color color2 = new(255, 255, 255, 255);
                Color c = BaseUtility.MultiLerpColor(factor.X * 2, color2, color1);
                return c;
            });
            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);

            Effect effect = Request<Effect>("Redemption/Effects/UrnWind", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //c Trail_3 Trail_2
            Texture2D tex0 = Request<Texture2D>("Redemption/Textures/Trails/Trail_2").Value;
            Texture2D tex1 = Request<Texture2D>("Redemption/Textures/Trails/vTrail_1").Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTexture0"].SetValue(tex0);
            effect.Parameters["uTexture1"].SetValue(tex1);
            effect.Parameters["uTime"].SetValue(Main.GameUpdateCount * -0.01f);
            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
        #endregion

    }
    */
    #endregion
}