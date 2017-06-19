using System.Text;

namespace Mrut{
    public class ResponseBody{
        private string textData;

        public byte[] Data { get; private set; }
        public string ContentType { get; private set; }

        public string TextData {
            get {
                if (Data == null) {
                    return string.Empty;
                }

                if (textData == null) {
                    textData = Encoding.UTF8.GetString(Data, 0, Data.Length);
                }

                return textData;
            }
        }

        public ResponseBody(byte[] data, string contentType) {
            this.Data = data;
            ParseContentType(contentType);
        }

        private void ParseContentType(string contentType) {
            if (string.IsNullOrEmpty(contentType)) {
                return;
            }

            string[] spliten = contentType.Split(';');
            if (spliten.Length > 0) {
                ContentType = spliten[0];
            }
        }
    }
}