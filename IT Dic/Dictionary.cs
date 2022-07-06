using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Dic
{
    public class Dictionary
    {
        private int tm_id;
        private string tm_japanese_translate;
        private string tm_japanese_hiragana;
        private string tm_vietnamese_tranlate;
        private string tm_english_tranlate;
        private string tm_example;

        public Dictionary()
        {

        }

        public Dictionary(Dictionary a)
        {
            tm_id = a.tm_id;
            tm_japanese_translate = a.tm_japanese_translate;
            tm_japanese_hiragana = a.tm_japanese_hiragana;
            tm_vietnamese_tranlate = a.tm_vietnamese_tranlate;
            tm_english_tranlate = a.tm_english_tranlate;
            tm_example = a.tm_example;
        }

        public Dictionary(int tm_id, string tm_japanese_translate, string tm_japanese_hiragana, string tm_vietnamese_tranlate, string tm_english_tranlate, string tm_example)
        {
            this.Tm_id = tm_id;
            this.Tm_japanese_translate = tm_japanese_translate;
            this.Tm_japanese_hiragana = tm_japanese_hiragana;
            this.Tm_vietnamese_tranlate = tm_vietnamese_tranlate;
            this.Tm_english_tranlate = tm_english_tranlate;
            this.Tm_example = tm_example;
        }
        public string Tm_japanese_translate { get => tm_japanese_translate; set => tm_japanese_translate = value; }
        public string Tm_japanese_hiragana { get => tm_japanese_hiragana; set => tm_japanese_hiragana = value; }
        public string Tm_vietnamese_tranlate { get => tm_vietnamese_tranlate; set => tm_vietnamese_tranlate = value; }
        public string Tm_english_tranlate { get => tm_english_tranlate; set => tm_english_tranlate = value; }
        public string Tm_example { get => tm_example; set => tm_example = value; }
        public int Tm_id { get => tm_id; set => tm_id = value; }
    }
}
