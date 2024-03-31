using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Projectiles.Misc;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Items.Armor.PostML.Xenium
{
    public class XeniumGrenadeCannon : HeldOnlyItem
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override bool VisibleInUI => false;
        public override void SetDefaults()
        {
            Item.damage = 800;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 32;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.holdStyle = -1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = CustomSounds.GrenadeLauncher with { Pitch = -.2f };
            Item.autoReuse = true;
            Item.shootSpeed = 20;
            Item.shoot = ProjectileType<XeniumGrenade>();
        }

        public override bool CanUseItem(Player player)
        {
            return XeniumVisor.Activate(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center + new Vector2(0, -8);
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override void HoldItem(Player player)
        {
            Projectile proj = Main.projectile[player.whoAmI];
            if (player.ownedProjectileCounts[ProjectileType<XeniumTrajectory>()] < 1)
            {
                Projectile.NewProjectile(proj.GetSource_FromAI(), player.Center, Vector2.Zero, ProjectileType<XeniumTrajectory>(), proj.damage, proj.knockBack, player.whoAmI);
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.armor[0].ModItem is not XeniumVisor || !player.RedemptionPlayerBuff().xeniumBonus)
                {
                    Item.type = ItemID.None;
                    Item.SetDefaults();
                    Item.stack = 0;
                    Main.mouseItem = new Item();
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips[0].OverrideColor = new Color(100, 255, 255);
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
        }
    }
    public abstract class HeldOnlyItem : ModItem //code from calamity mod
    {
        public virtual bool VisibleInUI => false;

        public override void Load()
        {
            On_Player.dropItemCheck += new On_Player.hook_dropItemCheck(DontDropCoolStuff);
            On_ItemSlot.LeftClick_ItemArray_int_int += new On_ItemSlot.hook_LeftClick_ItemArray_int_int(LockMouseToSpecialItem);
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += new On_ItemSlot.hook_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(DrawSpecial);
        }
        public override void Unload()
        {
            On_Player.dropItemCheck -= new On_Player.hook_dropItemCheck(DontDropCoolStuff);
            On_ItemSlot.LeftClick_ItemArray_int_int -= new On_ItemSlot.hook_LeftClick_ItemArray_int_int(LockMouseToSpecialItem);
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= new On_ItemSlot.hook_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(DrawSpecial);
        }

        private void DrawSpecial(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch sb, Item[] inv, int context, int slot, Vector2 position, Color color)
        {
            if (inv[slot].ModItem is not HeldOnlyItem || (inv[slot].ModItem as HeldOnlyItem).VisibleInUI)
            {
                orig.Invoke(sb, inv, context, slot, position, color);
            }
        }

        public override void PostUpdate()
        {
            Item.type = ItemID.None;
            Item.stack = 0;
        }

        public override bool CanPickup(Player player)
        {
            return false;
        }

        private void LockMouseToSpecialItem(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (Main.mouseItem.ModItem is not HeldOnlyItem)
            {
                orig.Invoke(inv, context, slot);
            }
        }

        private void DontDropCoolStuff(On_Player.orig_dropItemCheck orig, Player self)
        {
            if (Main.mouseItem.ModItem is not HeldOnlyItem)
            {
                orig.Invoke(self);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }
    }
    public class XeniumGrenade : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/ToxicGrenade";

        public override void SetStaticDefaults()
        {
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 29;
            Projectile.extraUpdates = 3;
        }

        private readonly int NUMPOINTS = 16;
        public Color baseColor = new(72, 243, 20);
        public Color endColor = new(48, 63, 73);
        public Color edgeColor = new(48, 63, 73);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 2f;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield", AssetRequestMode.ImmediateLoad).Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);
            Color colour = Color.LightGreen;
            float scale = MathHelper.Clamp(Projectile.ai[0] / 30, 0, 1);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin2, scale, 0, 0);

            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        private int fakeTimer;
        private Vector2 oldVelocity;
        private void FakeKill()
        {
            oldVelocity = Projectile.velocity;
            Projectile.alpha = 255;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer++ == 0)
                BlastSpawn();

            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 8;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width / 2, Projectile.height / 2);
            return false;
        }
        private void BlastSpawn()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<GasCanister_Gas>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, player.whoAmI);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<Xenium_GasExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI);

            RedeDraw.SpawnExplosion(Projectile.Center, Color.White * .5f, shakeAmount: 0, scale: 2f, noDust: true);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.LightGreen, shakeAmount: 0, scale: 1.5f, noDust: true);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.White, shakeAmount: 0, scale: 1f, noDust: true);
            RedeDraw.SpawnRing(Projectile.Center, Color.LimeGreen, 0.23f, 0.9f, 3);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            player.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            player.RedemptionScreen().ScreenShakeIntensity += 6;

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ash, oldVelocity.X * 0.3f, oldVelocity.Y * 0.3f, Scale: 2);
                Main.dust[dust].velocity *= 5;
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, oldVelocity.X * 0.3f, oldVelocity.Y * 0.3f, Scale: 2);
                Main.dust[dust].velocity *= 7;
                Main.dust[dust].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int g = 0; g < 3; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
            Rectangle boom = new((int)Projectile.Center.X, (int)Projectile.Center.Y, 160, 160);
            RedeHelper.NPCRadiusDamage(boom, Projectile, Projectile.damage, Projectile.knockBack);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            oldVelocity = Projectile.velocity;
            FakeKill();
        }

        public Vector2 staticVel;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }

            if (Projectile.ai[0]++ == 0)
            {
                staticVel = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
                Projectile.velocity = staticVel;
            }

            if (fakeTimer > 0)
                FakeKill();

            if (Projectile.timeLeft <= 2)
                FakeKill();
        }
    }
    public class Xenium_GasExplosion : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 2;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
    }

    public class XeniumTrajectory : LaserProjectile
    {
        public override string Texture => "Redemption/Textures/DottedBeam";
        public override void SetDefaults()
        {
            Projectile.height = 44;
            Projectile.width = 19;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            LaserScale = 0.5f;
            LaserSegmentLength = 44;
            LaserWidth = 19;
            LaserEndSegmentLength = 0;
            MaxLaserLength = 70;
            NewCollision = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ItemType<XeniumGrenadeCannon>())
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 5;
            }

            float dir = (Main.MouseWorld - player.Center).ToRotation();
            int dir2 = (dir <= MathHelper.PiOver2 && dir >= -MathHelper.PiOver2) ? 1 : -1;

            player.ChangeDir(dir2);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.MountedCenter + new Vector2(0, -8);
            Projectile.velocity = RedeHelper.PolarVector(1, dir);

            #region length

            // code from slr
            for (int k = 0; k < MaxLaserLength; k++)
            {
                Vector2 posCheck = player.Center + new Vector2(0, -8) + Vector2.UnitX.RotatedBy(dir) * k * 8;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (!npc.active || npc.dontTakeDamage || npc.friendly || NPCLoader.CanBeHitByProjectile(npc, Projectile) is false)
                        continue;

                    Rectangle posCheckRect = new((int)posCheck.X - 10, (int)posCheck.Y - 10, 20, 20);
                    if (posCheckRect.Intersects(npc.Hitbox))
                    {
                        Vector2 dist = RedeHelper.PolarVector(Projectile.Center.Distance(npc.Center), Projectile.velocity.ToRotation());
                        LaserLength = LengthSetting(Projectile, Projectile.Center - dist);
                        return;
                    }
                }
                if (Helper.PointInTile(posCheck) || k == MaxLaserLength - 1)
                {
                    endPoint = posCheck;
                    break;
                }
            }
            LaserLength = LengthSetting(Projectile, endPoint);
            #endregion
        }

        #region Drawcode
        // The core function of drawing a Laser, you shouldn't need to touch this
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/RadialTelegraph3", AssetRequestMode.ImmediateLoad).Value;
            Texture2D flare2 = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Rectangle rect2 = new(0, 0, flare2.Width, flare2.Height);

            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 origin2 = new(flare2.Width / 2, flare2.Height / 2);

            Vector2 position = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength - 2) - Main.screenPosition;
            Vector2 position2 = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength - 5) - Main.screenPosition;
            Color colour = Color.LightGreen;

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.velocity.ToRotation()) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.velocity.ToRotation()) * LaserScale, -1.57f, LaserScale, LaserLength - LaserSegmentLength * 2, Projectile.GetAlpha(colour), (int)FirstSegmentDrawDist);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin, 0.5f, 0, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin, 0.5f, 0, 0);
            Main.EntitySpriteDraw(flare2, position2, new Rectangle?(rect2), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin2, 1.3f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
    }
    public class Xenium_Gas : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virulent Gas");
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.scale = Main.rand.NextFloat(2, 2.5f);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.Next(1, 3);

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.003f;

            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 5;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (!Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    target.AddBuff(BuffType<XeniumGasDebuff>(), 30 * 60);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}