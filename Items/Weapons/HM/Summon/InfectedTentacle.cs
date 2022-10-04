using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Minions;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class InfectedTentacle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Your summons will focus struck enemies\n" +
                "Strike enemies to summon a friendly hive cyst\n" +
                "Inflicts Infection");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 40;
            Item.DefaultToWhip(ModContent.ProjectileType<InfectedTentacle_Proj>(), 46, 2, 8, 26);
            Item.shootSpeed = 8;
            Item.rare = ItemRarityID.LightRed;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 0, 95, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 2)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override bool MeleePrefix() => true;
    }
    public class InfectedTentacle_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Tentacle");
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 0.8f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            if (Main.rand.NextBool(5))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<HiveCyst_Proj>()] < 2)
            {
                SoundEngine.PlaySound(SoundID.NPCHit13, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<HiveCyst_Proj>(), Projectile.damage / 3, Projectile.knockBack, player.whoAmI);
            }
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
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