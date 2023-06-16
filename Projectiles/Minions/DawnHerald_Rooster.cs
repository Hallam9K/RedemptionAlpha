using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class DawnHerald_Rooster : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/FowlMorning/Haymaker_Nest";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rooster Booster");
            Main.projFrames[Projectile.type] = 3;
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

        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.velocity.Y += 1;

            if (Projectile.ai[0]++ == 120)
                SoundEngine.PlaySound(CustomSounds.RoosterRoar with { Volume = .2f, Pitch = -.2f }, Projectile.position);
            if (Projectile.ai[0] >= 120 && Projectile.ai[0] < 300)
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 2)
                        Projectile.frame = 1;
                }
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) > 460 * 460)
                        continue;

                    player.AddBuff(ModContent.BuffType<RoosterAuraBuff>(), 4);
                }
                if (Projectile.ai[0] % 20 == 0)
                    RedeDraw.SpawnCirclePulse(Projectile.Center, Color.IndianRed, 1.3f, Projectile);
            }
            else
                Projectile.frame = 0;
            if (Projectile.ai[0] >= 360)
                Projectile.ai[0] = 0;

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
                Projectile.Kill();
                return false;
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D chickenTex = ModContent.Request<Texture2D>("Redemption/Projectiles/Minions/DawnHerald_Rooster").Value;
            Texture2D nestBack = ModContent.Request<Texture2D>(Texture + "_Back").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = chickenTex.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, chickenTex.Width, height);
            Rectangle nestRect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(chickenTex.Width / 2, height / 2);
            Vector2 nestPos = Projectile.Center + new Vector2(0, 3.7f) - Main.screenPosition;
            Vector2 chickenPos = Projectile.Center - new Vector2(-1.3f, 10.7f) - Main.screenPosition;

            Main.EntitySpriteDraw(nestBack, nestPos, new Rectangle?(nestRect), Projectile.GetAlpha(lightColor), 0, new Vector2(nestBack.Width / 2, nestBack.Height / 2), 1, 0, 0);
            Main.EntitySpriteDraw(chickenTex, chickenPos, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, nestPos, new Rectangle?(nestRect), Projectile.GetAlpha(lightColor), 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, 0, 0);
            return false;
        }
    }
}