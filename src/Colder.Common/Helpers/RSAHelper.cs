using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Colder.Common.Helpers
{
    /// <summary>
    ///
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="str">原文</param>
        /// <param name="privateKeyXML">xml格式私钥</param>
        /// <returns></returns>
        public static string RSASign(string str, string privateKeyXML)
        {
            //根据需要加签时的哈希算法转化成对应的hash字符节
            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] rgbHash = sha256.ComputeHash(bt);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(privateKeyXML);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("SHA256"); //此处是你需要加签的hash算法，需要和上边你计算的hash值的算法一致，不然会报错。
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="str">原文</param>
        /// <param name="sign">签名</param>
        /// <param name="publicKeyXML">xml格式公钥</param>
        /// <returns></returns>
        public static bool RSASignCheck(string str, string sign, string publicKeyXML)
        {
            try
            {
                byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(str);
                var sha256 = new SHA256CryptoServiceProvider();
                byte[] rgbHash = sha256.ComputeHash(bt);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(publicKeyXML);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("SHA256");
                byte[] rgbSignature = Convert.FromBase64String(sign);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>    
        /// RSA公钥pem转为XML格式
        /// </summary>
        /// <param name="publicKey">pem公钥</param>    
        /// <returns></returns>    
        public static string RSAPublicKeyXML(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>    
        /// RSA私钥pem转为XML格式
        /// </summary>    
        /// <param name="privateKey">pem私钥</param>    
        /// <returns></returns>  
        public static string RSAPrivateKeyXML(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
              Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
              Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }
    }
}
