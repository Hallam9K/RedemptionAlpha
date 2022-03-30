using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class DaggerOfOathkeeper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dagger of the Oathkeeper");
            Tooltip.SetDefault("Inflicts soulless");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 666;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = false;
            Item.noMelee = false;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DaggerStab_Proj>();
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.RedemptionPlayerBuff().shadowBinder && player.RedemptionPlayerBuff().shadowBinderCharge >= 2 && !player.HasBuff(ModContent.BuffType<OathkeeperDaggerBuff>()))
            {
                Item.noUseGraphic = true;
                Item.noMelee = true;
            }
            else
            {
                Item.noUseGraphic = false;
                Item.noMelee = false;
            }
            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.RedemptionPlayerBuff().shadowBinder && player.RedemptionPlayerBuff().shadowBinderCharge >= 2 && !player.HasBuff(ModContent.BuffType<OathkeeperDaggerBuff>()))
            {
                player.itemAnimationMax = Item.useTime * 10;
                player.itemTime = Item.useTime * 10;
                player.itemAnimation = Item.useTime * 10;
                player.RedemptionPlayerBuff().shadowBinderCharge -= 2;
                return true;
            }
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            string text;
            if (player.RedemptionPlayerBuff().shadowBinder)
                text = "Right-clicking will give you a damage buff at the cost of decreased max life for 2 minutes (Consumes 2 Shadowbound Souls)";
            else
                text = "Has a special ability if Sielukaivo Shadowbinder is equipped";
            TooltipLine line = new(Mod, "text", text) { overrideColor = Color.DarkGray };
            tooltips.Insert(tooltipLocation, line);
        }
    }
    public class DaggerStab_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/DaggerOfOathkeeper";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dagger of the Oathkeeper");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }
        public float newX = 60;
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 40 && newX != 0 && Projectile.localAI[0] < 80)
                newX += -4f;
            if (newX == 0 || Projectile.localAI[0] >= 60)
            {
                newX = 0;
                Projectile.Kill();
            }
            Projectile.rotation = MathHelper.ToRadians(225) * projOwner.direction;
            Projectile.direction = projOwner.direction;
            Projectile.spriteDirection = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Projectile.position.X = projOwner.Center.X - (Projectile.width / 2) + (newX * projOwner.direction);
            Projectile.position.Y = projOwner.Center.Y - (Projectile.height / 2);
        }
        public override void Kill(int timeLeft)
        {
            Player projOwner = Main.player[Projectile.owner];
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity.X *= -6f * projOwner.direction;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            projOwner.AddBuff(ModContent.BuffType<OathkeeperDaggerBuff>(), 7200);
        }
    }
}