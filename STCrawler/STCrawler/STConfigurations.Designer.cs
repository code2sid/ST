﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace STCrawler {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class STConfigurations : global::System.Configuration.ApplicationSettingsBase {
        
        private static STConfigurations defaultInstance = ((STConfigurations)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new STConfigurations())));
        
        public static STConfigurations Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2,4,6,8,9")]
        public string ScheduledHour {
            get {
                return ((string)(this["ScheduledHour"]));
            }
            set {
                this["ScheduledHour"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("61053682~smile@15")]
        public string ST_UsernamePassword {
            get {
                return ((string)(this["ST_UsernamePassword"]));
            }
            set {
                this["ST_UsernamePassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("R3qPzdrsH5cG9m2Gk0A9ztxvqZzRVZT83vu3/DkvsVwPdxy9hEzVBoZ8g1Y4eeZLCVDkn8P/XLr6aW0fS" +
            "sJZ6lwthCppneCd7/l98CYuAkBI/dv8ZFssWMZkK9L8fOgtwtSxTzO3p5C1299fzYUtAX/cAl1SbJXuT" +
            "MJnYPk+96E=")]
        public string Code {
            get {
                return ((string)(this["Code"]));
            }
            set {
                this["Code"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public string WaitAfterClicks {
            get {
                return ((string)(this["WaitAfterClicks"]));
            }
            set {
                this["WaitAfterClicks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.frenzzup.com/MyTimeLine.aspx#")]
        public string ST_URL {
            get {
                return ((string)(this["ST_URL"]));
            }
            set {
                this["ST_URL"] = value;
            }
        }
    }
}
