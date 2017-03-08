namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2Plugin
{
    class PSO2PluginJsonObject
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Homepage { get; set; }
        public string Version { get; set; }
        public string MD5Hash { get; set; }
        public string Toggleable { get; set; }

        public override string ToString()
        {
            return $"ID: {this.ID}\nFilename: {this.Filename}\nDescription: {this.Description}";
        }

        public PSO2Plugin ToPSO2Plugin()
        {
            return new PSO2Plugin(this.ID, this.Name, this.Filename, this.Description, this.Author, this.Homepage, this.Version, this.MD5Hash, this.Toggleable, true);
        }
    }
}
