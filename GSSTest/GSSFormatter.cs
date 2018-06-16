/**
Copyright (c) 2018 garagara-jp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace GSSTest
{
    public class GSSFormatter
    {
        public static string CreateJson(string originalJson)
        {
            // 01.APIで取得したJsonをパース
            JsonNode tmpJson = JsonNode.Parse(originalJson)["values"];

            // 02.取得したJsonからKeyの名前を取得
            var keyNameList = new List<string>();
            foreach (var key in tmpJson[0])
            {
                string keyName = key.Get<string>();
                keyNameList.Add(keyName);
            }

            // 03.各オブジェクトのパラメータDictionaryをリストに格納する
            var param = new List<Dictionary<string, object>>();
            for (int i = 1; i < tmpJson.Count; i++)
            {
                // オブジェクトのパラメータを格納するDictionary
                var valueDic = new Dictionary<string, object>();

                foreach (var key in keyNameList)
                {
                    // 取得したJsonからパラメータを取得してDictionaryに格納
                    var num = keyNameList.IndexOf(key);

                    // string型かint型か区別
                    var tmp = tmpJson[i][num].Get<string>();
                    if (int.TryParse(tmp, out int outNum))
                        valueDic.Add(key, outNum);
                    else valueDic.Add(key, tmp);
                }

                // Dictionaryをリストに格納
                param.Add(valueDic);
            }

            // 04.Dictionaryを作成してJSON形式にシリアライズ
            var dic = new Dictionary<string, List<Dictionary<string, object>>>
            {
                { "param", param }
            };
            var json = JsonConvert.SerializeObject(dic, Formatting.Indented);

            return json;
        }
    }
}
