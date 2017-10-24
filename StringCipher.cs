using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace clearzombies
{
	public static class StringCipher
	{
		private const int Keysize = 256;

		private const int DerivationIterations = 1000;

		public static string Decrypt(string cipherText, string passPhrase)
		{
			string str;
			byte[] numArray = Convert.FromBase64String(cipherText);
			byte[] array = numArray.Take<byte>(32).ToArray<byte>();
			byte[] array1 = numArray.Skip<byte>(32).Take<byte>(32).ToArray<byte>();
			byte[] numArray1 = numArray.Skip<byte>(64).Take<byte>((int)numArray.Length - 64).ToArray<byte>();
			using (Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(passPhrase, array, 1000))
			{
				byte[] bytes = rfc2898DeriveByte.GetBytes(32);
				using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
				{
					rijndaelManaged.BlockSize = 256;
					rijndaelManaged.Mode = CipherMode.CBC;
					rijndaelManaged.Padding = PaddingMode.PKCS7;
					using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes, array1))
					{
						using (MemoryStream memoryStream = new MemoryStream(numArray1))
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
							{
								byte[] numArray2 = new byte[(int)numArray1.Length];
								int num = cryptoStream.Read(numArray2, 0, (int)numArray2.Length);
								memoryStream.Close();
								cryptoStream.Close();
								str = Encoding.UTF8.GetString(numArray2, 0, num);
							}
						}
					}
				}
			}
			return str;
		}

		public static string Encrypt(string plainText, string passPhrase)
		{
			string base64String;
			byte[] numArray = StringCipher.Generate256BitsOfRandomEntropy();
			byte[] numArray1 = StringCipher.Generate256BitsOfRandomEntropy();
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			using (Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(passPhrase, numArray, 1000))
			{
				byte[] bytes1 = rfc2898DeriveByte.GetBytes(32);
				using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
				{
					rijndaelManaged.BlockSize = 256;
					rijndaelManaged.Mode = CipherMode.CBC;
					rijndaelManaged.Padding = PaddingMode.PKCS7;
					using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(bytes1, numArray1))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
							{
								cryptoStream.Write(bytes, 0, (int)bytes.Length);
								cryptoStream.FlushFinalBlock();
								byte[] array = numArray.Concat<byte>(numArray1).ToArray<byte>();
								array = array.Concat<byte>(memoryStream.ToArray()).ToArray<byte>();
								memoryStream.Close();
								cryptoStream.Close();
								base64String = Convert.ToBase64String(array);
							}
						}
					}
				}
			}
			return base64String;
		}

		private static byte[] Generate256BitsOfRandomEntropy()
		{
			byte[] numArray = new byte[32];
			using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rNGCryptoServiceProvider.GetBytes(numArray);
			}
			return numArray;
		}
	}
}