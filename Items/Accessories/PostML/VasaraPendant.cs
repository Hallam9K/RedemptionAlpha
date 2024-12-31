using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Magic;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class VasaraPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Generic;
            Item.width = 26;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 19);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override bool WeaponPrefix() => false;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<VasaraPendant_Player>().vasaraPendant = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip2"));
            if (tooltipLocation != -1)
            {
                TooltipLine line = new(Mod, "IreLine", Language.GetTextValue("Mods.Redemption.Items.VasaraPendant.IreLine") + player.GetModPlayer<VasaraPendant_Player>().ireCharge + "/200") { OverrideColor = Color.LightGoldenrodYellow };
                tooltips.Insert(tooltipLocation, line);
            }
        }
    }
    public class VasaraPendant_Player : ModPlayer
    {
        public bool vasaraPendant;
        public int ireCharge = 0;
        public override void ResetEffects()
        {
            vasaraPendant = false;
        }
        public override void UpdateDead()
        {
            vasaraPendant = false;
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (vasaraPendant && !Player.HasBuff<VasaraPendantCooldown>())
            {
                ireCharge += info.Damage;
                if (ireCharge >= 200)
                {
                    Main.NewLightning();
                    Player.AddBuff(ModContent.BuffType<VasaraPendantCooldown>(), 900);
                    Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<VasaraPendant>())), Player.Center, Vector2.Zero, ModContent.ProjectileType<VasaraPendant_Proj>(), (int)(200 * Player.GetDamage<GenericDamageClass>().Multiplicative), 0, Main.myPlayer);
                    ireCharge = 0;
                }
            }
        }
    }
    public class VasaraPendant_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/StaticBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electricity Field");
        }
        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.alpha = 200;
            Projectile.timeLeft = 300;
        }
        private Vector2 targetPos;
        private readonly List<int> targets = new();
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.01f;

            if (Projectile.timeLeft > 30 && Main.rand.NextBool(10))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(400, RedeHelper.RandomRotation()), 1, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(400, RedeHelper.RandomRotation()), 1, 20, 0.1f);
            }
            else if (Projectile.timeLeft <= 30)
                Projectile.alpha += 2;

            Projectile.localAI[0]++;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter;
            if (Main.myPlayer == Projectile.owner && Projectile.localAI[0] % 10 == 0)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VasaraRock_Proj>(), Projectile.damage, 8, player.whoAmI);
            }
            if (Projectile.localAI[0] % 6 == 0)
            {
                targets.Clear();
                int target = -1;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                        continue;

                    if (npc.DistanceSQ(player.Center) > 500 * 500)
                        continue;

                    targets.Add(npc.whoAmI);
                    int[] targetsArr = targets.ToArray();
                    target = Utils.Next(Main.rand, targetsArr);
                }
                if (target != -1)
                {
                    targetPos = Main.npc[target].Center;
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Zap2 with { Volume = 0.2f }, targetPos);

                    DustHelper.DrawParticleElectricity<LightningParticle>(player.Center, targetPos, 1f, 20, 0.2f);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        if (npc.DistanceSQ(targetPos) > 40 * 40)
                            continue;

                        int hitDirection = npc.RightOfDir(Projectile);
                        BaseAI.DamageNPC(npc, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
                    }
                }
            }
            if (player.dead || !player.active)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 4f, 4.3f, 4f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 4.3f, 4f, 4.3f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Cyan, Color.LightCyan);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), -Projectile.rotation, drawOrigin, Projectile.scale * scale2, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class VasaraRock_Proj : Rockslide_Proj
    {
        public override string Texture => "Redemption/Projectiles/Magic/Rockslide_Proj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rockslide");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Generic;
            Rand = Main.rand.Next(80, 200);
            double angle = Main.rand.NextDouble() * 2d * Math.PI;
            MoveVector2.X = (float)(Math.Sin(angle) * Rand);
            MoveVector2.Y = (float)(Math.Cos(angle) * Rand);
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += (Projectile.ai[0] == 0 ? 0.01f : 0.2f) * Projectile.spriteDirection;
            if (Projectile.alpha > 0)
                Projectile.alpha -= 5;
            if (Projectile.ai[1]++ == 0)
                SoundEngine.PlaySound(SoundID.Item69, Projectile.position);
            if (Projectile.ai[1] < 60)
                pos *= 0.98f;
            else
            {
                if (Projectile.localAI[1] == 0)
                {
                    pos.Y += 0.03f;
                    if (pos.Y > .7f)
                        Projectile.localAI[1] = 1;
                }
                else if (Projectile.localAI[1] == 1)
                {
                    pos.Y -= 0.03f;
                    if (pos.Y < -.7f)
                        Projectile.localAI[1] = 0;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.position = player.Center + MoveVector2;
                    MoveVector2 += pos;
                    NPC target = null;
                    if (RedeHelper.ClosestNPC(ref target, 3000, Projectile.Center) && Main.rand.NextBool(5) && Projectile.alpha <= 0)
                    {
                        Projectile.timeLeft = 200;
                        Projectile.tileCollide = true;
                        SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
                        Projectile.velocity = Projectile.DirectionTo(target.Center + (target.velocity / 10)) * 25;
                        Projectile.ai[0] = 1;
                    }
                }
            }
            if (Projectile.timeLeft <= 30)
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, Projectile.timeLeft / 30);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(7), ModContent.ProjectileType<RockslidePebble_Proj>(), Projectile.damage / 2, 1, Main.myPlayer);
                    Main.projectile[p].DamageType = DamageClass.Generic;
                }
            }
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                    -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f, Scale: 2);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            if (Projectile.ai[0] == 1)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.LightGoldenrodYellow) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.LightGoldenrodYellow, Projectile.rotation, drawOrigin, Projectile.scale, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}