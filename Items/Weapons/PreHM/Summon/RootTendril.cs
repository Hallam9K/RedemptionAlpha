using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class RootTendril : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("4 summon tag damage\n" +
                "Your summons will focus struck enemies\n" +
                "Striking enemies with the tip of the whip will heal the user");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.DefaultToWhip(ModContent.ProjectileType<RootTendril_Proj>(), 16, 1, 6);
            Item.shootSpeed = 6;
            Item.rare = ItemRarityID.Green;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 3, 45, 0);
        }
        public override bool MeleePrefix() => true;
    }
    public class RootTendril_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Root Tendril");
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 10;
            Projectile.WhipSettings.RangeMultiplier = 0.5f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (target.DistanceSQ(player.Center) > 130 * 130)
            {
                int steps = (int)player.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(player.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                player.statLife += 3;
                player.HealEffect(3);
            }
            target.AddBuff(BuffID.BlandWhipEnemyDebuff, 180);
            player.MinionAttackTargetNPC = target.whoAmI;
        }
        private static void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            Main.DrawWhip_WhipBland(Projectile, list);
            return false;
        }
    }
}