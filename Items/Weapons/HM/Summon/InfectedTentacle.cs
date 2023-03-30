using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Minions;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class InfectedTentacle : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Your summons will focus struck enemies\n" +
                "Strike enemies to summon a friendly hive cyst\n" +
                "Inflicts Infection"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 40;
            Item.DefaultToWhip(ModContent.ProjectileType<InfectedTentacle_Proj>(), 46, 2, 8, 26);
            Item.shootSpeed = 8;
            Item.rare = ItemRarityID.LightRed;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 0, 95, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override bool MeleePrefix() => true;
    }
    public class InfectedTentacle_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infected Tentacle");
            ProjectileID.Sets.IsAWhip[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 0.8f;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            if (Main.rand.NextBool(5))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<HiveCyst_Proj>()] < 2)
            {
                SoundEngine.PlaySound(SoundID.NPCHit13 with { Volume = .5f }, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<HiveCyst_Proj>(), Projectile.damage / 3, Projectile.knockBack, player.whoAmI);
            }
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            //DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 14, 26);
                Vector2 origin = new(7, 8);
                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 120;
                    frame.Height = 18;
                }
                else if (i > 10)
                {
                    frame.Y = 92;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 64;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 36;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
}