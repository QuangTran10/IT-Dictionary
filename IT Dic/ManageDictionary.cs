using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Dic
{
    public class ManageDictionary
    {
        public string Encode(string data)
        {
            var windows1252 = Encoding.GetEncoding(1252);
            var utf8Bytes = windows1252.GetBytes(data);
            var correct = Encoding.UTF8.GetString(utf8Bytes);
            return correct;
        }
        public string Decode(string data)
        {
            byte[] utf8Text = Encoding.UTF8.GetBytes(data);
            Encoding cp1252 = Encoding.GetEncoding(1252);
            string result = cp1252.GetString(utf8Text);
            return result;
        }
        public ArrayList findWord(MySqlConnection conn, string findwords)
        {
            ArrayList listwords = new ArrayList();
            String sql = "";
            findwords = findwords.TrimStart();
            findwords = Decode(findwords);
            sql = "select * from mst_translate_mean where tm_vietnamese_translate like '%" + findwords + "%'"
                    + "or tm_english_translate like '%" + findwords
                    + "%' or tm_example like '%" + findwords
                    + "%' or tm_japanese_hiragana like '%" + findwords
                    + "%' or tm_japanese_translate like '%" + findwords + "%'"
                    + "order by tm_japanese_translate DESC";

            MySqlCommand command = new MySqlCommand();

            command.Connection = conn;

            command.CommandText = sql;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Dictionary dic = new Dictionary();
                        dic.Tm_id = reader.GetInt32(0);
                        if (reader.IsDBNull(2))
                        {
                            dic.Tm_japanese_translate = "";
                        }
                        else
                        {
                            dic.Tm_japanese_translate = reader.GetString(2);
                        }

                        dic.Tm_japanese_hiragana = reader.GetString(3);
                        dic.Tm_vietnamese_tranlate = reader.GetString(4);

                        if (reader.IsDBNull(5))
                        {
                            dic.Tm_english_tranlate = "";
                        }
                        else
                        {
                            dic.Tm_english_tranlate = reader.GetString(5);
                        }

                        if (reader.IsDBNull(6))
                        {
                            dic.Tm_example = "";
                        }
                        else
                        {
                            dic.Tm_example = reader.GetString(6);
                        }

                        listwords.Add(dic);
                    }
                }
            }

            return listwords;
        }

    }
}
