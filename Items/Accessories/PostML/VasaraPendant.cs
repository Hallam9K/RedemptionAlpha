using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class VasaraPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("When hit for damage exceeding 150, an aura forms around the player that electrifies enemies and heals the player\n" +
                "15 second cooldown"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().vasaraPendant = true;
        }
    }
    public class VasaraPendant_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/StaticBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electricity Field");
        }
        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 200;
            Projectile.timeLeft = 300;
        }
        private Vector2 targetPos;
        private readonly List<int> targets = new();
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.01f;

            if (Projectile.timeLeft > 30 && Main.rand.NextBool(10))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(400, RedeHelper.RandomRotation()), 1, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(400, RedeHelper.RandomRotation()), 1, 20, 0.1f);
            }
            else if (Projectile.timeLeft <= 30)
                Projectile.alpha += 2;

            Projectile.localAI[0]++;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            if (Projectile.localAI[0] % 6 == 0)
            {
                targets.Clear();
                int target = -1;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                        continue;

                    if (npc.DistanceSQ(player.Center) > 500 * 500)
                        continue;

                    targets.Add(npc.whoAmI);
                    int[] targetsArr = targets.ToArray();
                    target = Utils.Next(Main.rand, targetsArr);
                }
                if (target != -1)
                {
                    targetPos = Main.npc[target].Center;
                    SoundEngine.PlaySound(CustomSounds.Zap2 with { Volume = 0.2f }, targetPos);

                    DustHelper.DrawParticleElectricity<LightningParticle>(player.Center, targetPos, 1f, 20, 0.2f);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        if (npc.DistanceSQ(targetPos) > 40 * 40)
                            continue;

                        int hitDirection = npc.RightOfDir(Projectile);
                        BaseAI.DamageNPC(npc, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
                    }
                }
            }
            if (player.dead || !player.active)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 4f, 4.3f, 4f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 4.3f, 4f, 4.3f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Cyan, Color.LightCyan);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), -Projectile.rotation, drawOrigin, Projectile.scale * scale2, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}