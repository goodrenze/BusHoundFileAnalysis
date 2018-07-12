using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusHoundFileAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 主命令枚举定义
        public enum eMainCmd : byte
        {
            /* 0x00 */
            IOT_COMM_INIT_CONTEXT = 0x00,
            /* 0x01 */
            IOT_COMM_WRITE_REG,
            /* 0x02 */
            IOT_COMM_READ_REG,
            /* 0x03 */
            IOT_COMM_FIRMWARE_DOWNLOAD,
            /* 0x04 */
            IOT_COMM_GET_VERSION,
            /* 0x05 */
            IOT_COMM_SOFTWARE_RESET,
            /* 0x06 */
            IOT_COMM_WORK_PARAMETER_INQ,
            /* 0x07 */
            IOT_COMM_EPC_PARAM_CFG,
            /* 0x08 */
            IOT_COMM_EPC_PARAM_INQ,
            /* 0x09 */
            IOT_COMM_GB_PARAM_CFG,   // IOT_COMM_GBGJB_PARAM_CFG
                                     /* 0x0A */
            IOT_COMM_GB_PARAM_INQ,   // IOT_COMM_GBGJB_PARAM_INQ
                                     /* 0x0B */
            IOT_COMM_GJB_PARAM_CFG,
            /* 0x0C */
            IOT_COMM_GJB_PARAM_INQ,
            /* 0x0D */
            IOT_COMM_SAVE_PARA_TO_FLASH,

            /* 0x0F */
            IOT_COMM_FREQUENCY_CHANNEL_CFG = 0x0F,
            /* 0x10 */
            IOT_COMM_FHSS_SET,
            /* 0x11 */
            IOT_COMM_POWER_MODE,
            /* 0x12 */
            IOT_COMM_ENCODING_MODE,
            /* 0x13 */
            IOT_COMM_TX_POWER,
            /* 0x14 */
            IOT_COMM_ANTENNA_DETECTION,
            /* 0x15 */
            IOT_COMM_ANTENNA_SWITCH,
            /* 0x16 */
            IOT_COMM_SET_TREXT,
            /* 0x17 */
            IOT_COMM_GET_RETURN_LOSS,

            /* 0x1F */
            IOT_COMM_FLASH_FIRMWARE_DOWNLOAD = 0x1F,

            /* 0x20 */
            IOT_COMM_EPC_INVENTORY = 0x20,
            /* 0x21 */
            IOT_COMM_EPC_INVENTORY_REQ,    // this command may useless.
                                           /* 0x22 */
            IOT_COMM_EPC_STOP_INVENTORY,
            /* 0x23 */
            IOT_COMM_EPC_WRITE,
            /* 0x24 */
            IOT_COMM_EPC_READ,
            /* 0x25 */
            IOT_COMM_EPC_LOCK,
            /* 0x26 */
            IOT_COMM_EPC_ERASE,
            /* 0x27 */
            IOT_COMM_EPC_KILL,
            /* 0x28 */
            IOT_COMM_EPC_CHANGE_PASSWORD,

            /* 0x30 */
            IOT_COMM_GB_INVENTORY = 0x30,
            /* 0x31 */
            IOT_COMM_GB_INVENTORY_REQ,
            /* 0x32 */
            IOT_COMM_GB_STOP_INVENTORY,
            /* 0x33 */
            IOT_COMM_GB_WRITE,
            /* 0x34 */
            IOT_COMM_GB_READ,
            /* 0x35 */
            IOT_COMM_GB_LOCK,
            /* 0x36 */
            IOT_COMM_GB_ERASE,
            /* 0x37 */
            IOT_COMM_GB_KILL,
            /* 0x38 */
            IOT_COMM_GB_CHANGE_PASSWORD,
            /* 0x39 */
            IOT_COMM_GB_LED_CTRL,

            /* 0x40 */
            IOT_COMM_GJB_INVENTORY = 0x40,
            /* 0x41 */
            IOT_COMM_GJB_INVENTORY_REQ,
            /* 0x42 */
            IOT_COMM_GJB_STOP_INVENTORY,
            /* 0x43 */
            IOT_COMM_GJB_WRITE,
            /* 0x44 */
            IOT_COMM_GJB_READ,
            /* 0x45 */
            IOT_COMM_GJB_LOCK,
            /* 0x46 */
            IOT_COMM_GJB_ERASE,
            /* 0x47 */
            IOT_COMM_GJB_KILL,
            /* 0x48 */
            IOT_COMM_GJB_CHANGE_PASSWORD,

            /* 0x60 */
            IOT_COMM_CB_INVENTORY = 0x60,
            /* 0x61 */
            IOT_COMM_CB_INVENTORY_REQ,
            /* 0x62 */
            IOT_COMM_CB_STOP_INVENTORY,
            /* 0x63 */
            IOT_COMM_CB_PARAM_CFG,
            /* 0x64 */
            IOT_COMM_CB_PARAM_INQ,

            /* MAX */
            MAX_NUM_HOST_COMM_ID
        }

        const int COL_NUM = 11;             // Bushound保存的数据文件的最大列数
        const int IDX_DEV = 0;             // Bushound保存的数据文件的Device列索引
        const int IDX_ADDR = 1;             // Bushound保存的数据文件的Address列索引
        const int IDX_LEN = 2;             // Bushound保存的数据文件的Length列索引
        const int IDX_PHA = 3;             // Bushound保存的数据文件的Phase列索引
        const int IDX_DATA = 4;             // Bushound保存的数据文件的Data列索引
        const int IDX_DESC = 5;             // Bushound保存的数据文件的Description列索引
        const int IDX_DELT = 6;             // Bushound保存的数据文件的Delta列索引
        const int IDX_OFS = 7;             // Bushound保存的数据文件的Cmd.Phase.Ofs(rep)列索引
        const int IDX_DATE = 8;             // Bushound保存的数据文件的Date列索引
        const int IDX_TIME = 9;             // Bushound保存的数据文件的Time列索引
        const int IDX_DRV = 10;             // Bushound保存的数据文件的Driver列索引
        // Bushound保存的数据文件的所有列名称，顺序就是以下数组中的顺序
        string[] m_astrColumnsName = { "Device", "Address", "Length", "Phase", "Data", "Description", "Delta", "Cmd.Phase.Ofs(rep)", "Date", "Time", "Driver" };
        struct tColIdxLen
        {
            public int iIdx;
            public int iLen;
        };
        tColIdxLen[] m_asColIdxLen = new tColIdxLen[COL_NUM];

        private void vGetColIdxLen(string p_strLine)
        {
            int i;
            int l_iLastValidIdx = 0;

            for(i = 0; i < COL_NUM; i++)
            {
                // 获取指定字符串对应的索引，如果没有对应字符串，则返回-1
                m_asColIdxLen[i].iIdx = p_strLine.IndexOf(m_astrColumnsName[i]);
                // 如果2个相邻字符串都在，则通过2个索引算出上一列字符串占用的字符个数
                if((-1 != m_asColIdxLen[l_iLastValidIdx].iIdx) && (-1 != m_asColIdxLen[i].iIdx))
                {
                    m_asColIdxLen[l_iLastValidIdx].iLen = m_asColIdxLen[i].iIdx - m_asColIdxLen[l_iLastValidIdx].iIdx;
                }
                if (-1 != m_asColIdxLen[i].iIdx)
                {
                    l_iLastValidIdx = i;
                }
            }
            // 算出最后一列字符串占用的字符个数
            m_asColIdxLen[l_iLastValidIdx].iLen = p_strLine.Length - m_asColIdxLen[l_iLastValidIdx].iIdx;
        }

        private byte[] abyGetHexData(string p_strLine)
        {
            string l_strHex = "";
            byte[] l_abyHexData = null;
            int l_iLen = 0;
            int i, j;

            l_strHex = p_strLine.Replace(" ", "");
            l_iLen = l_strHex.Length % 2;
            if (0 == l_iLen)
            {
                l_iLen = l_strHex.Length / 2;
                l_abyHexData = new byte[l_iLen];
                for(i = 0, j = 0; i < l_iLen; i++, j += 2)
                {
                    l_abyHexData[i] = Convert.ToByte(l_strHex.Substring(j, 2), 16);
                }
            }

            return l_abyHexData;
        }

        // 以下常量定义用于协议数据帧的解析
        public const int FR_SOF = 0x55;
        public const int FR_EOF = 0xAA;

        public const int FR_SOF_INDEX = 0;
        public const int FR_ADDR_INDEX = 1;
        public const int FR_CMD_INDEX = 3;
        public const int FR_LEN_INDEX = 4;
        public const int FR_DATA_INDEX = 6;

        public const int FR_SOF_LEN = 1;
        public const int FR_ADDR_LEN = 2;
        public const int FR_CMD_LEN = 1;
        public const int FR_LEN_LEN = 2;
        public const int FR_CRC_LEN = 2;
        public const int FR_EOF_LEN = 1;
        public const int FR_MIN_LEN = FR_SOF_LEN + FR_ADDR_LEN + FR_CMD_LEN + FR_LEN_LEN + FR_CRC_LEN + FR_EOF_LEN;

        public const int FR_MAX_BUF_LEN = 2048;
        // 协议状态机的枚举定义
        public enum eStatusMachine : int
        {
            SM_SOF = 0,
            SM_ADDR,
            SM_CMD,
            SM_LENGTH,
            SM_DATA,
            SM_CRC,
            SM_EOF
        }
        // 计算1字节校验和
        UInt16 wCheckSum(byte[] p_pbyData, int p_iLength)
        {
            int i;
            UInt16 l_wCheckSum = 0;

            for (i = 0; i < p_iLength; i++)
            {
                l_wCheckSum += p_pbyData[i];
            }

            l_wCheckSum = (UInt16)(~l_wCheckSum + 1);

            return l_wCheckSum;
        }
        uint m_uiTotalTagNum = 0;       // 通过数据解析出来的标签总数
        uint m_uiFwTotalTagNum = 0;     // 固件返回的标签总数
        // 以下静态变量用于协议状态机，用来解析数据
        private static eStatusMachine s_eStatusMachine = eStatusMachine.SM_SOF;
        private static int s_iLen = 0;      // 临时保存数据长度
        private static int s_iDataLen = 0;      // 数据区域的数据长度
        private static int s_iDataIndex = 0;
        private static UInt16 s_wRecvCrcValue = 0;      // 接收到的CRC校验值
        private static UInt16 s_wCalCrcValue = 0;      // 计算出的CRC校验值
        private static byte[] s_abyDataBuf = new byte[FR_MAX_BUF_LEN];
        private void vVerifyFrameData(byte[] p_pbyData, int p_iLineNo)
        {
            int l_iLen = p_pbyData.Length;

            for (int i = 0; i < l_iLen; i++)
            {
                switch (s_eStatusMachine)
                {
                    case eStatusMachine.SM_SOF:
                        if ((byte)FR_SOF == p_pbyData[i])
                        {
                            s_iDataIndex = 0;
                            s_iLen = 0;
                            s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                            s_eStatusMachine = eStatusMachine.SM_ADDR;
                        }
                        else
                        {
                            txtInfo.AppendText(string.Format("第{0}行出现帧头错误。\r\n", p_iLineNo));
                        }
                        break;
                    case eStatusMachine.SM_ADDR:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        s_iLen++;
                        if (FR_ADDR_LEN == s_iLen)
                        {
                            s_iLen = 0;
                            s_eStatusMachine = eStatusMachine.SM_CMD;
                        }
                        break;
                    case eStatusMachine.SM_CMD:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        s_iLen++;
                        if (FR_CMD_LEN == s_iLen)
                        {
                            s_iLen = 0;
                            s_eStatusMachine = eStatusMachine.SM_LENGTH;
                        }
                        break;
                    case eStatusMachine.SM_LENGTH:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        s_iLen++;
                        if (FR_LEN_LEN == s_iLen)
                        {
                            s_iLen = (s_abyDataBuf[s_iDataIndex - 2] << 8) + s_abyDataBuf[s_iDataIndex - 1];
                            s_iDataLen = s_iLen;
                            // 有数据，并且数据长度小于缓存最大容量，转到数据接收流程；没有则接收CRC校验
                            if ((FR_MAX_BUF_LEN - FR_MIN_LEN) < s_iDataLen)
                            {
                                s_iDataLen = s_iLen = 0;
                                s_eStatusMachine = eStatusMachine.SM_SOF;
                                txtInfo.AppendText(string.Format("第{0}行出现缓存溢出错误。\r\n", p_iLineNo));
                            }
                            else if (0 < s_iDataLen)
                            {
                                s_eStatusMachine = eStatusMachine.SM_DATA;
                            }
                            else
                            {
                                s_eStatusMachine = eStatusMachine.SM_CRC;
                            }
                        }
                        break;
                    case eStatusMachine.SM_DATA:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        s_iLen--;
                        if (0 == s_iLen)
                        {
                            s_eStatusMachine = eStatusMachine.SM_CRC;
                        }
                        break;
                    case eStatusMachine.SM_CRC:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        s_iLen++;
                        if (FR_CRC_LEN == s_iLen)
                        {
                            s_iLen = 0;
                            s_wRecvCrcValue = (UInt16)((s_abyDataBuf[s_iDataIndex - 2] << 8) + s_abyDataBuf[s_iDataIndex - 1]);
                            s_wCalCrcValue = wCheckSum(s_abyDataBuf, s_iDataIndex - 2);
                            if (s_wCalCrcValue == s_wRecvCrcValue)
                            {
                                s_eStatusMachine = eStatusMachine.SM_EOF;
                            }
                            else
                            {
                                s_eStatusMachine = eStatusMachine.SM_SOF;
                                txtInfo.AppendText(string.Format("第{0}行出现校验和错误。\r\n", p_iLineNo));
                            }
                        }
                        break;
                    case eStatusMachine.SM_EOF:
                        s_abyDataBuf[s_iDataIndex++] = p_pbyData[i];
                        if ((byte)FR_EOF == s_abyDataBuf[s_iDataIndex - 1])
                        {
                            if((byte)eMainCmd.IOT_COMM_EPC_INVENTORY_REQ == s_abyDataBuf[FR_CMD_INDEX])
                            {
                                s_iLen = (s_abyDataBuf[FR_LEN_INDEX] << 8) + s_abyDataBuf[FR_LEN_INDEX + 1];
                                if(3 == s_iLen)
                                {
                                    m_uiFwTotalTagNum = (uint)((s_abyDataBuf[FR_DATA_INDEX + 1] << 8) + s_abyDataBuf[FR_DATA_INDEX + 2]);
                                    txtInfo.AppendText(string.Format("固件标签总数：{0}；解析出的标签总数：{1}。\r\n", m_uiFwTotalTagNum, m_uiTotalTagNum));
                                }
                                else if (5 == s_iLen)
                                {
                                    m_uiFwTotalTagNum = (uint)((s_abyDataBuf[FR_DATA_INDEX + 1] << 24) + (s_abyDataBuf[FR_DATA_INDEX + 2] << 16) + (s_abyDataBuf[FR_DATA_INDEX + 3] << 8) + s_abyDataBuf[FR_DATA_INDEX + 4]);
                                    txtInfo.AppendText(string.Format("固件标签总数：{0}；解析出的标签总数：{1}。\r\n", m_uiFwTotalTagNum, m_uiTotalTagNum));
                                }
                                else if(3 < s_iLen)
                                {
                                    m_uiTotalTagNum++;
                                }
                            }
                            if ((byte)eMainCmd.IOT_COMM_EPC_STOP_INVENTORY == s_abyDataBuf[FR_CMD_INDEX])
                            {
                                s_iLen = (s_abyDataBuf[FR_LEN_INDEX] << 8) + s_abyDataBuf[FR_LEN_INDEX + 1];
                                if (3 == s_iLen)
                                {
                                    m_uiFwTotalTagNum = (uint)((s_abyDataBuf[FR_DATA_INDEX + 1] << 8) + s_abyDataBuf[FR_DATA_INDEX + 2]);
                                    txtInfo.AppendText(string.Format("固件标签总数：{0}；解析出的标签总数：{1}。\r\n", m_uiFwTotalTagNum, m_uiTotalTagNum));
                                }
                                else if (5 == s_iLen)
                                {
                                    m_uiFwTotalTagNum = (uint)((s_abyDataBuf[FR_DATA_INDEX + 1] << 24) + (s_abyDataBuf[FR_DATA_INDEX + 2] << 16) + (s_abyDataBuf[FR_DATA_INDEX + 3] << 8) + s_abyDataBuf[FR_DATA_INDEX + 4]);
                                    txtInfo.AppendText(string.Format("固件标签总数：{0}；解析出的标签总数：{1}。\r\n", m_uiFwTotalTagNum, m_uiTotalTagNum));
                                }
                            }
                        }
                        else
                        {
                            txtInfo.AppendText(string.Format("第{0}行出现帧尾错误。\r\n", p_iLineNo));
                        }
                        s_eStatusMachine = eStatusMachine.SM_SOF;
                        break;
                    default:
                        s_eStatusMachine = eStatusMachine.SM_SOF;
                        break;
                }
            }
        }

        int m_iLineNo = 0;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog l_ofdOpen = new OpenFileDialog();
                l_ofdOpen.Filter = "txt文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                l_ofdOpen.FilterIndex = 1;
                l_ofdOpen.RestoreDirectory = true;
                byte[] l_abyHexData = null;

                if (l_ofdOpen.ShowDialog() == DialogResult.OK)
                {
                    string l_strModule = string.Empty;
                    string l_strCurLine;
                    string l_strLastLine = "";
                    bool l_bHyphenLine = false;
                    StreamReader l_srReader = new StreamReader(l_ofdOpen.FileName);
                    txtFile.Text = l_ofdOpen.FileName;

                    m_uiTotalTagNum = 0;
                    m_iLineNo = 0;

                    while (null != (l_strCurLine = l_srReader.ReadLine()))
                    {
                        m_iLineNo++;
                        // 找到连字符所在的行，其上一行是对应列的名称，用于确定每一列数据的起始索引和数据长度
                        if (false == l_bHyphenLine)
                        {
                            if(4 > l_strCurLine.Length)
                            {
                                continue;
                            }
                            else if ("----" == l_strCurLine.Substring(0, 4))
                            {
                                l_bHyphenLine = true;
                                vGetColIdxLen(l_strLastLine);
                                continue;
                            }
                            else
                            {
                                l_strLastLine = l_strCurLine;
                            }
                            continue;
                        }

                        l_abyHexData = abyGetHexData(l_strCurLine.Substring(m_asColIdxLen[IDX_DATA].iIdx, m_asColIdxLen[IDX_DATA].iLen));
                        vVerifyFrameData(l_abyHexData, m_iLineNo);
                    }

                    l_srReader.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开文件失败！" + ex.Message);
            }
        }
    }
}
