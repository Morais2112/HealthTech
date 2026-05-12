namespace ClinicaApi.Configuration;

// classe so pra mapear a secao "MongoSettings" do appsettings.json
// (vi na doc da microsoft que esse jeito de tipar a config eh o "options pattern")
public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
