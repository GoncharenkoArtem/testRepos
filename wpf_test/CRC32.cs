using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_test
{
    public class CRC_32
    {
        private readonly string _filePath = null!;
        private readonly uint[] Crc32Table;
        //public string SontrolSum;

        public CRC_32(string filePath) 
        {
            _filePath = filePath;
            Crc32Table = CreateCrc32Table();
         }

        private static uint[] CreateCrc32Table()
        {
            uint[] table = new uint[256];
            const uint polynomial = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (uint j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = (crc >> 1) ^ polynomial;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
                table[i] = crc;
            }
            return table;
        }


        private string ComputeCrc32()
        {
            using (FileStream stream = File.OpenRead(_filePath))
            {
                uint crcValue = 0xFFFFFFFF;
                int byteValue;
                while ((byteValue = stream.ReadByte()) != -1)
                {
                    byte tableIndex = (byte)(((crcValue) & 0xff) ^ byteValue);
                    crcValue = Crc32Table[tableIndex] ^ ((crcValue >> 8) & 0x00FFFFFF);
                }
                crcValue = ~crcValue;
                return crcValue.ToString("X8");
            }
        }


        public string Get()
        {
            if (File.Exists(_filePath))
            {
                return ComputeCrc32();
            }
            else
            {
                return "-";
            }
        }

    }
}
