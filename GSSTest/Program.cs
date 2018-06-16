/*
 * 以下参照
 *   Google.Apis,Google.Apis.Auth,Google.Apis.Auth.PlatformServices,
 *   Google.Apis.Core,Google.Apis.PlatformServices,Google.Apis.Sheets.v4
 */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Newtonsoft.Json;

namespace GSSTest
{
    class Program
    {
        private static readonly string CredFileSaveDirectlyName = "google.apis.sheets.v4-Test";

        private static readonly string ApplicationName = "GSS API Test";
        private static readonly string AppClientId = "my-clientId"; // TODO: Update placeholder value.
        private static readonly string AppClientSecret = "my-clientSecret"; // TODO: Update placeholder value.
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

        private static readonly string SpreadSheetId = "my-sheetId";    // TODO: Update placeholder value.
        private static readonly string Range = "my-range";  // TODO: Update placeholder value.

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Spreadsheet API Test");
            Console.WriteLine("================================");

            // APIを呼び出してJSONを取得する
            var originalJson = Task.Run(() => GetJsonAsync());
            Console.WriteLine("Now Loading...");

            // 取得したJSONを加工する
            var formattedJson = GSSFormatter.CreateJson(await originalJson);
            Console.WriteLine($"Original JSON : {await originalJson}");
            Console.WriteLine();
            Console.WriteLine($"Formatted JSON : {formattedJson}");
            Console.WriteLine();

            // 試しに呼び出してみる
            var jsonNode = JsonNode.Parse(formattedJson);
            //Console.WriteLine($"浅見 リリスのバストサイズ：{jsonNode["param"][0]["バスト"].Get<Int64>()}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// APIを呼び出してJSONを取得する
        /// </summary>
        private static async Task<string> GetJsonAsync()
        {
            // Oauth認証情報のキャッシュするためのディレクトリパスを指定する
            string credentialSaveToPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            credentialSaveToPath = Path.Combine(credentialSaveToPath, ".credentials/" + CredFileSaveDirectlyName);
            Console.WriteLine($"Credential file saved to : {credentialSaveToPath}");

            // 認証を実行
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
              new ClientSecrets
              {
                  ClientId = AppClientId,
                  ClientSecret = AppClientSecret
              },
              Scopes,
              "user",
              CancellationToken.None,
              new FileDataStore(credentialSaveToPath)
            );

            Console.WriteLine("Credential is OK");

            // 認証結果を使ってスプレッドシートサービスのインスタンスを作成する
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Requestを投げてセル値のデータを受け取る
            var request = service.Spreadsheets.Values.Get(SpreadSheetId, Range);
            var response = await request.ExecuteAsync();

            Console.WriteLine("Response is OK");

            return JsonConvert.SerializeObject(response, Formatting.Indented);
        }
    }
}