using Microsoft.Xna.Framework;
using Redemption.Projectiles.Melee;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Buffs.Cooldowns;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class XeniumLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Left click to do a close ranged combo attack\n" +
                "Right click to thrust forward, doing more damage at the tail end of the thrust");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 50;
            Item.height = 50;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 15);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 750;
            Item.knockBack = 8;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<XeniumLance_Proj>();
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

            if (player.altFunctionUse == 2 && !player.HasBuff<XeniumLanceCooldown>())
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ElectricSlash2").WithPitchVariance(0.1f).WithVolume(1), player.position);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 3, sp ? 1 : 0);
                player.AddBuff(ModContent.BuffType<XeniumLanceCooldown>(), 10 * 60);
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
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ElectricSlash").WithPitchVariance(0.1f).WithVolume(0.8f), player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 1, sp ? 1 : 0);
                            Level++;
                            break;
                        case 1:
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ElectricSlash2").WithPitchVariance(0.1f).WithVolume(0.8f), player.position);
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 2, sp ? 1 : 0);

                            Cooldown = 0;
                            Level = 0;
                            ShotCount = 5;
                            return false;
                    }
                }
                else
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ElectricSlash").WithPitchVariance(0.1f).WithVolume(0.8f), player.position);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 0, sp ? 1 : 0);
                    Level = 0;
                }
            }
            
           

            Cooldown = 40;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 15)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}