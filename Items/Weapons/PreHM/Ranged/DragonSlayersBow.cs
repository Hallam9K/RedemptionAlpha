using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class DragonSlayersBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 26;
            Item.height = 58;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 42;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 20f;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ProjectileID.BoneArrow;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 10;
        }


        float flag = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            flag++;
            if (flag >= 10)
            {
                flag = 0;
                for (int i = 0; i < 10; i++)
                    count[i] = 0;
            }

            float numberProjectiles = 4;
            float rotation = MathHelper.ToRadians(10);
            for (int i = 0; i < numberProjectiles; i++)
            {
                float spread = Main.rand.NextFloat(-MathHelper.ToRadians(2), MathHelper.ToRadians(2));
                float speedSpread = Main.rand.NextFloat(0.97f, 1.03f);
                velocity *= speedSpread;
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation - spread, rotation + spread, i / (numberProjectiles - 1)));
                Projectile proj = Projectile.NewProjectileDirect(source, position + RedeHelper.PolarVector(2, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), perturbedSpeed, type, damage, knockback, player.whoAmI, flag);
                proj.GetGlobalProjectile<DragonSlayersBowGlobal>().ShotFrom = true;
                proj.GetGlobalProjectile<DragonSlayersBowGlobal>().Flag = flag;
            }
            if (breath)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DragonBreathStart>(), damage, knockback, player.whoAmI);
                breath = false;
                ready = false;
            }
            return false;
        }
        public bool breath = false;
        public bool ready = false;
        public int[] count = new int[10];
        public override void HoldItem(Player player)
        {
            for (int k = 0; k < 10; k++)
            {
                count[k] = Main.projectile.Count(n => n.type == ModContent.ProjectileType<DragonSlayersBowHitcheck>() && n.owner == player.whoAmI && (int)n.ai[0] == k);
                if (count[k] >= 4)
                    breath = true;
            }

            if (breath && !ready)
            {
                ready = true;
                SoundEngine.PlaySound(SoundID.Item88, player.Center);
                RedeDraw.SpawnRing(player.Center, new Color(255, 120, 65), 0.12f, 0.86f, 4);
            }
        }
    }
    public class DragonSlayersBowGlobal : GlobalProjectile //base code from slr, modified
    {
        public override bool InstancePerEntity => true;
        public bool ShotFrom = false;
        public float Flag;
        public bool isHit;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            isHit = true;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (ShotFrom && isHit)
            {
                Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonSlayersBowHitcheck>(), projectile.damage, projectile.knockBack, projectile.owner, Flag);
            }
        }
    }
    public class DragonSlayersBowHitcheck : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.timeLeft = 120;
        }
        public int[] count = new int[10];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.dead || !player.active)
                Projectile.Kill();

            for (int k = 0; k < 10; k++)
            {
                IEnumerable<Projectile> list = Main.projectile.Where(n => n.type == ModContent.ProjectileType<DragonSlayersBowHitcheck>() && n.owner == player.whoAmI && (int)n.ai[0] == k);
                count[k] = Main.projectile.Count(n => n.type == ModContent.ProjectileType<DragonSlayersBowHitcheck>() && n.owner == player.whoAmI && (int)n.ai[0] == k);
                if (count[k] >= 4)
                    foreach (Projectile proj in list)
                        proj.Kill();
            }
        }
    }
}
