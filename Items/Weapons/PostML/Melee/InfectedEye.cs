using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Projectiles;
using Redemption.Rarities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class InfectedEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<SwarmerCannon>();
        }
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 42;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = RarityType<TurquoiseRarity>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 800;
            Item.knockBack = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.shootSpeed = 32f;
            Item.shoot = ProjectileType<InfectedEye_Ball>();
        }
    }
    public class InfectedEye_Ball : Flail
    {
        public override void SetSafeStaticDefaults()
        {
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;

            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.Redemption().TechnicallyMelee = true;
            Projectile.penetrate = -1;
        }
        public override void SetStats(ref int throwTime, ref float throwSpeed, ref float recoverDistance, ref float recoverDistance2, ref int attackCooldown)
        {
            throwTime = 12;
            throwSpeed = 32f;
            recoverDistance = 24f;
            recoverDistance2 = 24f;
            attackCooldown = 15;
        }
        private bool release;
        private int soundTimer;
        public override void PostAI()
        {
            if (Projectile.ai[0] == 0 && soundTimer++ % 30 == 0)
            {
                if (Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            }
            if (Projectile.ai[0] is 2 or 4 or 5 && !release)
            {
                ReleaseEye(Projectile.velocity);
            }
            if (release)
            {
                Projectile.ai[0] = 4;
                Projectile.friendly = false;
            }
            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) + 1.57f;
        }
        public override void OnTileCollide_Extra(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] != 0 && !release)
            {
                ReleaseEye(oldVelocity);
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = Request<Texture2D>("Redemption/Items/Weapons/PostML/Melee/InfectedEye_Chain").Value;
            Vector2 HeadPos = player.MountedCenter;
            Rectangle sourceRectangle = new(0, 0, chainTexture.Width, chainTexture.Height);
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            float num1 = chainTexture.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            var effects = player.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Main.EntitySpriteDraw(chainTexture, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), Projectile.GetAlpha(lightColor), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2 + 20);

            if (!release)
                Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(release);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            release = reader.ReadBoolean();
        }
        public void ReleaseEye(Vector2 oldVel)
        {
            release = true;
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            Vector2 vel = oldVel.SafeNormalize(default) * 16f;
            Projectile.NewProjectile(null, Projectile.Center, vel, ProjectileType<InfectedEye_Ball2>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .2f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SludgeDust>(), Scale: 2);
                Main.dust[dustIndex].velocity = vel * 0.1f;
            }
        }
    }
    public class InfectedEye_Ball2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;

            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
            Projectile.velocity.Y += 0.2f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffType<BileDebuff>(), 180);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .2f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SludgeDust>(), Scale: 2);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity *= 0.98f;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            RedeDraw.SpawnXenoSplat(Projectile.Center, 1, false);
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SludgeDust>(), Scale: 1.5f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
        }
    }
}
