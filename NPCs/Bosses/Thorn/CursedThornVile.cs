using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class CursedThornVile : ModProjectile
    {
        private static Asset<Texture2D> mainTex;
        private static Asset<Texture2D> endTex;
        public override void Load()
        {
            mainTex = TextureAssets.Projectile[Projectile.type];
            endTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Thorn/CursedThornVile_End");
        }

        public override void Unload()
        {
            mainTex = null;
            endTex = null;
        }

        public static Color lightColor = new Color(0, 40, 0);
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
            BaseAI.AIVilethorn(Projectile, 190, 4, 18);
            spineEnd = Projectile.ai[1] == 18;
            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle rect = spineEnd ? endTex.Value.Bounds : mainTex.Value.Bounds;
            Vector2 drawOrigin = spineEnd ? endTex.Size() * 0.5f : mainTex.Size() * 0.5f;

            Main.EntitySpriteDraw(spineEnd ? endTex.Value : mainTex.Value, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}