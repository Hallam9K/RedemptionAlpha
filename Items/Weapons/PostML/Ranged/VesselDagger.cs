using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Melee;
using Redemption.Rarities;
using Redemption.WorldGeneration;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class VesselDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vessel Dagger");
            Tooltip.SetDefault("Daggers burst into candle light after a small distance");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 23f;
            Item.damage = 210;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 7;
            Item.useTime = 7;
            Item.width = 18;
            Item.height = 46;
            Item.maxStack = 999;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 0, 3, 0);
            Item.shoot = ModContent.ProjectileType<VesselDagger_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient<VesselFragment>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class VesselDagger_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Ranged/VesselDagger";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vessel Dagger");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
            if (++Projectile.ai[0] >= 20)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath39 with { Volume = .5f }, Projectile.position);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.Spread(8), ModContent.ProjectileType<CandleLight_Proj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                        Main.projectile[p].DamageType = DamageClass.Ranged;
                    }
                }
                Projectile.Kill();
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 10;
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            return projHitbox.Intersects(targetHitbox);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 usePos = Projectile.position;
            Vector2 rotVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2();
            usePos += rotVector * 16f;
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.AncientLight);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += rotVector * 2f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                usePos -= rotVector * 8f;
            }
        }
    }
}
