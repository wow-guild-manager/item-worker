using Blizzard.WoWClassic.ApiClient;
using Blizzard.WoWClassic.ApiClient.Helpers;
using System.Threading.Tasks;

namespace ItemTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientWow = new WoWClassicApiClient("bxSvhNNHJwI0kgNvKy6Z91oMEOpwgjmv", "2b136112d3064b11b19c5ea275846996");
            clientWow.SetDefaultValues(RegionHelper.Us, NamespaceHelper.Static, LocaleHelper.French);

            var item = await clientWow.GetItemDetailsAsync(19019, RegionHelper.Us, NamespaceHelper.Static);
        }
    }
}
