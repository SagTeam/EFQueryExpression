using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    internal static class TypeConverter
    {

        static string[] _boolTrueValueStrings = new string[] { "True", "Yes", "是" };

        /// <summary>
        /// 将查询值转换至列定义的基本类型,成功则输出转换后的值，失败则输出原值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destType">期望的目标类型,通常是被查询属性的基本数据类型,</param>
        /// <returns>成功：返回True,失败:返回False</returns>
        /// <exception cref="InvalidCastException"/>
        public static bool TryConvertSingleValueType(object value, Type destType, out object outValue)
        {
            outValue = value;

            if (value == null)            //空值不需要处理
                return true;
            if (destType == null)            //目标类型错误
                return false;


            Type orgType = value.GetType();

            //是否类型相同
            if ((orgType == destType)//类型相同，//若是Nullable<T>,取其基础类型,//查询值的类型与字段值类型是否相同
                || (destType.IsNullableType(out var destUnderlyingType) && destUnderlyingType == orgType)
                || (orgType.IsNullableType(out var orgUnderlyingType) && destUnderlyingType == orgUnderlyingType))
                return true;
            //throw new Exception(string.Format(MsgStrings.TypeCannotNull, MsgStrings.DestTypeForConvert));
            dynamic tmpVal;
            try
            {
                //字符串转布尔型,若转换失败则=false
                if (destType == Constants.Static_TypeOfBool && orgType == Constants.Static_TypeOfString)
                {
                    tmpVal = false;
                    var strValue = ((string)value).Trim();
                    var valueSize = strValue.Length;
                    if (valueSize == 1 && strValue[0] == 1)     //比使用数字测方式快10倍
                    {
                        tmpVal = true;                          //=1的数字视作true,其它数字视为false
                    }
                    else
                    {
                        for (var i = 0; i < _boolTrueValueStrings.Length; i++)
                        {
                            var str = _boolTrueValueStrings[i];
                            if (valueSize == str.Length)
                            {
                                if (string.CompareOrdinal(str, strValue) == 0
                                    || (string.Compare(str, strValue, StringComparison.OrdinalIgnoreCase) == 0))
                                {
                                    tmpVal = true;
                                    break;
                                }
                            }
                        }
                    }
                }               
                else 
                //日期型转换
                if (destType == Constants.Static_TypeOfDateTime)
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime outDate))
                        tmpVal = outDate;
                    else
                        return false;
                }
                else
                {
                    if (!(value is IConvertible))            //不支持转换
                        return false;
                    if (destType.IsNullableType(out destType))
                        tmpVal = Convert.ChangeType(value, destType);
                    else
#if DEBUG
                    throw new InvalidCastException(MsgStrings.InvalidTypeConvert(orgType, destType));
#endif
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                //throw new Exception(MsgStrings.InvalidTypeConvert + "\n\r" + ex.Message);
            }
            outValue = tmpVal;
            return true;
        }

        public static bool TryConvertItemValuesType(ICollection orgList, Type destType, out ICollection outArray)
        {
            if (orgList == null)
            { outArray = null; return false; }
            if (destType == null)
            { outArray = null; return false; }
            if (orgList.GetType() == destType)
            { outArray = orgList; return true; }
            //校检并重构数组
            var destElemType = destType.GetCollectionElementType();
            var newListType = typeof(List<>).MakeGenericType(destElemType);         //新集合的类型
            var newList = Activator.CreateInstance(newListType);                //新集合实例
            var method = newListType.GetMethod("Add", new[] { destElemType });
            foreach (var v in orgList)
            {
                if (TryConvertSingleValueType(v, destElemType, out dynamic nv))
                    method.Invoke(newList, new[] { nv });
                else//无法转换的值丢掉？
                {
                    throw new InvalidCastException();
                }
            }
            outArray = ((ICollection)newList);
            return true;

        }
        public static bool TryConvertItemValuesType(IEnumerable orgList, Type destType, out IEnumerable outArray)
        {
            outArray = null;
            if (orgList == null || destType == null)
                return false;
            if (orgList.GetType() == destType)
            { outArray = orgList; return true; }
            //校检并重构数组
            var destElemType = destType.GetCollectionElementType();
            if (!(destElemType is IConvertible))
                throw new Exception();

            var arr = Array.CreateInstance(destElemType, 10);
            var count = 0;
            var enumerator = orgList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //System.ComponentModel.TypeConverter.
                if (TryConvertSingleValueType(enumerator.Current, destElemType, out object nv))
                {
                    if (count >= arr.GetLength(0))
                    {
                        var resize = Array.CreateInstance(destElemType, count * 2);
                        Array.Copy(arr, resize, count);
                        arr = resize;
                    }
                    arr.SetValue(nv, count);
                    count++;
                }
                else
                {
                    return false;
                }
            }
            var result = Array.CreateInstance(destElemType, count);
            Array.Copy(arr, result, count);
            outArray = result;
            return true;

        }
    }
}
