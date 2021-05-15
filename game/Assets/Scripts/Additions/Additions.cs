using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Additions
{
    public static class Addition
    {
        public static GameObject GetChildWithName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }

        // принимает процент успеха в виде десятичного числа
        public static bool RamdomPersent(double persent)
        {
            return UnityEngine.Random.value > (1 - persent);
        }
    }

    public static class IEnumerableExtension // все методы не мутирующие
    {
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
       
    }

    public static class ListExtension // все методы не мутирующие
    {
        // возвращает новый список, не включая элементы c индексом >= index
        // если такого элемента нет, вернет вест список
        public static List<T> RemoveFrom<T>(this List<T> items, int index)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < items.Count && i < index; i++)
            {
                result.Add(items[i]);
            }

            return result;
        }
    }

}
