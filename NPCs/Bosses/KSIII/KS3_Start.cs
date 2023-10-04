using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Start : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 106;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);

            switch (NPC.ai[0])
            {
                case 0:
                    if (RedeWorld.alignment >= 0)
                    {
                        if (RedeBossDowned.slayerDeath <= 1)
                            NPC.ai[0] = 1;
                        else if (RedeBossDowned.slayerDeath > 1)
                            NPC.ai[0] = 4;

                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.ai[0] = 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    if (NPC.ai[2]++ == 10 || NPC.ai[2] == 30)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)player.Center.X + Main.rand.Next(70, 180), (int)player.Center.Y - Main.rand.Next(800, 850), ModContent.NPCType<KS3_ScannerDrone>(), ai3: NPC.whoAmI);
                    }
                    if (NPC.ai[2] > 30 && !NPC.AnyNPCs(ModContent.NPCType<KS3_ScannerDrone>()))
                    {
                        if (NPC.ai[1] >= 2)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_DroneKillCheck>(), 0, Vector2.Zero);
                            NPC.ai[0] = 4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            if (RedeBossDowned.slayerDeath < 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.slayerDeath = 1;
                                NPC.ai[0] = 3;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            else
                                NPC.ai[0] = 2;

                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 2:
                    if (NPC.ai[2]++ == 30)
                        NPC.Shoot(new Vector2(player.Center.X + 90, player.Center.Y - 50), ModContent.ProjectileType<KS3_HeadHologram>(), 0, Vector2.Zero);

                    if (NPC.ai[2] > 760)
                    {
                        if (RedeBossDowned.slayerDeath < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 2;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        NPC.ai[0] = 3;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 3:
                    NPC.active = false;
                    NPC.velocity.Y += 10;
                    break;
                case 4:
                    if (RedeBossDowned.slayerDeath < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeBossDowned.slayerDeath = 2;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }

                    NPC.Center = new Vector2(player.position.X + 200, player.position.Y - 80f);
                    if (NPC.ai[2] % 3 == 0)
                    {
                        int dustIndex2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y - 800), NPC.width, NPC.height + 750, DustID.Frost, 0f, 0f, 100, default, 1f);
                        Main.dust[dustIndex2].velocity *= 1f;
                        Main.dust[dustIndex2].noGravity = true;
                    }
                    if (NPC.ai[2]++ >= 120)
                    {
                        if (NPC.AnyNPCs(ModContent.NPCType<KS3>()) || NPC.AnyNPCs(ModContent.NPCType<KS3_Clone>()))
                            NPC.active = false;
                        else
                        {
                            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                            DustHelper.DrawDustImage(NPC.Center, 92, 0.2f, "Redemption/Effects/DustImages/WarpShape", 3, true, 0);
                            for (int i = 0; i < 30; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 3f);
                                Main.dust[dustIndex].velocity *= 6f;
                                Main.dust[dustIndex].noGravity = true;
                            }
                            NPC.netUpdate = true;
                            if (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus)
                                NPC.SetDefaults(ModContent.NPCType<KS3_Clone>());
                            else
                                NPC.SetDefaults(ModContent.NPCType<KS3>());
                        }
                    }
                    break;
                case 5:
                    if (NPC.ai[2]++ == 10 || NPC.ai[2] == 30)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)player.Center.X + Main.rand.Next(70, 180), (int)player.Center.Y - Main.rand.Next(800, 850), ModContent.NPCType<KS3_ScannerDrone>(), ai3: NPC.whoAmI);
                    }
                    if (NPC.ai[2] > 30 && !NPC.AnyNPCs(ModContent.NPCType<KS3_ScannerDrone>()))
                    {
                        NPC.ai[0] = 4;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override bool CheckActive()
        {
            return NPC.ai[0] == 3;
        }
    }
    public class KS3_DroneKillCheck : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
    }
}