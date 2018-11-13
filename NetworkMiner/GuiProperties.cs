using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Reflection;

using System.Xml.Serialization;


namespace NetworkMiner {

    [Obfuscation(Feature = "internalization", Exclude = true)]
    public class GuiProperties {

        private static readonly TimeZoneInfo DEFAULT_TIME_ZONE = TimeZoneInfo.Utc;//Was "Local" before versino 2.3

        public event PropertyChangedEventHandler PropertyChanged;

        private bool automaticallyResizeColumnsWhenParsingComplete = true;
        private ushort columnAutoResizeMaxWidth = 300;
        private int maxDisplayedFrames = 1000;

        private bool useHostsTab = true;
        private bool useBrowsersTab = true;
        private bool useFilesTab = true;
        private bool useImagesTab = true;
        private bool useMessagesTab = true;
        private bool useCredentialsTab = true;
        private bool useSessionsTab = true;
        private bool useDnsTab = true;
        private bool useParametersTab = true;
        private bool useKeywordsTab = true;
        private bool useCleartextTab = false;
        private bool useFramesTab = false;
        private bool useAnomaliesTab = true;
        private bool useAudioTab = false;

        private bool preserveLinesInCsvExport = true;
        //private bool showPartialDownloads = false;
        private TimeZoneInfo timestampDisplayTimeZone = DEFAULT_TIME_ZONE;

        private System.Drawing.Color advertismentColor, internetTrackerColor;

        //private System.Security.Cryptography.ICryptoTransform cryptoTransform = null;
        private System.Security.Cryptography.SymmetricAlgorithm cryptoAlg = null;

        public void SetCryptoAlgoritm(System.Security.Cryptography.SymmetricAlgorithm cryptoAlg) {
            this.cryptoAlg = cryptoAlg;
        }

        /*
        public void SetCryptoTransform(System.Security.Cryptography.ICryptoTransform cryptoTransform) {
            this.cryptoTransform = cryptoTransform;
        }
        */


        public GuiProperties() {
            this.advertismentColor = System.Drawing.Color.Red;
            this.internetTrackerColor = System.Drawing.Color.Blue;
        }

        public GuiProperties Clone() {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
                serializer.Serialize(stream, this);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                GuiProperties clone = serializer.Deserialize(stream) as GuiProperties;
                clone.cryptoAlg = this.cryptoAlg;
                return clone;
            }
        }

        public bool AutomaticallyResizeColumnsWhenParsingComplete {
            get {
                return automaticallyResizeColumnsWhenParsingComplete;
            }
            set {
                automaticallyResizeColumnsWhenParsingComplete = value;
            }
        }

        [DescriptionAttribute("Maximum width of columns when doing auto-resize to fit content. 0 = Unlimited (always fit content).")]
        public ushort ColumnAutoResizeMaxWidth {
            get { return this.columnAutoResizeMaxWidth; }
            set { this.columnAutoResizeMaxWidth = value; }
        }

        [DescriptionAttribute("-1 = No limit\n0 = Don't show any frames\n1000 = Default limitation")]
        public int MaxDisplayedFrames {
            get { return this.maxDisplayedFrames; }
            set {
                if (value < 0)
                    this.maxDisplayedFrames = -1;
                else
                    this.maxDisplayedFrames = value;
            }
        }


        [XmlIgnore]
        public System.Drawing.Color AdvertisementColor {
            get { return this.advertismentColor; }
            set { this.advertismentColor = value; }
        }
        [Browsable(false)]
        public string AdvertismentColorHtml {
            get { return System.Drawing.ColorTranslator.ToHtml(this.AdvertisementColor); }
            set { this.AdvertisementColor = System.Drawing.ColorTranslator.FromHtml(value); }
        }

        [XmlIgnore]
        public System.Drawing.Color InternetTrackerColor {
            get { return this.internetTrackerColor; }
            set { this.internetTrackerColor = value; }
        }
        [Browsable(false)]
        public string InternetTrackerColorHtml {
            get { return System.Drawing.ColorTranslator.ToHtml(this.InternetTrackerColor); }
            set { this.InternetTrackerColor = System.Drawing.ColorTranslator.FromHtml(value); }
        }

        public bool UseVoipTab {
            get { return this.useHostsTab; }
            set { this.useAudioTab = value; }
        }
        public bool UseHostsTab {
            get { return this.useHostsTab; }
            set { this.useHostsTab = value; }
        }
        public bool UseBrowsersTab {
            get {
                return this.useBrowsersTab;
            }
            set { this.useBrowsersTab = value; }
        }
        public bool UseFilesTab {
            get {
                return this.useFilesTab;
            }
            set { this.useFilesTab = value; }
        }
        public bool UseImagesTab {
            get {
                return this.useImagesTab;
            }
            set { this.useImagesTab = value; }
        }
        public bool UseMessagesTab {
            get {
                return this.useMessagesTab;
            }
            set { this.useMessagesTab = value; }
        }
        public bool UseCredentialsTab {
            get {
                return this.useCredentialsTab;
            }
            set { this.useCredentialsTab = value; }
        }
        public bool UseSessionsTab {
            get {
                return this.useSessionsTab;
            }
            set { this.useSessionsTab = value; }
        }
        public bool UseDnsTab {
            get {
                return this.useDnsTab;
            }
            set { this.useDnsTab = value; }
        }
        public bool UseParametersTab {
            get {
                return this.useParametersTab;
            }
            set { this.useParametersTab = value; }
        }
        public bool UseKeywordsTab {
            get {
                return this.useKeywordsTab;
            }
            set { this.useKeywordsTab = value; }
        }
        public bool UseCleartextTab {
            get {
                return this.useCleartextTab;
            }
            set { this.useCleartextTab = value; }
        }
        public bool UseFramesTab {
            get {
                return this.useFramesTab;
            }
            set { this.useFramesTab = value; }
        }
        public bool UseAnomaliesTab {
            get {
                return this.useAnomaliesTab;
            }
            set { this.useAnomaliesTab = value; }
        }

        public bool? CheckForUpdatesOnStartup { get; set; } = null;


        [DescriptionAttribute("If newline characters in strings should be preserved when exporting to a CSV file.")]
        public bool PreserveLinesInCsvExport {
            get {
                return preserveLinesInCsvExport;
            }

            set {
                preserveLinesInCsvExport = value;
            }
        }

        [XmlIgnore]
        [DescriptionAttribute("Time zone to use for displaying all timestamp values.")]
        public TimeZoneInfo TimeZone {
            get { return this.timestampDisplayTimeZone; }
            set { this.timestampDisplayTimeZone = value; }
        }
        [XmlIgnore]
        [DescriptionAttribute("Time zone offset (hh:mm:ss) from UTC to use for displaying all timestamp values.")]
        public TimeSpan TimeZoneOffset {
            get { return this.timestampDisplayTimeZone.BaseUtcOffset; }
            set {
                try {
                    this.TimestampDisplayTimeZone = TimeZoneInfo.CreateCustomTimeZone(value.ToString(), value, value.ToString(), value.ToString()).ToSerializedString();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeZoneOffset"));
                }
                catch (System.NotImplementedException e) {
                    //We can end up here when using Mono
                    PacketParser.Utils.Logger.Log("Cannot set TimeZoneOffset: " + e.GetType().ToString(), System.Diagnostics.EventLogEntryType.Error);
                }
            }
        }
        [XmlIgnore]
        [DescriptionAttribute("Time Zone")]
        public DateTimeKind TimeZoneSelection {
            get {
                if (this.timestampDisplayTimeZone == TimeZoneInfo.Local)
                    return DateTimeKind.Local;
                else if (this.timestampDisplayTimeZone == TimeZoneInfo.Utc)
                    return DateTimeKind.Utc;
                else
                    return DateTimeKind.Unspecified;
            }
            set {
                if (value == DateTimeKind.Local)
                    this.timestampDisplayTimeZone = TimeZoneInfo.Local;
                else if (value == DateTimeKind.Utc)
                    this.timestampDisplayTimeZone = TimeZoneInfo.Utc;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeZoneOffset"));
            }
        }

        [Browsable(false)]
        public string TimestampDisplayTimeZone {
            get {
                try {
                    return this.timestampDisplayTimeZone.ToSerializedString();
                }
                catch (NotImplementedException e) {
                    if (PacketParser.Utils.SystemHelper.IsRunningOnMono()) {
                        PacketParser.Utils.Logger.Log("Error serializing TimestampDisplayTimeZone in Mono : " + e.GetType().ToString(), System.Diagnostics.EventLogEntryType.Error);
                        return null;
                    }
                    else
                        throw e;
                }
            }
            set {
                try {
                    TimeZoneInfo customTimeZone = TimeZoneInfo.FromSerializedString(value);
                    if (TimeZoneInfo.Local.BaseUtcOffset == customTimeZone.BaseUtcOffset) {
                        this.timestampDisplayTimeZone = TimeZoneInfo.Local;
                        return;
                    }
                    else if (TimeZoneInfo.Utc.BaseUtcOffset == customTimeZone.BaseUtcOffset) {
                        this.timestampDisplayTimeZone = TimeZoneInfo.Utc;
                        return;
                    }
                    foreach (TimeZoneInfo tzi in TimeZoneInfo.GetSystemTimeZones())
                        if (tzi.BaseUtcOffset == customTimeZone.BaseUtcOffset) {
                            this.timestampDisplayTimeZone = tzi;
                            return;
                        }
                    this.timestampDisplayTimeZone = customTimeZone;
                }
                catch (Exception e) {
                    PacketParser.Utils.Logger.Log("Cannot set TimestampDisplayTimeZone: " + e.GetType().ToString(), System.Diagnostics.EventLogEntryType.Error);
                    this.timestampDisplayTimeZone = DEFAULT_TIME_ZONE;
                }
            }
        }

        [XmlIgnore]
        [DescriptionAttribute("Hybrid-Analysis API Key. Instructions for creating a free API key can be found on https://team.vxstream-sandbox.com/")]
        public string HybridAnalysisApiKey {
            get {
                try {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(this.HybridAnalysisApiKeyEncrypted)) {
                        byte[] iv = new byte[this.cryptoAlg.IV.Length];
                        ms.Read(iv, 0, iv.Length);
                        this.cryptoAlg.IV = iv;
                        using (System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(ms, this.cryptoAlg.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Read)) {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(cryptoStream))
                                return sr.ReadToEnd();
                        }
                    }
                }
                catch {
                    return "";
                }

            }
            set {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                    this.cryptoAlg.GenerateIV();
                    ms.Write(this.cryptoAlg.IV, 0, this.cryptoAlg.IV.Length);
                    using (System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(ms, this.cryptoAlg.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)) {//add IV?
                        //Encoding.UTF8.GetBytes(value ?? "")
                        byte[] data = Encoding.UTF8.GetBytes(value ?? "");
                        cryptoStream.Write(data, 0, data.Length);
                        //ms now has the encrypted data
                        cryptoStream.FlushFinalBlock();
                        ms.Position = 0;
                        this.HybridAnalysisApiKeyEncrypted = ms.ToArray();
                    }

                }
            }
        }
        [Browsable(false)]
        public byte[] HybridAnalysisApiKeyEncrypted { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        [DescriptionAttribute("Hybrid-Analysis API Secret.")]
        public string HybridAnalysisApiSecret {
            get {
                try {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(this.HybridAnalysisApiSecretEncrypted)) {
                        byte[] iv = new byte[this.cryptoAlg.IV.Length];
                        ms.Read(iv, 0, iv.Length);
                        this.cryptoAlg.IV = iv;
                        using (System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(ms, this.cryptoAlg.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Read)) {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(cryptoStream))
                                return sr.ReadToEnd();
                        }
                    }
                }
                catch {
                    return "";
                }

            }
            set {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                    this.cryptoAlg.GenerateIV();
                    ms.Write(this.cryptoAlg.IV, 0, this.cryptoAlg.IV.Length);
                    using (System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(ms, this.cryptoAlg.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)) {//add IV?
                        //Encoding.UTF8.GetBytes(value ?? "")
                        byte[] data = Encoding.UTF8.GetBytes(value ?? "");
                        cryptoStream.Write(data, 0, data.Length);
                        //ms now has the encrypted data
                        cryptoStream.FlushFinalBlock();
                        ms.Position = 0;
                        this.HybridAnalysisApiSecretEncrypted = ms.ToArray();
                    }

                }
            }
        }
        [Browsable(false)]
        public byte[] HybridAnalysisApiSecretEncrypted { get; set; }

        public string ToCustomTimeZoneString(DateTime timestamp) {
            return ToTimeZoneString(timestamp, this.timestampDisplayTimeZone);
        }

        public static string ToDefaultTimeZoneString(DateTime timestamp) {
            return ToTimeZoneString(timestamp, DEFAULT_TIME_ZONE);
        }
        private static string ToTimeZoneString(DateTime timestamp, TimeZoneInfo timeZoneInfo) {

            DateTime timeInCustomZone = TimeZoneInfo.ConvertTimeFromUtc(timestamp.ToUniversalTime(), timeZoneInfo);

            TimeSpan timeZoneOffset = timeZoneInfo.GetUtcOffset(timeInCustomZone);
            StringBuilder s = new StringBuilder(timeInCustomZone.ToString("yyyy-MM-dd HH:mm:ss"));
            if (timeZoneOffset == TimeSpan.FromSeconds(0))
                s.Append(" UTC");
            else if (timeZoneOffset > TimeSpan.FromSeconds(0))
                s.Append(" UTC+" + timeZoneOffset.ToString("hh"));
            else
                s.Append(" UTC-" + timeZoneOffset.ToString("hh"));

            return s.ToString();
        }

    }
}
