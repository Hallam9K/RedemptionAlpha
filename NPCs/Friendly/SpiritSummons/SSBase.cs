using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.Particles;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public abstract class SSBase : ModRedeNPC
    {
        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            SetSafeStaticDefaults();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public virtual void SetSafeDefaults() { }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.Redemption().spiritSummon = true;
            SetSafeDefaults();
        }
        public bool CannotBeSoulTugged;

        public static bool NoSpiritEffect(NPC npc)
        {
            if (npc.ai[3] < 0)
                return false;
            return Main.player[(int)npc.ai[3]].RedemptionPlayerBuff().cruxSpiritExtractor;
        }

        public override bool CheckActive() => false;
        public override bool PreAI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            SpiritBasicAI(NPC, player);
            return true;
        }
        public static void SpiritBasicAI(NPC npc, Player player)
        {
            if (!player.active || player.dead || !CheckActive(player))
                npc.SimpleStrikeNPC(9999, 1);

            if (npc.lavaWet)
            {
                npc.ai[0] = 10;
                npc.netUpdate = true;
            }

            if (Main.myPlayer == player.whoAmI && npc.DistanceSQ(player.Center) > 2000 * 2000)
            {
                npc.Center = player.Center;
                npc.velocity *= 0.1f;
                npc.netUpdate = true;
            }
            if (!Main.rand.NextBool(40) || NoSpiritEffect(npc))
                return;
            RedeParticleManager.CreateSpiritParticle(npc.RandAreaInEntity(), RedeHelper.Spread(2), 1, Main.rand.Next(90, 121));
        }
        public static bool CheckActive(Player owner)
        {
            if (!owner.HasBuff(BuffType<CruxCardBuff>()))
                return false;
            return true;
        }
        public static void SoulMoveState(NPC npc, ref float aiTimer, Player player, ref float timerRand, ref int runCooldown, float particleScale = .6f, float glowScale = 1f, int yOffset = 8, bool infernal = false, bool noTileCollide = false, bool flying = false)
        {
            Vector2 v = Vector2.Zero;
            SoulMoveState(npc, ref aiTimer, player, ref timerRand, ref runCooldown, ref v, particleScale, glowScale, yOffset, infernal, noTileCollide, flying);
        }
        public static void SoulMoveState(NPC npc, ref float aiTimer, Player player, ref float timerRand, ref int runCooldown, ref Vector2 moveTo, float particleScale = .6f, float glowScale = 1f, int yOffset = 8, bool infernal = false, bool noTileCollide = false, bool flying = false)
        {
            npc.alpha = 255;
            npc.noGravity = true;
            npc.noTileCollide = true;
            aiTimer = 0;

            RedeParticleManager.CreateSpiritParticle(npc.Center + RedeHelper.Spread(10) + npc.velocity, Vector2.Zero, particleScale, RedeParticleManager.spiritColors, Main.rand.Next(20, 30));

            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(npc.Center + npc.velocity - Vector2.One, 1, 1, DustType<GlowDust>(), 0, 0, 0, default, glowScale);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= .1f;
                Color dustColor = new(188, 244, 227) { A = 0 };
                if (infernal)
                    dustColor = new(255, 162, 17) { A = 0 };
                Main.dust[dust].color = dustColor;
            }

            bool check = npc.Hitbox.Intersects(player.Hitbox);
            if (!noTileCollide)
                check &= Collision.CanHit(npc.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(npc.position, npc.width, npc.height);

            if (check)
            {
                int dustType = infernal ? DustID.InfernoFork : DustID.DungeonSpirit;
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, dustType, 0, 0, Scale: 2);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].noGravity = true;
                }

                npc.alpha = 0;
                npc.noGravity = flying;
                npc.noTileCollide = noTileCollide;
                npc.velocity *= 0f;

                moveTo = npc.FindGround(20);
                runCooldown = 0;
                timerRand = Main.rand.Next(120, 260);
                npc.ai[0] = 0;
                npc.netUpdate = true;
            }
            else
                npc.Move(player.Center - new Vector2(0, yOffset), 20, 20);
        }
        public static int GetNearestNPC(NPC npc, int ID = 0, bool targetFriendly = false)
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == npc.whoAmI || target.dontTakeDamage || !target.chaseable || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                bool friendlyCheck = target.friendly || target.lifeMax <= 5 || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type];
                switch (ID)
                {
                    case 1:
                        friendlyCheck |= NPCLists.Plantlike.Contains(target.type);
                        break;
                    case 2:
                        friendlyCheck |= target.type == NPCType<KeeperSpirit>() || target.type == NPCType<Keeper>();
                        break;
                    case 3:
                        friendlyCheck = target.friendly && target.life < target.lifeMax;
                        break;
                }
                if (targetFriendly)
                {
                    if (!friendlyCheck)
                        continue;
                }
                else
                {
                    if (friendlyCheck)
                        continue;
                }

                if (nearestNPCDist != -1 && !(target.Distance(npc.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(npc.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
    }
}