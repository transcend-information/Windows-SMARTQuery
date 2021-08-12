using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMARTQuery
{
    class UtilityClass
    {
        public static string getModelName(byte[] deiveInfoArray, bool isNVMe)
        {
            string Model_number = "";
            if (!isNVMe)
            {
                for (int i = 0; i < 20; i++)    //Word 27~46
                {
                    Model_number += Encoding.Default.GetString(deiveInfoArray, (27 * 2) + (i * 2 + 1), 1) + Encoding.Default.GetString(deiveInfoArray, (27 * 2) + (i * 2), 1);
                }
            }
            else
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                for (int i = 24; i <= 63; i++)
                {
                    if (!deiveInfoArray[i].Equals(0))
                        Model_number += ascii.GetString(deiveInfoArray, i, 1);
                    else
                        break;
                }
            }
            Model_number = Model_number.Trim();
            return Model_number;
        }
    }
}
