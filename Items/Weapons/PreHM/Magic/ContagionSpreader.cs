using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class ContagionSpreader : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts a contagious shard that sticks into enemies" +
                "\nRight-click to break all shards stuck to enemies, causing an outward burst of projectiles"); */
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 46;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item42;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ContagionShard_Proj>();
            Item.shootSpeed = 17f;
        }
        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[Item.shoot] > 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 Offset = Vector2.Normalize(velocity) * 60f;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.type != Item.shoot || proj.owner != player.whoAmI || proj.ai[0] == 0)
                        continue;

                    int steps = (int)player.Distance(proj.Center) / 8;
                    for (int n = 0; n < steps; n++)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Dust dust = Dust.NewDustDirect(Vector2.Lerp(player.Center + Offset, proj.Center, (float)n / steps), 2, 2, DustID.Firework_Green);
                            dust.velocity = -proj.DirectionTo(dust.position) * 2;
                            dust.noGravity = true;
                        }
                    }

                    proj.localAI[1] = 1;
                    proj.netUpdate = true;
                }
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}