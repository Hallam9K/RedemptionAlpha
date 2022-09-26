using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class Akka_CursedThorn : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Thorn/CursedThornVile";
        private static Asset<Texture2D> endTex;
        public override void Load()
        {
            endTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Thorn/CursedThornVile_End");
        }

        public override void Unload()
        {
            endTex = null;
        }

        public static Color lightColor = new(0, 40, 0);
        public bool spineEnd = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Thorns");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 320;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            BaseAI.AIVilethorn(Projectile, 190, 4, 36);
            spineEnd = Projectile.ai[1] == 36;
            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = spineEnd ? endTex.Value.Bounds : mainTex.Bounds;
            Vector2 drawOrigin = spineEnd ? endTex.Size() * 0.5f : mainTex.Size() * 0.5f;

            Main.EntitySpriteDraw(spineEnd ? endTex.Value : mainTex, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
        }
    }
}