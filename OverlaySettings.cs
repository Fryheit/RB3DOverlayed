namespace RB3DOverlayed
{
    using System.ComponentModel;
    using System.IO;
    using ff14bot.Helpers;
    using Color = System.Drawing.Color;
    
    public class OverlaySettings : JsonSettings
    {
        public OverlaySettings(string settingsPath)
            : base(settingsPath)
        {
        }

        private static OverlaySettings _instance;

        public static OverlaySettings Instance => _instance ?? (_instance = new OverlaySettings(Path.Combine(CharacterSettingsDirectory, "RBOverlayed.json")));

        #region Render
        
        [DefaultValue(true)]
        [Category("1. Render")]
        [DisplayName("Info Panel")]
        [Description("Enables drawing the info panel.")]
        public bool EnableInfoPanel { get; set; }
        
        [DefaultValue(false)]
        [Category("1. Render")]
        [DisplayName("Only draw in foreground")]
        [Description("Only draw when Final Fantasy 14 is in the foreground.")]
        public bool OnlyDrawInForeground { get; set; }

        
        #endregion
        
        #region Info Panel
        
        [DefaultValue(false)]
        [Category("2. Info Panel")]
        [DisplayName("Game Stats")]
        [Description("Displays status informations about the bot.")]
        public bool ShowGameStats { get; set; }

        [DefaultValue(true)]
        [Category("2. Info Panel")]
        [DisplayName("Nearby Hunts")]
        [Description("Displays a list of nearby hunt mobs.")]
        public bool ShowHuntList { get; set; }
        
        #endregion
        
        #region Info Panel Config
        
        [DefaultValue(true)]
        [Category("3. Info Panel Config")]
        [DisplayName("Use Shadowed Text")]
        [Description("Drawing the game stats shadowed.")]
        public bool UseShadowedText { get; set; }

        [DefaultValue(12f)]
        [Category("3. Info Panel Config")]
        [DisplayName("Font Size")]
        [Description("The font size to be used for drawing the stats.")]
        public float GameStatsFontSize { get; set; }

        [DefaultValue(40)]
        [Category("3. Info Panel Config")]
        [DisplayName("Position X")]
        [Description("The X position of the game stats.")]
        public int GameStatsPositionX { get; set; }

        [DefaultValue(100)]
        [Category("3. Info Panel Config")]
        [DisplayName("Position Y")]
        [Description("The Y position of the game stats.")]
        public int GameStatsPositionY { get; set; }

        [Category("3. Info Panel Config")]
        [DisplayName("Foreground color")]
        [Description("The foreground color of the game stats.")]
        public Color GameStatsForegroundColor
        {
            get { return Color.FromArgb(GameStatsForegroundColorArgb); }
            set { GameStatsForegroundColorArgb = value.ToArgb(); }
        }

        [Browsable(false)]
        [DefaultValue(-1)]
        public int GameStatsForegroundColorArgb { get; set; }

        [Category("3. Info Panel Config")]
        [DisplayName("Shadow color")]
        [Description("The shadow color of the game stats.")]
        public Color GameStatsShadowColor
        {
            get { return Color.FromArgb(GameStatsShadowColorArgb); }
            set { GameStatsShadowColorArgb = value.ToArgb(); }
        }

        [Browsable(false)]
        [DefaultValue(-16777216)]
        public int GameStatsShadowColorArgb { get; set; }

        #endregion

        #region Hunts

        [DefaultValue(true)]
        [Category("4. The Hunt")]
        [DisplayName("Mark S mobs")]
        [Description("Enables drawing S mobs in game")]
        public bool DrawHuntSMobs { get; set; }

        [DefaultValue(true)]
        [Category("4. The Hunt")]
        [DisplayName("Mark A mobs")]
        [Description("Enables drawing A mobs in game")]
        public bool DrawHuntAMobs { get; set; }

        [DefaultValue(true)]
        [Category("4. The Hunt")]
        [DisplayName("Mark B mobs")]
        [Description("Enables drawing B mobs in game")]
        public bool DrawHuntBMobs { get; set; }

        [DefaultValue(true)]
        [Category("4. The Hunt")]
        [DisplayName("Mark Eureka mobs")]
        [Description("Enables drawing Eureka mobs in game")]
        public bool DrawHuntEMobs { get; set; }

        #endregion

        #region Mobs

        [DefaultValue(false)]
        [Category("5. Mobs")]
        [DisplayName("Draw unit names")]
        [Description("Enables drawing unit names in game")]
        public bool DrawUnitNames { get; set; }


        [DefaultValue(false)]
        [Category("5. Mobs")]
        [DisplayName("Draw aggro range")]
        [Description("Enables drawing of aggro range circles towards units.")]
        public bool DrawAggroRangeCircles { get; set; }


        [DefaultValue(false)]
        [Category("5. Mobs")]
        [DisplayName("Draw hostility boxes")]
        [Description("Enables drawing of hostility boxes on units.")]
        public bool DrawHostilityBoxes { get; set; }


        [DefaultValue(false)]
        [Category("5. Mobs")]
        [DisplayName("Draw mob lines")]
        [Description("Enables drawing lines to all units around the player.")]
        public bool DrawUnitLines { get; set; }


        [DefaultValue(false)]
        [Category("5. Mobs")]
        [DisplayName("Check LoS")]
        [Description("Toggles if you always want lines to be drawn only when you are in line of sight of units.")]
        public bool DrawUnitLinesLos { get; set; }

        #endregion

        #region Players
        [DefaultValue(false)]
        [Category("6. Players")]
        [DisplayName("Draw self box")]
        [Description("Draw a box around own character")]
        public bool DrawSelfBox { get; set; }
        
        [DefaultValue(false)]
        [Category("6. Players")]
        [DisplayName("Draw other player boxes")]
        [Description("Enables drawing of boxes around players")]
        public bool DrawPlayers { get; set; }

        [DefaultValue(false)]
        [Category("6. Players")]
        [DisplayName("Draw player names")]
        [Description("Enables drawing player names in game")]
        public bool DrawPlayerNames { get; set; }

        #endregion
        
        #region Gameobjects

        [DefaultValue(false)]
        [Category("7. Game Objects")]
        [DisplayName("Draw gameobject names")]
        [Description("Enables drawing game object names in game")]
        public bool DrawGameObjectNames { get; set; }

        [DefaultValue(false)]
        [Category("7. Game Objects")]
        [DisplayName("Draw gameobject boxes")]
        [Description("Enables drawing of boxes around gameobjects.")]
        public bool DrawGameObjectBoxes { get; set; }


        [DefaultValue(false)]
        [Category("7. Game Objects")]
        [DisplayName("Draw gameobject lines")]
        [Description("Enables drawing lines to all gameobjects around the player.")]
        public bool DrawGameObjectLines { get; set; }


        [DefaultValue(false)]
        [Category("7. Game Objects")]
        [DisplayName("Check LoS")]
        [Description("Toggles if you always want lines to be drawn only when you are in line of sight of gameobjects.")]
        public bool DrawGameObjectLinesLos { get; set; }

        //[Category("Gameobjects")]
        //[DisplayName("Gameobjects color")]
        //[Description("The color of lines and boxes for gameobjects.")]
        //public Color GameobjectsColor
        //{
        //    get { return Color.FromArgb(GameobjectsColorArgb); }
        //    set { GameobjectsColorArgb = value.ToArgb(); }
        //}

        //[Setting]
        //[Browsable(false)]
        //[DefaultValue(-38476)] // Hot Pink
        //public int GameobjectsColorArgb { get; set; }

        #endregion
        
        #region Misc

        [DefaultValue(true)]
        [Category("8. Misc")]
        [DisplayName("Only Care about targetable")]
        [Description("Only draw boxes/text for units that are targetable (event objects ignore this)")]
        public bool OnlyRenderTargetable { get; set; }

        [DefaultValue(true)]
        [Category("8. Misc")]
        [DisplayName("Draw current path")]
        [Description("Enables drawing the current path Honorbuddy is following.")]
        public bool DrawCurrentPath { get; set; }

        #endregion
    }
}
