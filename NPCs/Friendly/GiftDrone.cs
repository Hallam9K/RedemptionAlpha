using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.DataStructures;
using Redemption.Buffs.NPCBuffs;
using Redemption.Buffs.Debuffs;
using Redemption.Base;
using Redemption.Items.Usable;
using Terraria.ModLoader.Utilities;
using Redemption.BaseExtension;
using System;

namespace Redemption.NPCs.Friendly
{
    public class GiftDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gift Drone");
            Main.npcFrameCount[NPC.type] = 3;
            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 16;
            NPC.friendly = true;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }

            switch (NPC.ai[0])
            {
                case 0: // Fly Down
                    NPC.ai[2]++;
                    if (NPC.DistanceSQ(player.Center) < 200 * 200 || NPC.ai[2] > 220)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[2] = 0;
                    }
                    else
                        NPC.Move(player.Center + new Vector2(100 * player.direction, -100), 7, 15);
                    break;
                case 1: // Stop
                    Point tile = NPC.Center.ToTileCoordinates();
                    if (Framing.GetTileSafely(tile.X, tile.Y).HasTile)
                    {
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y -= 0.1f;
                        NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -2f, 8);
                    }
                    else
                    {
                        NPC.velocity *= 0.96f;
                        if (NPC.ai[2]++ > 120 && NPC.DistanceSQ(player.Center) < 200 * 200)
                        {
                            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.Center.X, (int)NPC.Center.Y + 26, 1, 1, ModContent.ItemType<OmegaGift>(), 1, false, 0, true, false);
                            NPC.ai[2] = 0;
                            NPC.ai[0] = 2;
                        }
                    }
                    break;
                case 2: // Yeet out
                    if (NPC.ai[2]++ > 10)
                    {
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
                            NPC.active = false;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight)
                    NPC.frame.Y = 0;
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.Redemption().foundLab || spawnInfo.Player.Redemption().omegaGiftGiven || NPC.AnyNPCs(Type) || RedeHelper.BossActive())
                return 0;

            return SpawnCondition.OverworldDay.Chance * 0.2f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D gift = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Gift").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (NPC.ai[0] < 2)
                spriteBatch.Draw(gift, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation * 2, NPC.frame.Size() / 2 + new Vector2(-6, -10), NPC.scale, effects, 0);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}