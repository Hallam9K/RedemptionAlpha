using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Synthesizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Shoots a wave of notes along the ground");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Pink;
            Item.damage = 48;
            Item.shoot = ModContent.ProjectileType<Synthesizer_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<SynthNote_Proj>());
            Item.shootSpeed = 0;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.autoReuse = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int floor = BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16);
            position = new Vector2(player.Center.X, floor * 16 - 10);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float cursorPosFromPlayer = player.Distance(Main.MouseWorld) / (Main.screenHeight / 2 / 24);
            if (!Main.dedServ)
            {
                if (cursorPosFromPlayer > 24) cursorPosFromPlayer = 1;
                else cursorPosFromPlayer = (cursorPosFromPlayer / 12) - 1;
                if (!Main.dedServ)
                {
                    SoundStyle s = CustomSounds.Synth with { Pitch = cursorPosFromPlayer };
                    SoundEngine.PlaySound(s, player.Center);
                }
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, cursorPosFromPlayer);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Xenomite>(), 16)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 4)
                .AddIngredient(ItemID.SoulofSight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class Synthesizer_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Synthesizer");
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 40;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ % 3 == 0 && Main.myPlayer == Projectile.owner)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 origin = Projectile.Center;
                    origin.X += Projectile.localAI[0] * (14 + (Projectile.ai[0] * 4)) * i;
                    int numtries = 0;
                    int x = (int)(origin.X / 16);
                    int y = (int)(origin.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        origin.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        origin.Y = y * 16;
                    }
                    if (numtries >= 20)
                        break;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), origin + new Vector2(0, 18), new Vector2(0, -14), ModContent.ProjectileType<SynthNote_Proj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }
        }
    }
}