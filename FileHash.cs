using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace eflayMH_WPF
{
    public class FileHash
    {
        

        public static byte[] GetFileHash(string filename)
        {
            MD5CryptoServiceProvider md5=new MD5CryptoServiceProvider();

            FileStream fst= new FileStream(filename,FileMode.Open,FileAccess.Read, FileShare.Read, 8192);

            md5.ComputeHash(fst);

            byte[] hash = md5.Hash;
            //string qwer = BitConverter.ToString(hash,0,hash.Length);

            return hash;
        }
        

    }
}
