using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnglishTrainPro.IO
{
    static class Serialization
    {
        public static void SaveObject<T>(string path, T stuff)
        {
            using (FileStream oFileStream = new FileStream(path, FileMode.Create))
            {
                //建立二進位格式化物件
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                //將物件進行二進位格式序列化，並且將之儲存成檔案
                myBinaryFormatter.Serialize(oFileStream, stuff);
                oFileStream.Flush();
                oFileStream.Close();
                oFileStream.Dispose();
            }
        }
        public static T LoadObject<T>(string path)
        {
            T obj = default(T);
            try
            {
                using (FileStream oFileStream = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    obj = (T)myBinaryFormatter.Deserialize(oFileStream);
                }
            }
            catch (Exception) { }
            return obj;
        }
    }
}
