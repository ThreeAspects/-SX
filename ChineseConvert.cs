using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChineseConvertTool
{
    public class ChineseConvert
    {
        #region 变量声明

        // XML文件读取实例
        /// <summary>
        /// XML文件读取实例
        /// </summary>
        private static XmlReader _Reader = null;

        // XML文件读取实例
        /// <summary>
        /// XML文件中数据
        /// </summary>
        private static string[] _StrXmlData = null;

        // 记录XML中五笔码开始位置
        /// <summary>
        /// 记录XML中五笔码开始位置
        /// </summary>
        private static int _WBCodeStation = 26;

        // 记录XML中结束位置
        /// <summary>
        /// 记录XML中结束位置
        /// </summary>
        private static int _OutStation = 100;

        #endregion 变量声明

        #region 构造函数

		public ChineseConvert()
        {
            SetData();
        }

        // 初始化XMLREADER
        /// <summary>
        /// 初始化XMLREADER
        /// </summary>
        public static void SetData()
        {
            try
            {
                string strXML = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory+ @"/Areas/SellerAdmin/Common/CodeConfig.xml", Encoding.GetEncoding("GBK"));
                TextReader txtReader = new StringReader(strXML.ToString());

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreComments = true;

                _Reader = XmlReader.Create(txtReader, settings);

                _StrXmlData = GetXmlData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 私有方法

        // 读取XML文件中数据
        /// <summary>
        /// 读取XML文件中数据
        /// </summary>
        /// <returns>返回String[]</returns>
        private static string[] GetXmlData()
        {
            try
            {
                //开辟52个空间，如添加XML节点，可开辟更多的空间
                StringBuilder[] strValue = new StringBuilder[52];
                string[] result = new string[52];
                int i = 0;

                while (_Reader.Read())
                {
                    switch (_Reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (_Reader.Name != "CodeConfig" && _Reader.Name != "SpellCode" && _Reader.Name != "WBCode")
                            {
                                strValue[i] = new StringBuilder();
                                strValue[i].Append(_Reader.Name);
                            }

                            if (_Reader.Name == "WBCode")
                            {
                                _WBCodeStation = i;
                            }

                            break;

                        case XmlNodeType.Text:

                            strValue[i].Append(_Reader.Value);

                            break;

                        case XmlNodeType.EndElement:

                            if (_Reader.Name != "CodeConfig" && _Reader.Name != "SpellCode" && _Reader.Name != "WBCode")
                            {
                                result[i] = strValue[i].ToString();
                                i++;
                            }

                            if (_Reader.Name == "CodeConfig")
                            {
                                _OutStation = i;
                            }

                            break;

                        default:
                            break;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (_Reader != null)
                {
                    _Reader.Close();
                }
            }
        }

        // 查找汉字
        /// <summary>
        /// 查找汉字
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <param name="start">搜索的开始位置</param>
        /// <param name="end">搜索的结束位置</param>
        /// <returns>汉语反义成字符串，该字符串只包含大写的英文字母</returns>
        private static string SearchWord(string strName, int start, int end)
        {
            try
            {
                strName = strName.Trim().Replace(" ", "");

                if (string.IsNullOrEmpty(strName))
                {
                    return strName;
                }

                StringBuilder myStr = new StringBuilder();

                foreach (char vChar in strName)
                {
                    // 若是字母或数字则直接输出
                    if ((vChar >= 'a' && vChar <= 'z') || (vChar >= 'A' && vChar <= 'Z') || (vChar >= '0' && vChar <= '9'))
                    {
                        myStr.Append(char.ToUpper(vChar));
                        continue;
                    }

                    // 若字符Unicode编码在编码范围则 查汉字列表进行转换输出
                    string strList = null;
                    int i;

                    for (i = start; i < end; i++)
                    {
                        strList = _StrXmlData[i];

                        if (strList.IndexOf(vChar) > 0)
                        {
                            myStr.Append(strList[0]);
                            break;
                        }
                    }
                }

                return myStr.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion 私有方法

        #region 公开方法

        // 获得汉语的拼音码
        /// <summary>
        /// 获得汉语的拼音码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <returns>汉语拼音码,该字符串只包含大写的英文字母</returns>
        public string GetSpellCode(string strName)
        {
            try
            {
                return SearchWord(strName, 0, _WBCodeStation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 获得汉语的五笔码
        /// <summary>
        /// 获得汉语的五笔码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <returns>汉语五笔码,该字符串只包含大写的英文字母</returns>
        public string GetWBCode(string strName)
        {
            try
            {
                return SearchWord(strName, _WBCodeStation, _OutStation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion 公开方法
    }
}
