using System;
using System.Reflection;

namespace SpeedrunManager
{
    public enum BossNameEnum
    {
        [BossNameAttr("TrophyEikthyr",          "$enemy_eikthyr")]     Eikthyr,
        [BossNameAttr("TrophyTheElder",         "$enemy_gdking")]      gd_king,
        [BossNameAttr("TrophyBonemass",         "$enemy_bonemass")]    Bonemass,
        [BossNameAttr("TrophyDragonQueen",      "$enemy_dragon")]      Dragon,
        [BossNameAttr("TrophyGoblinKing",       "$enemy_goblinking")]  GoblinKing,
        [BossNameAttr("TrophySeekerQueen",      "$enemy_seekerqueen")] SeekerQueen,
        [BossNameAttr("TrophyFader",            "$enemy_fader")]       Fader
    }
    
    class BossNameAttr: Attribute
    {
        internal BossNameAttr(string trophySpriteKey, string translationKey)
        {
            this.trophySpriteKey = trophySpriteKey;
            this.translationKey = translationKey;
        }
        
        public string translationKey { get; private set; }
        public string trophySpriteKey { get; private set; }
    }

    public static class BossNameFields
    {
        public static string GetTranslationKey(this BossNameEnum p)
        {
            BossNameAttr attr = GetAttr(p);
            return attr.translationKey;
        }

        public static string GetTrophySpriteKey(this BossNameEnum p)
        {
            BossNameAttr attr = GetAttr(p);
            return attr.trophySpriteKey;
        }

        private static BossNameAttr GetAttr(BossNameEnum p)
        {
            return (BossNameAttr)Attribute.GetCustomAttribute(ForValue(p), typeof(BossNameAttr));
        }

        private static MemberInfo ForValue(BossNameEnum p)
        {
            return typeof(BossNameEnum).GetField(Enum.GetName(typeof(BossNameEnum), p));
        }
    }
}