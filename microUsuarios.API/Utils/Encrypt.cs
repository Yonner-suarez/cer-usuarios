using microUsuarios.API.Dao;
using System.Security.Cryptography;
using System.Text;

namespace microUsuarios.API.Utils
{
    public class Encrypt
    {
        public string Encript(string strPlainText)
        {
            return DAUtil.encriptar(strPlainText);
        }
        public string Decrypt(string encryptedText)
        {
            return DAUtil.desencriptar(encryptedText);
        }
    }
    
}
