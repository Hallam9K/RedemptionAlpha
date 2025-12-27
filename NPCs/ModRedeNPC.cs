using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs
{
    public abstract class ModRedeNPC : ModNPC
    {
        public Vector2 moveTo;
        public float SpeedMultiplier = 1f;

        #region Attacker Methods
        public int PlayerTarget = -2;
        public Entity Attacker()
        {
            if (!NPC.HasValidTarget)
                return Main.player[Main.maxPlayers - 1];
            if (NPC.HasNPCTarget)
                return Main.npc[NPC.TranslatedTargetIndex];
            else
                return Main.player[NPC.target];
        }
        public void TargetPlayerByDefault(bool faceTarget = true)
        {
            if (NPC.HasValidTarget)
                return;
            if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(faceTarget);
        }
        public void SetPlayerTarget()
        {
            int closestPlayer = NPC.FindClosestPlayer();
            if (PlayerTarget < 0 || PlayerTarget >= 255 || Main.player[PlayerTarget].dead || !Main.player[PlayerTarget].active)
            {
                PlayerTarget = closestPlayer;
                NPC.netUpdate = true;
            }
        }
        public Player GetPlayerTarget()
        {
            if (PlayerTarget >= 0)
                return Main.player[PlayerTarget];
            return Main.LocalPlayer;
        }
        public Player GetAnyPlayerTarget()
        {
            if (!NPC.HasValidTarget || NPC.HasNPCTarget)
            {
                if (PlayerTarget >= 0)
                    return Main.player[PlayerTarget];
                else
                {
                    SetPlayerTarget();
                    return GetPlayerTarget();
                }
            }
            return Main.player[NPC.target];
        }
        public void TargetPlayer(bool faceTarget = true)
        {
            if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(faceTarget);
        }
        public Player PlayerAsTarget()
        {
            if (NPC.HasPlayerTarget)
                return Main.player[NPC.target];
            return Main.LocalPlayer;
        }
        public NPC NPCAsTarget()
        {
            if (NPC.HasPlayerTarget)
                return Main.npc[NPC.TranslatedTargetIndex];
            return null;
        }
        public static bool CheckAttackerIsSpirit(Entity attacker) => attacker != null && attacker.active && attacker is NPC npc && npc.Redemption().spiritSummon;
        public bool CheckAttackerIsFriendly(Entity attacker) => attacker != null && (NPC.HasPlayerTarget || CheckAttackerIsSpirit(attacker));

        public virtual bool SpecialNPCTargets(NPC target) => false;
        public virtual bool BlacklistNPCTargets(NPC target) => false;
        #endregion

        #region Dialect Methods
        public virtual bool HasTalkButton() => false;
        public virtual bool HasReviveButton() => false;
        public virtual bool HasCruxButton(Player player) => false;
        public virtual string CruxButtonText(Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        public virtual void CruxButton(Player player) { }

        public const short EPIDOTRA = 1, KINGDOM = 2, CAVERN = 3, LIDEN = 4, OMEGA = 5, SLAYER = 6, NEBULEUS = 7, BLACK = 8, DEMON = 9;
        public struct HangingButtonParams(int count = 1, bool glow = false, float positionY = 0)
        {
            public float PositionY = positionY;
            public bool Glow = glow;
            public int Count = count;
        }

        public short DialogueBoxStyle;
        public virtual HangingButtonParams LeftHangingButton(Player player) => new(1);
        public virtual HangingButtonParams RightHangingButton(Player player) => new(1);
        public virtual bool HasLeftHangingButton(Player player) => false;
        public virtual bool HasRightHangingButton(Player player) => false;
        #endregion

        public void BasicGroundedFrames(int frameHeight, int idleFrameMax, int walkFrameMin, int walkFrameMax, int jumpFrame, int frameCounter = 10, int walkFrameCounter = 3)
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (++NPC.frameCounter >= frameCounter)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > idleFrameMax * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frame.Y < walkFrameMin * frameHeight)
                        NPC.frame.Y = walkFrameMin * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= walkFrameCounter || NPC.frameCounter <= -walkFrameCounter)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > walkFrameMax * frameHeight)
                            NPC.frame.Y = walkFrameMin * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = jumpFrame * frameHeight;
            }
        }
        public void DamageInHitbox(Rectangle rect, int damage, float knockback = 4.5f, bool parried = false, int CD = 30, bool hitAnyPlayer = true)
        {
            NPC.GetGlobalNPC<HitboxNPC>().CreateExtendedHitbox(rect);
            if (!parried)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || target.whoAmI == NPC.whoAmI)
                        continue;

                    if (!(NPC.HasNPCTarget && NPC.TranslatedTargetIndex == target.whoAmI))
                    {
                        if (!SpecialNPCTargets(target) && !target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])
                            continue;
                    }

                    if (BlacklistNPCTargets(target))
                        continue;

                    if (target.immune[NPC.whoAmI] > 0 || !target.Hitbox.Intersects(rect))
                        continue;

                    target.immune[NPC.whoAmI] = CD;
                    int hitDirection = target.RightOfDir(NPC);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        BaseAI.DamageNPC(target, damage, knockback, hitDirection, NPC);
                }
                if (!hitAnyPlayer)
                {
                    if (!NPC.HasPlayerTarget)
                        return;
                }
                HitboxNPC.HitPlayerInHitbox(NPC, rect, damage, knockback);
            }
        }

        public int Shoot(Vector2 position, int projType, int damage, Vector2 velocity, SoundStyle sound, float ai0 = 0, float ai1 = 0, float knockback = 4.5f)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(sound, NPC.position);
            return NPCHelper.Shoot(NPC, position, projType, damage, velocity, ai0, ai1, knockback, NPC.whoAmI);
        }
        public int Shoot(Vector2 position, int projType, int damage, Vector2 velocity, float ai0 = 0, float ai1 = 0, float knockback = 4.5f)
        {
            return NPCHelper.Shoot(NPC, position, projType, damage, velocity, ai0, ai1, knockback, NPC.whoAmI);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
    }
}