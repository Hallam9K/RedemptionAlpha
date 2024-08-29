using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class CursedThornVile : ModProjectile
    {
        private static Asset<Texture2D> endTex;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            endTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Thorn/CursedThornVile_End");
        }

        public static Color lightColor = new(0, 40, 0);
        public bool spineEnd = false;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cursed Thorns");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ElementID.ProjNature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 320;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            BaseAI.AIVilethorn(Projectile, 190, 4, 18);
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