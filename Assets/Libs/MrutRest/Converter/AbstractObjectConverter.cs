namespace Mrut.Converter{
    public abstract class AbstractObjectConverter{
        public abstract T ToObject<T>(string data);
        public abstract string ToString(object obj);

        public byte[] ToRawData(object obj) {
            string stringData = ToString(obj);
            return System.Text.Encoding.UTF8.GetBytes(stringData);
        }
    }
}