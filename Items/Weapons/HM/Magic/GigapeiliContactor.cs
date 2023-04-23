using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Projectiles.Magic;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class GigapeiliContactor : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gigapeili Contactor");
            /* Tooltip.SetDefault("Fires a short-ranged spread of electrical bolts\n" +
                "Right-click to deploy a stationary drone, or to call it back\n" +
                "Bolts hitting the drone will reflect them with a longer range and tighter spread at the nearest enemy"); */
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.height = 56;
            Item.width = 56;
            Item.useTime = 31;
            Item.useAnimation = 31;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.UseSound = CustomSounds.Laser1 with { Pitch = .3f };
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<GigapeiliBolt>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            }
            else
                Item.UseSound = CustomSounds.Laser1 with { Pitch = .3f };
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 45f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                position += Offset;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<GigapeiliDrone>();
                if (player.ownedProjectileCounts[type] == 0)
                    Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI);
                else
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.active || proj.type != type || proj.owner != player.whoAmI || proj.ai[0] == 1)
                            continue;
                        proj.ai[0] = 1;
                        proj.netUpdate = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                    float scale = 1f - (Main.rand.NextFloat() * 0.4f);
                    perturbedSpeed *= scale;
                    Projectile.NewProjectile(source, position, perturbedSpeed, type, (int)(damage * .75f), knockback, player.whoAmI);
                }
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 6)
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddIngredient(ModContent.ItemType<AIChip>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}