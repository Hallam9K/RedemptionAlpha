using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Redemption.Textures.Emotes
{
    public abstract class RedeTownEmote : ModEmoteBubble
    {
        public override string Texture => "Redemption/Textures/Emotes/RedeTownEmote";
        public override void SetStaticDefaults()
        {
            AddToCategory(EmoteID.Category.Town);
        }
        public virtual int Row => 0;
        public override Rectangle? GetFrame()
        {
            return new Rectangle(EmoteBubble.frame * 34, 28 * Row, 34, 28);
        }
        public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter)
        {
            return new Rectangle(frame * 34, 28 * Row, 34, 28);
        }
    }
    public class AdamTownNPCEmote : RedeTownEmote
    {
        public override int Row => 0;
    }
    public class DaerelTownNPCEmote : RedeTownEmote
    {
        public override int Row => 1;
    }
    public class ForestNymphTownNPCEmote : RedeTownEmote
    {
        public override int Row => 2;
    }
    public class HappinsTownNPCEmote : RedeTownEmote
    {
        public override int Row => 3;
    }
    public class NewbTownNPCEmote : RedeTownEmote
    {
        public override int Row => 4;
    }
    public class OkvotTownNPCEmote : RedeTownEmote
    {
        public override int Row => 5;
    }
    public class TenvonTownNPCEmote : RedeTownEmote
    {
        public override int Row => 6;
    }
    public class ZephosTownNPCEmote : RedeTownEmote
    {
        public override int Row => 7;
    }
}