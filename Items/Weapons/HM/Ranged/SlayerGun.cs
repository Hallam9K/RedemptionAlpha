using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.KSIII;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Globals.Player;
using Redemption.Globals;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class SlayerGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hyper-Tech Blaster");
            /* Tooltip.SetDefault("\n(2-6[i:" + ModContent.ItemType<EnergyPack>() + "]) Replaces normal bullets with Energy Bolts"
                + "\nRight-clicking changes type of fire\n" +
                "Requires an Energy Pack to be in your inventory"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 78;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 26;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = CustomSounds.Gun1KS;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Item.ExtraItemShoot(ModContent.ProjectileType<KS3_EnergyBolt>());
        }

        public int AttackMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = CustomSounds.ShootChange;
            }
            else
            {
                switch (AttackMode)
                {
                    case 0:
                        Item.UseSound = CustomSounds.Gun1KS;
                        break;
                    case 1:
                        Item.UseSound = CustomSounds.Gun3KS;
                        break;
                    case 2:
                        Item.UseSound = CustomSounds.Gun2KS;
                        break;
                }
            }
            return player.GetModPlayer<EnergyPlayer>().statEnergy >= 2;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                AttackMode++;
                if (AttackMode >= 3)
                    AttackMode = 0;

                switch (AttackMode)
                {
                    case 0:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode1"), true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode2"), true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode3"), true, false);
                        break;
                }
            }
            else
            {
                switch (AttackMode)
                {
                    case 0:
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 2;
                        int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<KS3_EnergyBolt>(), damage, knockback, player.whoAmI);
                        Main.projectile[proj].hostile = false;
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].DamageType = DamageClass.Ranged;
                        Main.projectile[proj].tileCollide = true;
                        Main.projectile[proj].netUpdate = true;
                        break;
                    case 1:
                        player.itemAnimationMax = Item.useTime * 3;
                        player.itemTime = Item.useTime * 3;
                        player.itemAnimation = Item.useTime * 3;
                        damage = (int)(damage * 1.5f);

                        float numberProjectiles = 3;
                        float rotation = MathHelper.ToRadians(15);
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                            int proj3 = Projectile.NewProjectile(source, position, perturbedSpeed, ProjectileID.MartianTurretBolt, (int)(damage / 1.5f), knockback, player.whoAmI);
                            Main.projectile[proj3].Redemption().EnergyBased = true;
                            Main.projectile[proj3].hostile = false;
                            Main.projectile[proj3].friendly = true;
                            Main.projectile[proj3].DamageType = DamageClass.Ranged;
                            Main.projectile[proj3].tileCollide = true;
                            Main.projectile[proj3].netUpdate = true;
                        }
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 6;
                        int proj2 = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<KS3_EnergyBolt>(), damage, knockback, player.whoAmI);
                        Main.projectile[proj2].hostile = false;
                        Main.projectile[proj2].friendly = true;
                        Main.projectile[proj2].DamageType = DamageClass.Ranged;
                        Main.projectile[proj2].tileCollide = true;
                        Main.projectile[proj2].netUpdate = true;
                        break;
                    case 2:
                        damage = (int)(damage * 1.4f);
                        player.itemAnimationMax = Item.useTime * 2;
                        player.itemTime = Item.useTime * 2;
                        player.itemAnimation = Item.useTime * 2;
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 2;
                        int proj4 = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ReboundShot>(), damage, knockback, player.whoAmI);
                        Main.projectile[proj4].hostile = false;
                        Main.projectile[proj4].friendly = true;
                        Main.projectile[proj4].DamageType = DamageClass.Ranged;
                        Main.projectile[proj4].netUpdate = true;
                        break;

                }
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CyberPlating>(), 6)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 40f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shotType = "";
            switch (AttackMode)
            {
                case 0:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode1");
                    break;
                case 1:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode2");
                    break;
                case 2:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SlayerGun.Mode3");
                    break;
            }
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.LightCyan,
            };
            tooltips.Add(line);
        }
    }
}
