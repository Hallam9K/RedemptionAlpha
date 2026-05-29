using Redemption.Buffs.Cooldowns;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class XeniumLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Left click to do a close ranged combo attack\n" +
                "Right-click to thrust forward, doing more damage at the tail-end of the thrust\n" +
                "Hitting with the thrust allows chaining the right-click, while missing gives a 20 second cooldown"); */
            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 74;
            Item.height = 74;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 15);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 450;
            Item.crit = 26;
            Item.knockBack = 8;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<XeniumLance_Proj>();
        }
        public override bool MeleePrefix() => true;
        private int Cooldown;
        public override void UpdateInventory(Player player)
        {
            if (Cooldown <= 0)
                return;
            Cooldown--;
        }
        public override bool AltFunctionUse(Player player) => !player.HasBuff<XeniumLanceCooldown>();
        private int Level;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale2 = player.GetAdjustedItemScale(Item);

            if (player.altFunctionUse == 2)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.ElectricSlash2, player.position);
                Projectile.NewProjectile(source, position, velocity, ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 3, 0, adjustedItemScale2);
                player.AddBuff(BuffType<XeniumLanceCooldown>(), 5 * 60);
            }
            else
            {
                if (Cooldown > 0)
                {
                    switch (Level)
                    {
                        case 0:
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = .8f }, player.position);
                            Projectile.NewProjectile(source, position, velocity, ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 1, 0, adjustedItemScale2);
                            Level++;
                            break;
                        case 1:
                            Projectile.NewProjectile(source, position, velocity, ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 2, 0, adjustedItemScale2);

                            Cooldown = 0;
                            Level = 0;
                            return false;
                    }
                }
                else
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = .8f }, player.position);
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<XeniumLance_Proj>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
                    Level = 0;
                }
            }
            Cooldown = 40;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<XeniumAlloy>(), 15)
                .AddIngredient(ItemType<Capacitor>())
                .AddIngredient(ItemType<CarbonMyofibre>(), 8)
                .AddTile(TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}
