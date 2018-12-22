using EnglishTrainPro.DataObject;
using NSoup;
using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace EnglishTrainPro.DataFactory
{
    class YahooDictionaryFactory : WebDictionaryFactory
    {
        public YahooDictionaryFactory()
        {
        }
        protected override string GetDictionaryURL(string wordStr)
        {
            return $@"https://tw.dictionary.search.yahoo.com/search?p={wordStr}&fr2=dict";
        }
        public override WebDictionary GetDictionaryByHtml(string wordStr)
        {
            WebDictionary word = null;
            try
            {
                var htmlStr = GetHtml(GetDictionaryURL(wordStr));
                Document htmlDoc = NSoupClient.Parse(htmlStr);
                var allBlock = htmlDoc.GetElementsByTag("ol").First(x => x.Attr("class") == "mb-15 reg searchCenterMiddle");
                var meaningBlock = allBlock.GetElementsByTag("div").First(x => x.Attr("class") == "grp grp-tab-content-explanation tabsContent tab-content-explanation tabActived");

                var phonetic = htmlDoc.GetElementsByTag("div").First(x => x.Attr("class") == "compList ml-25 d-ib").Text();
                phonetic = phonetic.Replace('ˋ', '`');

                var rows = meaningBlock.GetElementsByTag("li").ToArray();
                var sentencesByPos = new Dictionary<string, Dictionary<string, List<Sentence>>>();
                string partOfSpeech = "";
                for (int i = 0; i < rows.Length; i++)//詞性
                {
                    var rowStr = rows[i].Text();
                    Regex regex = new Regex(@"\d+");
                    Match match = regex.Match(rowStr);
                    if (match.Success)
                    {
                        //中文意思
                        var meaning = rows[i].GetElementsByTag("span").First().Text();
                        if (!sentencesByPos[partOfSpeech].ContainsKey(meaning))
                        {
                            sentencesByPos[partOfSpeech].Add(meaning, new List<Sentence>());
                        }
                        var sentenceElements = rows[i].GetElementsByTag("p");
                        foreach (var sentenceElement in sentenceElements)
                        {
                            var sentence = sentenceElement.Text();
                            int firstChineseIndex = -1;
                            for (int j = 0; j < sentence.Length; j++)
                            {
                                UnicodeCategory cat = char.GetUnicodeCategory(sentence[j]);
                                if (cat == UnicodeCategory.OtherLetter)
                                {
                                    firstChineseIndex = j;
                                    break;
                                }
                            }
                            var engSentence = sentence.Substring(0, firstChineseIndex - 1);
                            var chiSentence = sentence.Substring(firstChineseIndex);
                            sentencesByPos[partOfSpeech][meaning].Add(new Sentence(chiSentence, engSentence));
                        }
                    }
                    else
                    {
                        //詞性
                        partOfSpeech = rowStr;
                        sentencesByPos[partOfSpeech] = new Dictionary<string, List<Sentence>>();
                    }
                }
                word = new YahooDictionary(wordStr, sentencesByPos, phonetic);
            }
            catch (Exception e)
            {
                //MessageBox.Show($"Error : {e.Message}");
            }
            return word;
        }
    }
}
