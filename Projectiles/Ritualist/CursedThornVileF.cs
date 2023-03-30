using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.DamageClasses;
using Redemption.Globals;

namespace Redemption.Projectiles.Ritualist
{
    public class CursedThornVileF : ModProjectile
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
            // DisplayName.SetDefault("Cursed Thorns");
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 320;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<RitualistClass>();
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.Redemption().RitDagger = true;
        }
        public override void AI()
        {
            BaseAI.AIVilethorn(Projectile, 190, 10, 18);
            spineEnd = Projectile.ai[1] == 18;
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
    }
}