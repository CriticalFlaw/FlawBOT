using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
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

    #region ITEM_SCHEMA

    public class Capabilities
    {
        [JsonProperty("nameable")]
        public bool Nameable { get; set; }

        [JsonProperty("can_gift_wrap")]
        public bool CanGiftWrap { get; set; }

        [JsonProperty("can_craft_mark")]
        public bool CanCraft { get; set; }

        [JsonProperty("can_be_restored")]
        public bool CanBeRestored { get; set; }

        [JsonProperty("strange_parts")]
        public bool StrangeParts { get; set; }

        [JsonProperty("can_card_upgrade")]
        public bool CanCardUpgrade { get; set; }

        [JsonProperty("can_strangify")]
        public bool CanStrangify { get; set; }

        [JsonProperty("can_killstreakify")]
        public bool CanKillstreakify { get; set; }

        [JsonProperty("can_consume")]
        public bool CanConsume { get; set; }

        [JsonProperty("can_collect")]
        public bool? CanCollect { get; set; }

        [JsonProperty("paintable")]
        public bool? Paintable { get; set; }

        [JsonProperty("can_craft_if_purchased")]
        public bool? CanCraftIfPurchased { get; set; }

        [JsonProperty("can_craft_count")]
        public bool? CanCraftCount { get; set; }

        [JsonProperty("can_unusualify")]
        public bool? CanUnusualify { get; set; }

        [JsonProperty("usable_gc")]
        public bool? UsableGC { get; set; }

        [JsonProperty("usable")]
        public bool? Usable { get; set; }

        [JsonProperty("can_customize_texture")]
        public bool? CanCustomizeTexture { get; set; }

        [JsonProperty("usable_out_of_game")]
        public bool? UsableOutOfGame { get; set; }

        [JsonProperty("can_spell_page")]
        public bool? CanSpellPage { get; set; }

        [JsonProperty("duck_upgradable")]
        public bool? DuckUpgradable { get; set; }
    }

    public class AdditionalHiddenBodygroups
    {
        [JsonProperty("hat")]
        public int Hat { get; set; }

        [JsonProperty("headphones")]
        public int Headphones { get; set; }

        [JsonProperty("head")]
        public int? Head { get; set; }
    }

    public class Style
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additional_hidden_bodygroups")]
        public AdditionalHiddenBodygroups HiddenBodyGroups { get; set; }
    }

    public class Attribute
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("@class")]
        public string Class { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public class PerClassLoadoutSlots
    {
        [JsonProperty("Soldier")]
        public string Soldier { get; set; }

        [JsonProperty("Heavy")]
        public string Heavy { get; set; }

        [JsonProperty("Pyro")]
        public string Pyro { get; set; }

        [JsonProperty("Engineer")]
        public string Engineer { get; set; }

        [JsonProperty("Demoman")]
        public string Demoman { get; set; }
    }

    public class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class SchemaItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("defindex")]
        public int DefIndex { get; set; }

        [JsonProperty("item_class")]
        public string ItemClass { get; set; }

        [JsonProperty("item_type_name")]
        public string ItemTypeName { get; set; }

        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        [JsonProperty("proper_name")]
        public bool ProperName { get; set; }

        [JsonProperty("item_slot")]
        public string ItemSlot { get; set; }

        [JsonProperty("model_player")]
        public string ModelPlayer { get; set; }

        [JsonProperty("item_quality")]
        public int ItemQuality { get; set; }

        [JsonProperty("image_inventory")]
        public string ImageInventory { get; set; }

        [JsonProperty("min_ilevel")]
        public int MinILevel { get; set; }

        [JsonProperty("max_ilevel")]
        public int MaxILevel { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("image_url_large")]
        public string ImageUrlLarge { get; set; }

        [JsonProperty("craft_class")]
        public string CraftClass { get; set; }

        [JsonProperty("craft_material_type")]
        public string CraftMaterialType { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }

        [JsonProperty("used_by_classes")]
        public List<object> UsedByClasses { get; set; }

        [JsonProperty("item_description")]
        public string ItemDescription { get; set; }

        [JsonProperty("styles")]
        public List<Style> Styles { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }

        [JsonProperty("drop_type")]
        public string DropType { get; set; }

        [JsonProperty("item_set")]
        public string ItemSet { get; set; }

        [JsonProperty("holiday_restriction")]
        public string HolidayRestriction { get; set; }

        [JsonProperty("per_class_loadout_slots")]
        public PerClassLoadoutSlots PerClassLoadoutSlot { get; set; }

        [JsonProperty("tool")]
        public Tool Tool { get; set; }
    }

    public class Result
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("items_game_url")]
        public string ItemsGameUrl { get; set; }

        [JsonProperty("items")]
        public List<SchemaItem> Items { get; set; }

        [JsonProperty("next")]
        public int Next { get; set; }
    }

    public class TF2ItemSchema
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    #endregion ITEM_SCHEMA
}