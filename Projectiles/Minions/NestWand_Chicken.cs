using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class NestWand_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/FowlMorning/Haymaker_Nest";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chicken Nest");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[1] = Main.rand.Next(4);
            Projectile.netUpdate = true;
        }
        public override bool? CanDamage() => false;
        NPC target;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
                return;

            if (RedeHelper.ClosestNPC(ref target, 900, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
                Projectile.LookAtEntity(target);
                if (Projectile.localAI[0]++ <= 30 && Projectile.frame == 1)
                    Projectile.frameCounter = 0;

                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (Projectile.frame is 2 && Projectile.owner == Main.myPlayer)
                    {
                        int height = 4;
                        if (Projectile.DistanceSQ(target.Center) < 200 * 200)
                            height = 1;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 16), Projectile.DirectionTo(target.Center) * Main.rand.Next(8, 13) - new Vector2(0, height), ModContent.ProjectileType<ChickenEgg_Proj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI, 1);
                        Main.projectile[p].DamageType = DamageClass.Summon;
                        Main.projectile[p].netUpdate = true;
                    }
                    if (++Projectile.frame > 4)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                        Projectile.localAI[0] = 0;
                    }
                }
            }
            else
            {
                Projectile.localAI[0] = 0;
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            Projectile.velocity.Y += 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D chickenTex = ModContent.Request<Texture2D>("Redemption/Projectiles/Minions/NestWand_Chicken").Value;
            Texture2D nestBack = ModContent.Request<Texture2D>(Texture + "_Back").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = chickenTex.Height / 5;
            int width = chickenTex.Width / 4;
            int y = height * Projectile.frame;
            int x = (int)(width * Projectile.localAI[1]);
            Rectangle rect = new(x, y, width, height);
            Rectangle nestRect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(width / 2, height / 2);
            Vector2 nestPos = Projectile.Center + new Vector2(0, 3.7f) - Main.screenPosition;
            Vector2 chickenPos = Projectile.Center - new Vector2(-1.3f, 10.7f) - Main.screenPosition;

            Main.EntitySpriteDraw(nestBack, nestPos, new Rectangle?(nestRect), Projectile.GetAlpha(lightColor), 0, new Vector2(nestBack.Width / 2, nestBack.Height / 2), 1, 0, 0);
            Main.EntitySpriteDraw(chickenTex, chickenPos, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, nestPos, new Rectangle?(nestRect), Projectile.GetAlpha(lightColor), 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, 0, 0);
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<NestWandBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<NestWandBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
    }
}