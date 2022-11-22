using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Minions;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Summon
{
    public class CosmosChains : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chains of the Cosmos");
            Tooltip.SetDefault("20 summon tag damage\n" +
                "10% summon tag critical strike chance\n" +
                "Your summons will focus struck enemies\n" +
                "Strike enemies to summon friendly cosmic eyes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 30;
            Item.DefaultToWhip(ModContent.ProjectileType<CosmosChains_Proj>(), 260, 6, 6, 28);
            Item.shootSpeed = 6;
            Item.rare = ModContent.RarityType<CosmicRarity>();
            Item.channel = true;
            Item.value = Item.buyPrice(1, 0, 0, 0);
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RainbowWhip)
                .AddIngredient<LifeFragment>(7)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class CosmosChains_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chains of the Cosmos");
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 32;
            Projectile.WhipSettings.RangeMultiplier = 2f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<ChainsCosmicEye>()] < 4)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + RedeHelper.PolarVector(Main.rand.Next(150, 351), Main.rand.NextFloat(0, MathHelper.TwoPi)), Vector2.Zero, ModContent.ProjectileType<ChainsCosmicEye>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, player.whoAmI, target.whoAmI);
            }
            target.AddBuff(BuffID.RainbowWhipNPCDebuff, 180);
            player.MinionAttackTargetNPC = target.whoAmI;
        }
        private int soundTimer;
        public override void PostAI()
        {
            if (soundTimer++ == 18)
                SoundEngine.PlaySound(SoundID.Item125, Projectile.position);
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