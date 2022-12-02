using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class VasaraPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("When hit for damage exceeding 150, an aura forms around the player that electrifies enemies and heals the player\n" +
                "15 second cooldown");
            SacrificeTotal = 1;
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
        public override string Texture => "Redemption/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electricity Field");
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 220;
            Projectile.timeLeft = 300;
        }
        private Vector2 targetPos;
        private readonly List<int> targets = new();
        public override void AI()
        {
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

                    if (npc.DistanceSQ(player.Center) > 600 * 600)
                        continue;

                    targets.Add(npc.whoAmI);
                    int[] targetsArr = targets.ToArray();
                    target = Utils.Next(Main.rand, targetsArr);
                }
                if (target != -1)
                {
                    targetPos = Main.npc[target].Center;
                    SoundEngine.PlaySound(CustomSounds.Zap2, targetPos);

                    DustHelper.DrawParticleElectricity(player.Center, targetPos, new LightningParticle(), 1f, 20, 0.2f);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        if (npc.DistanceSQ(targetPos) > 40 * 40)
                            continue;

                        int hitDirection = Projectile.Center.X > npc.Center.X ? -1 : 1;
                        BaseAI.DamageNPC(npc, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
                    }
                }
            }
            if (player.dead || !player.active)
                Projectile.Kill();
        }
    }
}