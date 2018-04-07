using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace EnglishTrainPro.cs
{
    class YahooDictionaryFactory : WebDictionaryFactory
    {
        public YahooDictionaryFactory()
        {
            Type = DictionaryType.Yahoo;
        }
        protected override string GetWordURL(string wordStr)
        {
            return $@"https://tw.dictionary.search.yahoo.com/search?p={wordStr}&fr2=dict";
        }
        protected override Word GetWordByHtml(Document htmlDoc, string wordStr)
        {
            Word word = null;
            try
            {
                var allBlock = htmlDoc.GetElementsByTag("ol").First(x => x.Attr("class") == "mb-15 reg searchCenterMiddle");
                var meaningBlock = allBlock.GetElementsByTag("div").First(x => x.Attr("class") == "grp grp-tab-content-explanation tabsContent tab-content-explanation tabActived");

                var phonetic = htmlDoc.GetElementsByTag("div").First(x => x.Attr("class") == "compList ml-25 d-ib").Text();
                var phonetics = phonetic.Replace('ˋ', '`');

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
                            var index = sentence.LastIndexOf('.');
                            var engSentence = sentence.Substring(0, index + 1);
                            var chiSentence = sentence.Substring(index + 2);
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
                word = new YahooWord(wordStr, sentencesByPos, phonetics);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error : {e.Message}");
            }
            return word;
        }
    }
}
