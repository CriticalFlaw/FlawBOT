using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    #region TEAMWORK_NEWS

    public class TeamworkNews
    {
        [JsonProperty("hash")]
        public string HASH { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }

    #endregion TEAMWORK_NEWS

    #region TEAMWORK_SERVERS

    public class TeamworkServer
    {
        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("reachable")]
        public bool Reachable { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("valve_secure")]
        public bool ValveSecure { get; set; }

        [JsonProperty("map_name")]
        public string MapName { get; set; }

        [JsonProperty("map_name_thumbnail")]
        public string MapThumbnail { get; set; }

        [JsonProperty("map_name_next")]
        public string NextMap { get; set; }

        [JsonProperty("players")]
        public int PlayerCount { get; set; }

        [JsonProperty("max_players")]
        public int PlayerMax { get; set; }

        [JsonProperty("gamemodes")]
        public List<string> GameModes { get; set; }

        [JsonProperty("gametype")]
        public string GameType { get; set; }

        [JsonProperty("has_password")]
        public bool? HasPassword { get; set; }

        [JsonProperty("has_rtd")]
        public bool HasRTD { get; set; }

        [JsonProperty("has_randomcrits")]
        public bool? HasRandomCrits { get; set; }

        [JsonProperty("has_norespawntime")]
        public bool HasNoSpawnTimer { get; set; }

        [JsonProperty("has_alltalk")]
        public bool HasAllTalk { get; set; }
    }

    #endregion TEAMWORK_SERVERS

    #region TEAMWORK_MAPS

    public class TeamworkMap
    {
        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("first_seen")]
        public object FirstSeen { get; set; }

        [JsonProperty("last_seen")]
        public string LastSeen { get; set; }

        [JsonProperty("all_gamemodes")]
        public List<string> GameModes { get; set; }

        [JsonProperty("all_server_types")]
        public List<string> ServerTypes { get; set; }

        [JsonProperty("highest_players")]
        public int HighestPlayerCount { get; set; }

        [JsonProperty("highest_servers")]
        public int HighestServerCount { get; set; }

        [JsonProperty("alltime_avg_players")]
        public string AvgPlayers { get; set; }

        [JsonProperty("alltime_avg_players_days")]
        public int AvgPlayersDays { get; set; }

        [JsonProperty("official_map")]
        public bool OfficialMap { get; set; }

        [JsonProperty("normalized_map_name")]
        public string MapName { get; set; }

        [JsonProperty("related_maps")]
        public List<string> RelatedMaps { get; set; }
    }

    #endregion TEAMWORK_MAPS

    #region SCHEMA

    public class TFItemSchema
    {
        [JsonProperty("result")]
        public Result Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("originNames")]
        public List<ItemOrigin> OriginNames { get; set; }

        [JsonProperty("attribute_controlled_attached_particles")]
        public List<UnusualEffect> UnusualEffect { get; set; }

        [JsonProperty("qualities")]
        public QualityId QualityId { get; set; }

        [JsonProperty("qualityNames")]
        public QualityName QualityName { get; set; }

        [JsonProperty("attributes")]
        public List<Attributes> Attributes { get; set; }

        [JsonProperty("items")]
        public List<SchemaItem> Items { get; set; }
    }

    public class ItemOrigin
    {
        [JsonProperty("origin")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UnusualEffect
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class QualityId
    {
        [JsonProperty("Unique")]
        public string Unique { get; set; }

        [JsonProperty("selfmade")]
        public string SelfMade { get; set; }

        [JsonProperty("collectors")]
        public string Collectors { get; set; }

        [JsonProperty("Normal")]
        public string Normal { get; set; }

        [JsonProperty("vintage")]
        public string Vintage { get; set; }

        [JsonProperty("completed")]
        public string Completed { get; set; }

        [JsonProperty("customized")]
        public string Customized { get; set; }

        [JsonProperty("rarity1")]
        public string Rarity1 { get; set; }

        [JsonProperty("rarity2")]
        public string Rarity2 { get; set; }

        [JsonProperty("rarity3")]
        public string Rarity3 { get; set; }

        [JsonProperty("rarity4")]
        public string Rarity4 { get; set; }

        [JsonProperty("paintkitweapon")]
        public string PaintKit { get; set; }

        [JsonProperty("strange")]
        public string Strange { get; set; }

        [JsonProperty("haunted")]
        public string Haunted { get; set; }

        [JsonProperty("community")]
        public string Community { get; set; }

        [JsonProperty("developer")]
        public string Developer { get; set; }
    }

    public class QualityName
    {
        [JsonProperty("Unique")]
        public string Unique { get; set; }

        [JsonProperty("selfmade")]
        public string SelfMade { get; set; }

        [JsonProperty("collectors")]
        public string Collectors { get; set; }

        [JsonProperty("Normal")]
        public string Normal { get; set; }

        [JsonProperty("vintage")]
        public string Vintage { get; set; }

        [JsonProperty("completed")]
        public string Completed { get; set; }

        [JsonProperty("customized")]
        public string Customized { get; set; }

        [JsonProperty("rarity1")]
        public string Rarity1 { get; set; }

        [JsonProperty("rarity2")]
        public string Rarity2 { get; set; }

        [JsonProperty("rarity3")]
        public string Rarity3 { get; set; }

        [JsonProperty("rarity4")]
        public string Rarity4 { get; set; }

        [JsonProperty("paintkitweapon")]
        public string PaintKit { get; set; }

        [JsonProperty("strange")]
        public string Strange { get; set; }

        [JsonProperty("haunted")]
        public string Haunted { get; set; }

        [JsonProperty("community")]
        public string Community { get; set; }

        [JsonProperty("developer")]
        public string Developer { get; set; }
    }

    public class Attributes
    {
        [JsonProperty("defindex")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description_string")]
        public string Description { get; set; }

        [JsonProperty("effect_type")]
        public string EffectType { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }
    }

    public class Level
    {
        [JsonProperty("level")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("required_score")]
        public int ReqScore { get; set; }
    }

    #endregion SCHEMA

    #region ITEM

    public class SchemaItem
    {
        [JsonProperty("defindex")]
        public int DefIndex { get; set; }

        [JsonProperty("used_by_classes")]
        public List<object> UsedByClasses { get; set; }

        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("name")]
        public string ItemName2 { get; set; }

        [JsonProperty("item_description")]
        public string Description { get; set; }

        [JsonProperty("image_inventory")]
        public string ItemImagePath { get; set; }

        [JsonProperty("item_type_name")]
        public string ItemType { get; set; }

        [JsonProperty("item_slot")]
        public string ItemSlot { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }

        [JsonProperty("image_url")]
        public string ImageURL { get; set; }

        [JsonProperty("image_url_large")]
        public string ImageURL_Large { get; set; }

        [JsonProperty("item_quality")]
        public int QualityId { get; set; }

        [JsonProperty("styles")]
        public List<Style> Styles { get; set; }

        [JsonProperty("attributes")]
        public List<ItemAttribute> Attributes { get; set; }

        [JsonProperty("item_set")]
        public string ItemSet { get; set; }

        [JsonProperty("holiday_restriction")]
        public string HolidayRestriction { get; set; }
    }

    public class Capabilities
    {
        [JsonProperty("can_craft_mark")]
        public bool Craftable { get; set; }

        [JsonProperty("can_card_upgrade")]
        public bool Upgradable { get; set; }

        [JsonProperty("can_be_restored")]
        public bool Restorable { get; set; }

        [JsonProperty("nameable")]
        public bool Renamable { get; set; }

        [JsonProperty("strange_parts")]
        public bool TakesStrangeParts { get; set; }

        [JsonProperty("can_killstreakify")]
        public bool Killstreakable { get; set; }

        [JsonProperty("can_consume")]
        public bool Consumeable { get; set; }

        [JsonProperty("can_strangify")]
        public bool Strangifyable { get; set; }

        [JsonProperty("can_gift_wrap")]
        public bool Giftable { get; set; }

        [JsonProperty("can_collect")]
        public bool? Collectable { get; set; }

        [JsonProperty("can_craft_if_purchased")]
        public bool? CraftableIfPurchased { get; set; }

        [JsonProperty("paintable")]
        public bool? Paintable { get; set; }

        [JsonProperty("can_unusualify")]
        public bool? Unusualifyable { get; set; }

        [JsonProperty("can_craft_count")]
        public bool? CraftCounted { get; set; }

        [JsonProperty("can_customize_texture")]
        public bool? CustomizeTexture { get; set; }
    }

    public class Style
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ItemAttribute
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double value { get; set; }

        [JsonProperty("@class")]
        public string Class { get; set; }
    }

    #endregion ITEM
}