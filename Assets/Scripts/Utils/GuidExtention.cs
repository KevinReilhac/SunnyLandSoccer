using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GuidExtention
{
	public static Guid ToGuid(this string id)
	{
		MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

		byte[] inputBytes = Encoding.Default.GetBytes(id);
		byte[] hashBytes = provider.ComputeHash(inputBytes);

		return (new Guid(hashBytes));
	}
}
