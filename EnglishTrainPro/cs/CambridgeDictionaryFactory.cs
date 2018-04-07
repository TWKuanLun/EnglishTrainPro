using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EnglishTrainPro.cs
{
    class CambridgeDictionaryFactory : WebDictionaryFactory
    {
        public CambridgeDictionaryFactory()
        {
            Type = DictionaryType.Cambridge;
        }
        protected override string GetWordURL(string wordStr)
        {
            wordStr = wordStr.ToLower();
            return $@"https://dictionary.cambridge.org/zht/%E8%A9%9E%E5%85%B8/%E8%8B%B1%E8%AA%9E-%E6%BC%A2%E8%AA%9E-%E7%B9%81%E9%AB%94/{wordStr}";
        }
        protected override Word GetWordByHtml(Document htmlDoc, string wordStr)
        {
            Word word = null;
            try
            {
                var meaningsByPos = new Dictionary<string, List<string>>();
                var sentencesByPos = new Dictionary<string, Dictionary<string, List<Sentence>>>();
                var POS_Blocks = htmlDoc.GetElementsByTag("div").Where(x => x.Attr("class") == "entry-body__el clrd js-share-holder").ToArray();
                HashSet<string> phonetics = new HashSet<string>();
                foreach (var POS_Block in POS_Blocks)
                {
                    var pos = POS_Block.GetElementsByTag("div").FirstOrDefault(x => x.Attr("class") == "pos-header").GetElementsByTag("span").FirstOrDefault(x => x.Attr("class") == "pos").Text();
                    if (!meaningsByPos.ContainsKey(pos))
                        meaningsByPos.Add(pos, new List<string>());
                    if (!sentencesByPos.ContainsKey(pos))
                        sentencesByPos.Add(pos, new Dictionary<string, List<Sentence>>());
                    var header = POS_Block.GetElementsByTag("span").Where(x => x.Attr("class") == "pron-info");
                    var phoneticList = new List<string>();
                    foreach (var item in header)
                    {
                        var element = item as NSoup.Nodes.Element;
                        var kind = element.GetElementsByTag("span").FirstOrDefault(x => x.Attr("class") == "region");
                        var phonetic = element.GetElementsByTag("span").FirstOrDefault(x => x.Attr("class") == "pron");
                        phoneticList.Add($"{kind?.Text()}:{phonetic?.Text()}");
                    }
                    var phoneticStr = String.Join(" ", phoneticList);
                    phonetics.Add(phoneticStr);

                    var senseBlocks = POS_Block.GetElementsByTag("div").Where(x => x.Attr("class") == "sense-block").ToArray();
                    foreach (var senseBlock in senseBlocks)
                    {
                        var defBlocks = senseBlock.GetElementsByTag("div").Where(x => x.Attr("class") == "def-block pad-indent");
                        foreach (var defBlock in defBlocks)
                        {
                            var engSense = defBlock.GetElementsByTag("b").FirstOrDefault(x => x.Attr("class") == "def").Text();
                            var chiSense = defBlock.GetElementsByTag("span").FirstOrDefault(x => x.Attr("class") == "trans").Text();
                            var mean = $"{chiSense}\n{engSense}";
                            meaningsByPos[pos].Add(mean);
                            if (!sentencesByPos[pos].ContainsKey(mean))
                                sentencesByPos[pos].Add(mean, new List<Sentence>());
                            var examples = defBlock.GetElementsByTag("div").Where(x => x.Attr("class") == "examp emphasized");
                            foreach (var example in examples)
                            {
                                var meanStr = example.Text();
                                var chiSentence = example.GetElementsByTag("span").FirstOrDefault(x => x.Attr("class") == "trans").Text();
                                var engSentence = meanStr.Substring(0, meanStr.Length - chiSentence.Length - 1);
                                var sentence = new Sentence(chiSentence, engSentence);
                                sentencesByPos[pos][mean].Add(sentence);
                            }
                        }
                    }
                }
                word = new CambridgeWord(wordStr, sentencesByPos, phonetics);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error : {e.Message}");
            }
            return word;
        }
    }
}
