using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Buffs;
using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Rarities;
using Terraria.GameContent.Creative;
using Redemption.Textures;
using Redemption.Globals;

namespace Redemption.Items.Usable
{
    public class StatuetteOfFaith : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statuette of Faith");
            Tooltip.SetDefault("Temporarily creates a large Celestine Dreamsong aura around the location of use"
                + "\nPlayers in the aura will see better in the Soulless Caverns\n" +
                "'Have a little faith'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.UseSound = new("Redemption/Sounds/Custom/WorldTree");
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.noUseGraphic = false;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 5;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.shoot = ModContent.ProjectileType<StatuetteOfFaith_Proj>();
            Item.shootSpeed = 0;
        }
    }
    public class StatuetteOfFaith_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Usable/StatuetteOfFaith";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statuette of Faith");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 44;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
        }

        NPC target;
        Player playerTarget;
        public override void AI()
        {
            if (Projectile.localAI[0] == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DreamsongLight_Visual2>(), 0, 0, Main.myPlayer, Projectile.whoAmI);
                Projectile.localAI[0] = 1;
            }
            Projectile.velocity *= 0;
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                target = Main.npc[n];
                if (!target.active || target.immortal || target.dontTakeDamage || target.friendly || target.DistanceSQ(Projectile.Center) > 300 * 300)
                    continue;

                if (!NPCLists.Soulless.Contains(target.type))
                    continue;

                target.AddBuff(ModContent.BuffType<DreamsongBuff>(), 10, false);
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                playerTarget = Main.player[p];
                if (playerTarget.active && !playerTarget.dead && playerTarget.DistanceSQ(Projectile.Center) <= 300 * 300)
                {
                    playerTarget.buffImmune[ModContent.BuffType<BlackenedHeartDebuff>()] = true;
                    playerTarget.AddBuff(ModContent.BuffType<DreamsongBuff>(), 10);
                }
            }
        }
    }
    public class DreamsongLight_Visual2 : DreamsongLight_Visual
    {
        public override string Texture => "Redemption/Textures/DreamsongLight_Visual";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestine Light");
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool PreAI()
        {
            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            if (!host.active)
                Projectile.Kill();

            Projectile.Center = host.Center;
            Projectile.timeLeft = 10;
            Projectile.velocity *= 0;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.localAI[0] < 30)
                    Projectile.alpha -= 5;
                else
                    Projectile.alpha += 5;
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.scale = 1;
                Projectile.localAI[0] = 0;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            return false;
        }
    }
}
