using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Magic;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class BladeOfTheMountain : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.IceS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blade of the Mountain");
            /* Tooltip.SetDefault("Parries physical or ice projectiles, including your own Icefall crystals" +
                "\nDeals more damage at the tip of the blade" +
                "\nHitting on the very tip of the blade can freeze enemies" +
                "\nEnemies with knockback immunity cannot be frozen\n" +
                "'Send them to their snowy grave'"); */

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 76;
            Item.height = 80;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 78;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<BladeOfTheMountain_Slash>();

            Item.Redemption().TechnicallySlash = true;
            Item.Redemption().CanSwordClash = true;
        }
        static bool IcefallNeighbour(Player player, out int icefallIndex)
        {
            icefallIndex = -1;
            int thisIndex = -1;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ItemType<Icefall>())
                    icefallIndex = i;
                if (player.inventory[i] == player.HeldItem)
                    thisIndex = i;
            }
            if (icefallIndex >= 0 && icefallIndex <= 9 && (icefallIndex == thisIndex - 1 || icefallIndex == thisIndex + 1))
                return true;
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return IcefallNeighbour(player, out _);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && IcefallNeighbour(player, out int icefallIndex))
            {
                Item icefall = player.inventory[icefallIndex];
                SoundEngine.PlaySound(icefall.UseSound);

                player.itemAnimationMax = icefall.useAnimation;
                player.itemTime = icefall.useAnimation;
                player.itemAnimation = icefall.useAnimation;

                int p = Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<Icefall_HeldProj>(), player.GetWeaponDamage(icefall), player.GetWeaponKnockback(icefall), player.whoAmI, icefallIndex);
                Main.projectile[p].CritChance = player.GetWeaponCrit(icefall);
                Main.projectile[p].ArmorPenetration = player.GetWeaponArmorPenetration(icefall);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && IcefallNeighbour(player, out int icefallIndex))
            {
                if (!ItemLoader.CanUseItem(player.inventory[icefallIndex], player))
                    return false;
                return BasePlayer.ReduceMana(player, player.inventory[icefallIndex].mana);
            }
            return base.CanUseItem(player);
        }
        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.IceTorch, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override bool MeleePrefix() => true;
        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy, PrefixID.Weak, PrefixID.Lazy, PrefixID.Small, PrefixID.Slow, PrefixID.Tiny, PrefixID.Sluggish, PrefixID.Unhappy };
        public override bool AllowPrefix(int pre)
        {
            if (Array.IndexOf(unwantedPrefixes, pre) > -1)
                return false;
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.BladeOfTheMountain.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
    public class Icefall_HeldProj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/Icefall";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        public ref float IcefallIndex => ref Projectile.ai[0];
        public ref float UseLimit => ref Projectile.ai[1];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter + new Vector2(18 * player.direction, -17), true) - Projectile.Size / 2f;
            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.bodyFrame.Y = 2 * player.bodyFrame.Height;

            if (IcefallIndex < 0 || IcefallIndex > 9)
            {
                Projectile.Kill();
                return;
            }
            Item icefall = player.inventory[(int)IcefallIndex];

            if (Projectile.localAI[0]++ % icefall.useTime == 0 && UseLimit < 4 && Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(player.GetSource_ItemUse(icefall), Main.MouseWorld, new Vector2(Main.rand.NextFloat(-2, 2), 0), icefall.shoot, Projectile.damage, Projectile.knockBack, player.whoAmI);

                UseLimit++;
            }
            if (Projectile.localAI[0] >= icefall.useAnimation + icefall.reuseDelay)
                Projectile.Kill();
        }
    }
}