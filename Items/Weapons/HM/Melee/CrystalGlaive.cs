using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class CrystalGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Left-click to do a close ranged combo attack when hitting enemies\n" +
                "Right-click to fire a mid range blast of crystal shards\n" +
                "Completing the left-click combo on an enemy empowers the right-click ability for 5 shots"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 52;
            Item.height = 52;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 90;
            Item.knockBack = 7f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<CrystalGlaive_Proj>();
        }
        private int Cooldown;
        public override void UpdateInventory(Player player)
        {
            if (Cooldown <= 0)
                return;
            Cooldown--;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool sp = player.Redemption().crystalGlaiveShotCount > 0;
            if (player.altFunctionUse == 2)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = .1f }, player.position);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 3, sp ? 1 : 0);
                player.Redemption().crystalGlaiveShotCount--;
            }
            else
            {
                if (Cooldown > 0)
                {
                    switch (player.Redemption().crystalGlaiveLevel)
                    {
                        case 0:
                            player.Redemption().crystalGlaiveLevel = 0;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = .1f }, player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 0, sp ? 1 : 0);
                            break;
                        case 1:
                            player.Redemption().crystalGlaiveLevel = 0;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = .1f }, player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 1, sp ? 1 : 0);
                            break;
                        case 2:
                            player.Redemption().crystalGlaiveLevel = 0;
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = .1f }, player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 2, sp ? 1 : 0);
                            return false;
                    }
                }
                else
                {
                    player.Redemption().crystalGlaiveLevel = 0;
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Swing1 with { Pitch = .1f }, player.position);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 0, sp ? 1 : 0);
                }

                Cooldown = 40;
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<CrystalGlaive_Proj>()] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteGlaive)
                .AddIngredient(ItemID.SoulofLight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumTrident)
                .AddIngredient(ItemID.SoulofLight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}