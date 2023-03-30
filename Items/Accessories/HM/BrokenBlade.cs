using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class BrokenBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Broken Blade");
            /* Tooltip.SetDefault("Hitting enemies with physical melee has a chance to summon a Phantom Cleaver above their heads" +
                "\n10% increased physical melee damage"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 48;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.expert = true;
            Item.accessory = true;
		}
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.RedemptionPlayerBuff().brokenBlade = true;
            player.RedemptionPlayerBuff().TrueMeleeDamage += 0.1f;
        }
	}
    public class PhantomCleaver_F2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/PhantomCleaver";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.rotation = MathHelper.Pi;
            switch (Projectile.localAI[1])
            {
                case 0:
                    if (!npc.active)
                        Projectile.velocity *= 0;
                    else
                        Projectile.Center = new Vector2(npc.Center.X, npc.position.Y - 200);
                    Projectile.alpha -= 5;
                    if (Projectile.alpha <= 80)
                    {
                        Projectile.friendly = true;
                        Projectile.localAI[1] = 1;
                    }
                    break;
                case 1:
                    Projectile.friendly = true;
                    Projectile.velocity.X = 0;
                    Projectile.velocity.Y += 10;
                    Projectile.alpha += 10;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(30, 0);
                Color color = Projectile.GetAlpha(RedeColor.RedPulse) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
