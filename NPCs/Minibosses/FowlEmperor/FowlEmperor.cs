using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable.Summons;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    [AutoloadBossHead]
    public class FowlEmperor : ModNPC
    {
        public enum ActionState
        {
            Start,
            Idle,
            FeatherThrow,
            FlutterAtk,
            Admire
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 20;

            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1250;
            NPC.damage = 18;
            NPC.defense = 2;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.aiStyle = -1;
            NPC.width = 46;
            NPC.height = 52;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossFowl");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AniType is (int)AnimType.Throw && NPC.frame.Y >= 17 * 80;
        public override bool CanHitNPC(NPC target) => target.friendly && AniType is (int)AnimType.Throw && NPC.frame.Y >= 17 * 80;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.FowlEmperor1")),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.FowlEmperor2"))
            });
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedFowlEmperor)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.FowlDefeat"), 120, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedFowlEmperor, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<FowlEmperorRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<EggPet>(), 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FowlEmperorTrophy>(), 10));
            npcLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FowlCrown>(), 7));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FowlWarHorn>()));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>(), 1, 5, 5));
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (item.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), 5);
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), 5);
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (projectile.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), 5);
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), 5);
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item) => AIState > ActionState.Start ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => AIState > ActionState.Start ? null : false;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.7f);
        }

        public int AniType;
        public enum AnimType { None, Flap, Laugh, Throw, Admire, Stun, Mad, Recrown }
        Vector2 landingPos;
        private int repeat;
        private int AttackType;
        public int crownLife = 40;
        public Vector2 moveTo;
        private bool eggCracked;
        private bool empowered;
        private Projectile crownProj;
        public override void AI()
        {
            CustomFrames(76);

            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (DespawnHandler())
                return;

            NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Start:
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.dedServ)
                                RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.FowlEmperor.Name"), 60, 90, 0.8f, 0, Color.PeachPuff, Language.GetTextValue("Mods.Redemption.TitleCard.FowlEmperor.Modifier"));

                            NPC.noTileCollide = true;
                            NPC.noGravity = true;
                            Vector2 pos = new(player.Center.X, BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16) * 16);
                            bool landed = false;
                            int attempts = 0;
                            while (!landed && attempts < 1000)
                            {
                                attempts++;
                                landingPos = NPCHelper.FindGroundVector(NPC, pos, 15);
                                if (landingPos.X > pos.X && NPC.Center.X < pos.X)
                                    continue;
                                if (landingPos.X < pos.X && NPC.Center.X > pos.X)
                                    continue;
                                if (landingPos.DistanceSQ(pos) < 40 * 40)
                                    continue;

                                landed = true;
                            }
                            AniType = (int)AnimType.Flap;
                            TimerRand++;
                            break;
                        case 1:
                            if (NPC.DistanceSQ(landingPos - new Vector2(0, 100)) < 20 * 20)
                            {
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;
                                TimerRand++;

                                AniType = (int)AnimType.None;
                            }
                            else
                                NPC.Move(landingPos - new Vector2(0, 100), 10, 20);
                            break;
                        case 2:
                            if (NPC.velocity.Y == 0)
                            {
                                NPC.LookAtEntity(player);
                                EmoteBubble.NewBubble(15, new WorldUIAnchor(NPC), 90);
                                NPC.Shoot(NPC.Center + new Vector2(10 * NPC.spriteDirection, -16), ModContent.ProjectileType<FowlEmperor_Crown_Proj>(), 0, NPC.velocity / 4, NPC.whoAmI);
                                NPC.velocity.X = 0;
                                hideCrown = true;
                                TimerRand++;
                                for (int p = 0; p < Main.maxProjectiles; p++)
                                {
                                    Projectile crown = Main.projectile[p];
                                    if (!crown.active || crown.type != ModContent.ProjectileType<FowlEmperor_Crown_Proj>() || crown.ai[0] != NPC.whoAmI)
                                        continue;

                                    crownProj = crown;
                                }
                            }
                            break;
                        case 3:
                            AniType = (int)AnimType.Laugh;
                            if (AITimer++ >= 90)
                            {
                                if (crownProj is null)
                                    NPC.LookAtEntity(crownProj);
                                NPC.frame.Y = 8 * 76;
                                AniType = (int)AnimType.Stun;
                            }
                            if (AITimer >= 180)
                            {
                                AITimer = 0;
                                TimerRand++;
                            }
                            break;
                        case 4:
                            AniType = (int)AnimType.None;
                            if (crownProj is null)
                                AITimer = 120;
                            else
                            {
                                NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, crownProj.Center.Y);
                                NPCHelper.HorizontallyMove(NPC, crownProj.Center, 0.08f, 2f, 18, 18, NPC.Center.Y > crownProj.Center.Y, crownProj);
                            }
                            if (AITimer++ >= 120)
                            {
                                NPC.velocity.X = 0;
                                EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 80);
                                AniType = (int)AnimType.Recrown;
                                AITimer = 0;
                                TimerRand++;
                            }
                            break;
                        case 5:
                            if (AniType is (int)AnimType.None)
                            {
                                AniType = (int)AnimType.Recrown;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                TimerRand = Main.rand.Next(140, 201);
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    float moveSpeed = 1.4f;
                    if (player.Center.DistanceSQ(NPC.Center) > 800 * 800 || empowered)
                        moveSpeed = 3f;
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                    NPCHelper.HorizontallyMove(NPC, player.Center, 0.08f, moveSpeed, 18, 24, NPC.Center.Y > player.Center.Y, player);

                    if (NPC.velocity.Y == 0 && AniType != (int)AnimType.Throw && NPC.Hitbox.Intersects(player.Hitbox) && Main.rand.NextBool(4))
                    {
                        SoundEngine.PlaySound(SoundID.Item1, NPC.position);
                        NPC.LookAtEntity(player);
                        AniType = (int)AnimType.Throw;
                    }
                    if (AniType == (int)AnimType.Throw)
                        NPC.frameCounter++;
                    else
                        AniType = (int)AnimType.None;

                    if (AITimer++ >= TimerRand && NPC.velocity.Y == 0)
                    {
                        AITimer = 0;
                        NPC.velocity.X = 0;
                        switch (AttackType)
                        {
                            default:
                                TimerRand = Main.rand.Next(40, 81);
                                AIState = ActionState.FeatherThrow;
                                break;
                            case 1:
                                TimerRand = 0;
                                AIState = ActionState.FlutterAtk;
                                break;
                            case 2:
                                TimerRand = 0;
                                AIState = ActionState.Admire;
                                break;
                        }
                    }
                    break;
                case ActionState.FeatherThrow:
                    switch (TimerRand2)
                    {
                        case 0:
                            AniType = (int)AnimType.Laugh;
                            if (AITimer++ >= TimerRand)
                            {
                                NPC.LookAtEntity(player);
                                AniType = (int)AnimType.Throw;
                                TimerRand2++;
                                TimerRand = 0;
                                AITimer = 0;
                            }
                            break;
                        case 1:
                            if (AITimer++ == 12)
                            {
                                int spread = Main.expertMode || empowered ? 8 : 16;
                                float speed = MathHelper.Distance(player.Center.X, NPC.Center.X) / 50;
                                speed = MathHelper.Clamp(speed, 4, 20);
                                for (int i = 0; i < (Main.expertMode || empowered ? 5 : 3); i++)
                                {
                                    int rot = spread * i;
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<FowlFeather_Proj>(), NPC.damage, new Vector2(speed * NPC.spriteDirection, -Main.rand.Next(9, 12)).RotatedBy(MathHelper.ToRadians(rot - (Main.expertMode || empowered ? 20 : 16))), SoundID.Item1);
                                }
                            }
                            if (AITimer == 24 && TimerRand <= (NPC.life <= NPC.lifeMax / 2 || empowered ? (empowered ? 2 : 1) : 0) && (player.velocity.X >= 2 || player.velocity.X <= -2))
                            {
                                NPC.LookAtEntity(player);
                                NPC.frame.Y = 0;
                                NPC.frameCounter = 0;
                                AniType = (int)AnimType.Throw;
                                TimerRand++;
                                AITimer = 0;
                            }
                            if (AITimer >= 40)
                            {
                                AttackType = 1;
                                AITimer = 0;
                                TimerRand = Main.rand.Next(140, 201);
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                            }
                            break;
                    }
                    break;
                case ActionState.FlutterAtk:
                    switch (TimerRand2)
                    {
                        case 0:
                            if (AITimer++ == 0)
                            {
                                NPC.velocity.X = 0;
                                NPC.velocity.Y = -7;
                            }
                            if (AITimer > 2 && NPC.velocity.Y >= 0)
                            {
                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                                TimerRand = player.RightOfDir(NPC);
                                AniType = (int)AnimType.Flap;
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (AITimer++ < 120)
                                NPC.Move(player.Center + new Vector2(400 * -TimerRand, -Main.rand.Next(200, 251)), 10, 30);
                            else
                            {
                                NPC.Move(player.Center + new Vector2(400 * TimerRand, -Main.rand.Next(200, 251)), 10, 40);
                                if (AITimer % (NPC.life <= NPC.lifeMax / 2 || empowered ? 4 : 6) == 0 && Main.rand.NextBool())
                                {
                                    SoundEngine.PlaySound(SoundID.Item16, NPC.position);
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Rooster_EggBomb>(), NPC.damage * 2, new Vector2(0, -4) + RedeHelper.SpreadUp(4));

                                }
                            }
                            if (AITimer >= 300 || (TimerRand == -1 ? NPC.Center.X <= player.Center.X - 400 : NPC.Center.X >= player.Center.X + 400))
                            {
                                if (repeat <= 0 && Main.rand.NextBool())
                                {
                                    TimerRand *= -1;
                                    AITimer = 60;
                                    repeat++;
                                }
                                else
                                {
                                    repeat = 0;
                                    AITimer = 0;
                                    TimerRand2++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 2:
                            NPC.Move(player.Center + new Vector2(0, -20), 10, 30);
                            if (AITimer++ >= 30 && Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(NPC.position + new Vector2(0, 10), NPC.width, NPC.height - 10))
                            {
                                AttackType = 2;
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;
                                TimerRand = Main.rand.Next(140, 201);
                                AniType = (int)AnimType.None;
                                AITimer = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Admire:
                    switch (TimerRand2)
                    {
                        case 0:
                            NPC.LookAtEntity(player);
                            AniType = (int)AnimType.Admire;
                            TimerRand2++;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (AITimer++ == 20)
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FowlEmperor_Crown>(), NPC.whoAmI);
                            if (AITimer == 240 - 30)
                                AniType = (int)AnimType.Recrown;
                            if (AITimer >= 240)
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    int dust = Dust.NewDust(NPC.position + new Vector2(0, 40), NPC.width, NPC.height, ModContent.DustType<DustSpark2>(), newColor: Color.IndianRed, Scale: 2f);
                                    Main.dust[dust].velocity.Y = -2;
                                    Main.dust[dust].velocity.X = 0;
                                    Main.dust[dust].noGravity = true;
                                }
                                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
                                empowered = true;
                                AttackType = 0;
                                TimerRand = Main.rand.Next(140, 201);
                                AniType = (int)AnimType.None;
                                AITimer = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            AniType = (int)AnimType.Stun;
                            if (AITimer++ >= 120)
                            {
                                empowered = false;
                                moveTo = NPC.FindGroundPlayer(30);
                                AniType = (int)AnimType.Mad;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.RoosterRoar, NPC.position);
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 3:
                            if (AITimer++ % 180 == 0 || (NPC.Center.X + 10 > moveTo.X * 16 && NPC.Center.X - 10 < moveTo.X * 16))
                                moveTo = NPC.FindGroundPlayer(30);

                            if (AITimer % 80 == 0 && Main.rand.NextBool() && NPC.CountNPCS(ModContent.NPCType<Chickenvoy>()) < 4)
                            {
                                int chickType = ModContent.NPCType<Chickenvoy>();
                                if (Main.rand.NextBool() && NPC.CountNPCS(ModContent.NPCType<Chickadier>()) < 2)
                                    chickType = ModContent.NPCType<Chickadier>();
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)player.position.X + (Main.rand.Next(500, 801) * (Main.rand.NextBool() ? -1 : 1)), (int)player.position.Y + 800, chickType, 0, 0, 0, NPC.whoAmI);
                            }
                            if (AITimer % 120 == 0 && Main.rand.NextBool() && NPC.CountNPCS(ModContent.NPCType<CoopCrate>()) < 1)
                            {
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)player.position.X + (Main.rand.Next(500, 801) * (Main.rand.NextBool() ? -1 : 1)), (int)player.position.Y + 800, ModContent.NPCType<CoopCrate>(), 0, 0, 0, NPC.whoAmI);
                            }
                            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                            NPCHelper.HorizontallyMove(NPC, moveTo, 0.2f, 12, 30, 30, NPC.Center.Y > player.Center.Y, player);

                            if (eggCracked)
                            {
                                NPC.velocity.Y = -12;
                                NPC.velocity.X = NPC.RightOfDir(player) * 2;
                                AITimer = 0;
                                TimerRand2++;
                            }
                            break;
                        case 4:
                            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].velocity.X = 0f;
                            Main.dust[dustIndex].velocity.Y = -5f;
                            NPC.rotation += NPC.velocity.X * 0.07f;
                            if (AITimer++ > 0 && BaseAI.HitTileOnSide(NPC, 3))
                            {
                                NPC.rotation = 0;
                                AniType = (int)AnimType.Stun;
                                NPC.velocity.X = 0;
                                AITimer = 0;
                                TimerRand2++;
                            }
                            break;
                        case 5:
                            if (AITimer < 180 - 30)
                                AniType = (int)AnimType.Stun;
                            if (AITimer == 180 - 30)
                                AniType = (int)AnimType.Recrown;
                            if (AITimer++ > 180)
                            {
                                AttackType = 0;
                                NPC.frameCounter = 0;
                                eggCracked = false;
                                AniType = (int)AnimType.None;
                                AITimer = 0;
                                TimerRand = Main.rand.Next(140, 201);
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
            if (empowered && Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(NPC.position + new Vector2(0, 40), NPC.width, NPC.height, ModContent.DustType<DustSpark2>(), newColor: Color.IndianRed, Scale: 2f);
                Main.dust[dust].velocity.Y = -2;
                Main.dust[dust].velocity.X = 0;
                Main.dust[dust].noGravity = true;
            }
        }
        private void CustomFrames(int frameHeight)
        {
            switch ((AnimType)AniType)
            {
                case AnimType.None:
                    NPC.frame.X = 0;
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.rotation = 0;
                        if (NPC.velocity.X == 0)
                            NPC.frame.Y = 0;
                        else
                        {
                            if (NPC.frame.Y < frameHeight)
                                NPC.frame.Y = frameHeight;

                            NPC.frameCounter += NPC.velocity.X * 0.5f;
                            if (NPC.frameCounter is >= 3 or <= -3)
                            {
                                NPC.frameCounter = 0;
                                NPC.frame.Y += frameHeight;
                                if (NPC.frame.Y > 8 * frameHeight)
                                    NPC.frame.Y = frameHeight;
                            }
                        }
                    }
                    else
                    {
                        NPC.rotation = NPC.velocity.X * 0.05f;
                        NPC.frame.Y = 10 * frameHeight;
                        NPC.frameCounter = 10;
                    }
                    break;
                case AnimType.Flap:
                    NPC.frame.X = 0;
                    if (NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y = 9 * frameHeight;

                    NPC.rotation = NPC.velocity.X * 0.05f;
                    if (NPC.frameCounter++ >= 3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y % 4 * frameHeight == 0)
                            SoundEngine.PlaySound(SoundID.Item32, NPC.position);
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 9 * frameHeight;
                    }
                    break;
                case AnimType.Laugh:
                    NPC.frame.X = 0;
                    if (NPC.frame.Y < 13 * frameHeight)
                        NPC.frame.Y = 13 * frameHeight;

                    if (NPC.frameCounter++ >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 14 * frameHeight)
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit48 with { Pitch = .6f }, NPC.position);
                            NPC.frame.Y = 13 * frameHeight;
                        }
                    }
                    break;
                case AnimType.Throw:
                    NPC.velocity.X = 0;
                    NPC.frame.X = 0;
                    if (NPC.frame.Y < 15 * frameHeight)
                        NPC.frame.Y = 15 * frameHeight;

                    if (NPC.frameCounter++ >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 19 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                            AniType = (int)AnimType.None;
                        }
                    }
                    break;
                case AnimType.Admire:
                    NPC.frame.X = NPC.frame.Width;
                    if (NPC.frameCounter++ >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 6 * frameHeight)
                            NPC.frame.Y = 3 * frameHeight;
                    }
                    break;
                case AnimType.Recrown:
                    NPC.frame.X = NPC.frame.Width;
                    if (NPC.frame.Y > 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;
                    if (NPC.frameCounter++ >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y -= frameHeight;
                        if (NPC.frame.Y < 0)
                        {
                            hideCrown = false;
                            NPC.frame.Y = 0;
                            AniType = (int)AnimType.None;
                        }
                    }
                    break;
                case AnimType.Stun:
                    NPC.frame.X = NPC.frame.Width;
                    if (eggCracked)
                    {
                        if (++NPC.frameCounter >= 5)
                        {
                            NPC.frameCounter = 0;
                            if (++StunFrame > 3)
                                StunFrame = 0;
                        }
                        NPC.frame.Y = 7 * frameHeight;
                        break;
                    }
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y = 7 * frameHeight;
                    if (NPC.frameCounter++ >= 30)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 8 * frameHeight;
                    }
                    break;
                case AnimType.Mad:
                    NPC.frame.X = NPC.frame.Width;
                    if (eggCracked)
                    {
                        NPC.frame.Y = 9 * frameHeight;
                        break;
                    }
                    if (NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y = 9 * frameHeight;
                    NPC.frameCounter += Math.Abs(NPC.velocity.X * 0.5f);
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 16 * frameHeight)
                            NPC.frame.Y = 9 * frameHeight;
                    }
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private int StunFrame;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frame.X = 0;
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = frameHeight;
                    }
                }
            }
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (empowered)
                modifiers.Defense.Base += 10;
            if (eggCracked && AniType is (int)AnimType.Stun)
                modifiers.FinalDamage *= 2;
            if (AniType is (int)AnimType.Mad)
                modifiers.FinalDamage *= 0.1f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (AniType is (int)AnimType.Mad && projectile.type == ModContent.ProjectileType<EggCracker_Proj>())
            {
                eggCracked = true;
                modifiers.FinalDamage *= 10;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.5f;
        }
        private bool hideCrown;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D crownTex = ModContent.Request<Texture2D>(Texture + "_CrownSheet").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy && (AniType is (int)AnimType.Mad or (int)AnimType.Flap))
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - new Vector2(0, 8) - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }

            spriteBatch.Draw(texture, NPC.Center - new Vector2(0, 8) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            if (!hideCrown)
                spriteBatch.Draw(crownTex, NPC.Center - new Vector2(0, 8) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            if (!eggCracked || AniType is not (int)AnimType.Stun)
                return false;

            Texture2D starTex = ModContent.Request<Texture2D>("Redemption/Textures/StunVisual").Value;
            int height = starTex.Height / 4;
            int y = height * StunFrame;
            Vector2 drawOrigin = new(starTex.Width / 2, height / 2);

            spriteBatch.Draw(starTex, NPC.Center - new Vector2(0, 44) - Main.screenPosition, new Rectangle?(new Rectangle(0, y, starTex.Width, height)), Color.White, 0, drawOrigin, 1, 0, 0);

            return false;
        }

        private bool DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    AniType = (int)AnimType.Flap;
                    NPC.velocity.Y -= 1f;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return true;
                }
            }
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                SoundEngine.PlaySound(SoundID.Item16 with { Pitch = -0.7f }, NPC.position);
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke);
                for (int i = 0; i < 100; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 3f;

                    int dust2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), Scale: 2);
                    Main.dust[dust2].velocity *= 10f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust1>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
    public class FowlEmperor_Crown : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public static int BodyType() => ModContent.NPCType<FowlEmperor>();
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Very Hittable Looking Crown");
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 40;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 22;
            NPC.height = 28;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 225;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCHit4;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 40;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC npc = Main.npc[(int)NPC.ai[0]];
                npc.ai[1] = 0;
                npc.ai[3] = 2;
                npc.netUpdate = true;
                if (Main.netMode == NetmodeID.Server)
                    return;
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/FowlEmperor_Crown").Type, 1);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.npc[(int)NPC.ai[0]].ModNPC is FowlEmperor fowl)
                NPC.life = fowl.crownLife;
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.type == ItemID.SlapHand)
                modifiers.FlatBonusDamage += 999;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)NPC.ai[0]];
            if (!npc.active || npc.type != BodyType() || npc.ai[0] != 4 || npc.ai[1] > 240 - 30)
                NPC.active = false;
            if (npc.ModNPC is FowlEmperor fowl)
                fowl.crownLife = NPC.life;
            NPC.Center = npc.Center + new Vector2(27 * npc.spriteDirection, 2);
        }
    }
}