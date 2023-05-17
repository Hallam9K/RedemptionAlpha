using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Summon
{
    public class SlumberEel : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slumber Eel");
            /* Tooltip.SetDefault("20 summon tag damage\n" +
                "10% summon tag critical strike chance\n" +
                "Your summons will focus struck enemies\n" +
                "Strike enemies to do something idk"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.DefaultToWhip(ModContent.ProjectileType<SlumberEel_Proj>(), 200, 6, 7, 30);
            Item.shootSpeed = 7;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.channel = true;
            Item.value = Item.sellPrice(0, 1, 26, 0);
        }
        public override bool MeleePrefix() => true;
    }
    public class SlumberEel_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slumber Eel");
            ProjectileID.Sets.IsAWhip[Type] = true;
            ElementID.ProjShadow[Type] = true;
            ElementID.ProjWater[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 32;
            Projectile.WhipSettings.RangeMultiplier = 1.4f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffID.RainbowWhipNPCDebuff, 180);
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

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 40, 18);
                Vector2 origin = new(20, 8);
                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 90;
                    frame.Height = 38;
                }
                else if (i > 10)
                {
                    frame.Y = 72;
                    frame.Height = 14;
                }
                else if (i > 5)
                {
                    frame.Y = 50;
                    frame.Height = 12;
                }
                else if (i > 0)
                {
                    frame.Y = 28;
                    frame.Height = 12;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
}