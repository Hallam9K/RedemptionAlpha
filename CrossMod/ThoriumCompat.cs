using Terraria.ModLoader;

namespace Redemption.CrossMod;


public static class ThoriumHelper
{
    private static readonly Mod thorium = CrossMod.Thorium.Instance;

    public static bool AddFlailProjectileID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddFlailProjectileID", type);
    }

    public static bool AddPlayerDoTBuffID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddPlayerDoTBuffID", type);
    }

    public static bool AddPlayerStatusBuffID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddPlayerStatusBuffID", type);
    }

    public static bool AddGemStoneTileID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddGemStoneTileID", type);
    }

    public static bool AddSkeletonRepellentNPCID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddSkeletonRepellentNPCID", type);
    }

    public static bool AddInsectRepellentNPCID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddInsectRepellentNPCID", type);
    }

    public static bool AddFishRepellentNPCID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddFishRepellentNPCID", type);
    }

    public static bool AddZombieRepellentNPCID(int type)
    {
        if (!CrossMod.Thorium.Enabled)
            return false;

        return (bool)thorium.Call("AddZombieRepellentNPCID", type);
    }
}