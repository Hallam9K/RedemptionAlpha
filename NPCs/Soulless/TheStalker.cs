using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System.Threading;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class TheStalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 74;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }
        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            switch (NPC.ai[0])
            {
                case 0:
                    if (SoullessArea.soullessInts[1] > 1)
                        NPC.active = false;
                    break;
                case 1:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Rectangle activeZone = new((473 + SoullessArea.Offset.X) * 16, (1097 + SoullessArea.Offset.Y) * 16, 17 * 16, 9 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                            {
                                SoullessArea.soullessInts[2] = 1;
                                NPC.ai[1] = 1;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            break;
                        case 1:
                            if (SoullessArea.soullessInts[1] > 4)
                                NPC.active = false;

                            if (NPC.ai[2]++ == 0)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 eyesPos = RedeHelper.RandomPosition(new Vector2(510 + SoullessArea.Offset.X, 1118 + SoullessArea.Offset.Y), new Vector2(554 + SoullessArea.Offset.X, 1125 + SoullessArea.Offset.Y)) * 16;
                                    NPC.NewNPC(new EntitySource_SpawnNPC(), (int)eyesPos.X, (int)eyesPos.Y, ModContent.NPCType<StalkingEyes>());
                                }
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (284 + SoullessArea.Offset.X) * 16, (1084 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TheStalker_Fake>());
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(new Vector2(459 + SoullessArea.Offset.X, 1175 + SoullessArea.Offset.Y) * 16, ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 1);
                                NPC.Shoot(new Vector2(518 + SoullessArea.Offset.X, 1190 + SoullessArea.Offset.Y) * 16, ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 4);
                            }

                            NPC.spriteDirection = 1;
                            if (SoullessArea.soullessInts[1] <= 5 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                            }
                            break;
                        case 1:
                            break;
                    }
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.NegativeDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + RedeHelper.Spread(2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor) * .5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override bool CheckActive() => false;
    }
    public class TheStalker_Fake : TheStalker
    {
        public override string Texture => "Redemption/NPCs/Soulless/TheStalker";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool PreAI()
        {
            Player player = Main.player[Main.myPlayer];
            if (SoullessArea.soullessInts[2] != 1)
                NPC.active = false;
            switch (NPC.ai[0])
            {
                case 0:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Rectangle activeZone = new((300 + SoullessArea.Offset.X) * 16, (1071 + SoullessArea.Offset.Y) * 16, 36 * 16, 21 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                                NPC.ai[1] = 1;
                            break;
                        case 1:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
            }
            return false;
        }
    }
}