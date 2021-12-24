using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.Audio;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class CrystalGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Left-click to do a close ranged combo attack\n" +
                "Right-click to fire a mid range blast of crystal shards\n" +
                "Completing the Left-click combo empowers the right-click ability for 5 shots");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 52;
            Item.height = 52;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);

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
        private int Level;
        private int ShotCount;
        public override void UpdateInventory(Player player)
        {
            if (Cooldown <= 0)
                return;
            Cooldown--;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool sp = ShotCount > 0;
            if (player.altFunctionUse == 2)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Swoosh1").WithPitchVariance(0.1f), player.position);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 3, sp ? 1 : 0);
                ShotCount--;
            }
            else
            {
                if (Cooldown > 0)
                {
                    switch (Level)
                    {
                        case 0:
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Swing1").WithPitchVariance(0.1f).WithVolume(0.6f), player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 1, sp ? 1 : 0);
                            Level++;
                            break;
                        case 1:
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Swoosh1").WithPitchVariance(0.1f), player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 2, sp ? 1 : 0);
                            if (ShotCount <= 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, player.position);
                                DustHelper.DrawCircle(player.Center, DustID.CrystalPulse, 4, 1, 1, 1, 2, nogravity: true);
                            }
                            Cooldown = 0;
                            Level = 0;
                            ShotCount = 5;
                            return false;
                    }
                }
                else
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Swing1").WithPitchVariance(0.1f).WithVolume(0.6f), player.position);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalGlaive_Proj>(), damage, knockback, player.whoAmI, 0, sp ? 1 : 0);
                    Level = 0;
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