using System.Text;
using System.Security.Cryptography;

namespace WebGwenchana.Extension
{
	public static class HashMD5
	{
		public static string GetMD5Hash(string str)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
			byte[] encoded = md5.ComputeHash(bytes);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < encoded.Length; i++)
				sb.Append(encoded[i].ToString("x2"));

			return sb.ToString();
		}

	}
}
