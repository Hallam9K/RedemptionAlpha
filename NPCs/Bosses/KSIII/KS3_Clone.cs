using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3_Clone : KS3
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III... ?");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.UsesNewTargetting[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] = 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 65; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override LocalizedText DeathMessage => Language.GetText("Announcement.HasBeenDefeated_Single").WithFormatArgs(Language.GetText("Mods.Redemption.NPCs.KS3_Clone.DefeatName"));
        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSlayer, -1);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.6f;
        }

        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Slayer.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.1f };
        public override bool PreAI()
        {
            switch (AIState)
            {
                case ActionState.Dialogue:
                    Lighting.AddLight(NPC.Center, .3f, .6f, .8f);

                    TargetPlayerByDefault();
                    SetPlayerTarget();
                    Player player = GetPlayerTarget();
                    Entity attacker = Attacker();

                    if (NPC.DespawnHandler())
                        return false;

                    chance = MathHelper.Clamp(chance, 0, 1);

                    player.RedemptionScreen().ScreenFocusPosition = NPC.Center;

                    Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
                    int dmgIncrease = NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 10 : 0;

                    if (!player.active || player.dead)
                        return false;

                    #region No Dialogue Moment :(
                    NPC.LookAtEntity(player);
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    AITimer++;
                    if (AITimer == 60)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.3"), new Color(170, 255, 255), Color.Black, voice, .03f, 1.6f, .16f, true, null, Bubble, null, modifier, 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer == 5001)
                    {
                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;
                    }
                    if (AITimer >= 5060)
                    {
                        phase = 4;
                        ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                        Shoot(NPC.Center, ProjectileType<KS3_Shield>(), 0, Vector2.Zero);
                        AITimer = 0;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    #endregion
                    if (NPC.DistanceSQ(attacker.Center) >= 1100 * 1100 && NPC.ai[0] > 0 && !player.RedemptionScreen().lockScreen)
                    {
                        if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                            return false;
                        Teleport(false, Vector2.Zero);
                        NPC.netUpdate = true;
                    }
                    return false;
            }
            return true;
        }
        public override bool CheckDead()
        {
            return true;
        }
    }
}