using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Effects.Trails;
using Redemption.Effects.Trails.Tips;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ritualist;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class ErhanMagnifyingGlass : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Magnifying Glass");
            /* Tooltip.SetDefault("Hold left-click to charge a scorching ray" +
                "\n'Super effective on insects'"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<Bindeklinge>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item1 with { Volume = 0 };
            Item.autoReuse = false;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 11;
            Item.mana = 10;
            Item.knockBack = 1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.shoot = ProjectileType<ErhanMagnifyingGlass_HoldOut>();
            Item.shootSpeed = 10f;
        }
    }
    public class ErhanMagnifyingGlass_HoldOut : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/ErhanMagnifyingGlass";
        public override void SetStaticDefaults()
        {
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = false;

            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.ownerHitCheck = true;

            InitializeTrail();
        }
        public float maxTime;
        public override void OnSpawn(IEntitySource source)
        {
            maxTime = (int)(Owner.HeldItem.useTime / Owner.GetAttackSpeed(DamageClass.Magic));
        }
        public Vector2 positionVector;
        public override void AI()
        {
            TrailSetUp();

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
                Projectile.Kill();

            Vector2 armCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction * -4, -4);
            RedeProjectile.HoldOutProj_SlowTurn(Projectile, Owner, armCenter, 0.4f);
            Projectile.Center = armCenter;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f);

            if (Timer == maxTime * 4)
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);

            if (Timer >= maxTime * 4 && Timer % maxTime == 0)
            {
                int mana = Owner.inventory[Owner.selectedItem].mana;
                if (BasePlayer.ReduceMana(Owner, mana / 2))
                {
                    if (Owner.channel)
                        Projectile.timeLeft = 200;
                }
                else
                {
                    Owner.channel = false;
                }
            }
            if (Timer >= maxTime * 4)
            {
                Projectile.friendly = true;
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 200, 255);
            }
            if (Projectile.timeLeft < 10 || !Owner.channel || !Owner.active || Owner.dead)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }
                Projectile.alpha += 20;
            }
            lightOpacity = 0.85f;
            Timer++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength = 375;
            float maxAngle = MathHelper.ToRadians(15);
            float rotation = Projectile.velocity.ToRotation();
            Vector2 offset = new Vector2(Owner.direction * 2, -16);
            Vector2 position = Projectile.Center + offset.RotatedBy(Owner.direction == 1 ? Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation() + MathHelper.Pi);
            if (targetHitbox.IntersectsConeSlowMoreAccurate(position, coneLength, rotation, maxAngle))
                return true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.OnFire, 300);
        }
        public float damageReduction;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float tipBonus;
            tipBonus = MathHelper.Lerp(1.4f, .5f, player.Distance(target.Center) / 400f);
            tipBonus = MathHelper.Clamp(tipBonus, 0.5f, 1);

            modifiers.SourceDamage *= tipBonus;
        }
        public float lightOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = rect.Size() / 2;

            Vector2 offset = new Vector2(Owner.direction * 2, -10);
            Vector2 position = Projectile.Center + offset.RotatedBy(Owner.direction == 1 ? Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation() + MathHelper.Pi);

            float rotation = Projectile.spriteDirection == 1 ? Projectile.rotation - 1 * MathHelper.PiOver4 : Projectile.rotation - 3 * MathHelper.PiOver4;
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, lightColor, rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            DrawFlare();
            DrawFlare2();
            DrawTrail();
            return false;
        }
        public void DrawFlare()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 drawOrigin = rect.Size() / 2;
            Color colour = Main.dayTime ? new(249, 240, 161) : Color.CornflowerBlue;

            Vector2 offset = new Vector2(Owner.direction * -32, -16);
            Vector2 position = Projectile.Center + offset.RotatedBy(Owner.direction == 1 ? Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation() + MathHelper.Pi);

            Vector2 scale = new Vector2(1, 2f) * 0.5f;
            float rotation = Timer / 60f;
            Main.EntitySpriteDraw(flare, position - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(colour), rotation + 1.57f, drawOrigin, scale, 0, 0);
            Main.EntitySpriteDraw(flare, position - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(colour), -rotation, drawOrigin, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
        public void DrawFlare2()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 drawOrigin = rect.Size() / 2;
            Color colour = Main.dayTime ? new(249, 240, 161) : Color.CornflowerBlue;

            Vector2 offset = new Vector2(Owner.direction * 2, -16);
            Vector2 position = Projectile.Center + offset.RotatedBy(Owner.direction == 1 ? Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation() + MathHelper.Pi);

            Vector2 scale = new Vector2(1, 2f) * 0.5f;
            Main.EntitySpriteDraw(flare, position - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(colour), 1.57f, drawOrigin, scale, 0, 0);
            Main.EntitySpriteDraw(flare, position - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(colour), 1.57f, drawOrigin, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
        #region draw trail
        private DanTrail trail;
        private List<Vector2> cache = new();
        private int NumPoints = 200;
        private float length = 400;
        public void TrailSetUp()
        {
            ManageCache();
            ManageTrail();
        }
        public void ManageCache()
        {
            Vector2 offset = new Vector2(Owner.direction * 2, -17);
            Vector2 position = Projectile.Center + offset.RotatedBy(Owner.direction == 1 ? Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation() + MathHelper.Pi) + Owner.velocity;
            cache = new List<Vector2>();

            for (float i = 0; i < NumPoints; i++)
            {
                float internalDiv = i / (NumPoints - 1);
                float modifiedProgress = internalDiv;
                Vector2 result = Projectile.velocity.SafeNormalize(default) * length * modifiedProgress;

                cache.Add(position + result);
            }
        }
        public void InitializeTrail()
        {
            trail = new DanTrail(RedeGraphics.Instance.Primitives, new NoTip(),
            factor =>
            {
                float widthFunc = BaseUtility.MultiLerp(EaseFunction.Linear.Ease(factor), 20, 300);
                return widthFunc;
            },
            factor =>
            {
                Color c = Main.dayTime ? new(249, 240, 161) : Color.CornflowerBlue;
                c.A = 0;
                float opacity = Projectile.Opacity * (1 - factor.X);
                return c * opacity * .7f;
            });
        }
        public void ManageTrail()
        {
            trail.SetPositions(cache.ToArray(), Projectile.Center + Projectile.velocity);
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Effect effect = Request<Effect>("Redemption/Effects/GlowTrail").Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //Trail_6
            Texture2D texture = Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(texture);
            effect.Parameters["uTime"].SetValue(1);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        #endregion
    }
}
