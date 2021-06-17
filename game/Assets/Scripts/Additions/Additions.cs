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

    // типо доп штуки для полей
    public static class TransformExtension
    {
        // возврат листа всех карт всех дочерних полей
        public static List<Transform> getThreeFieldElements(this Transform field, bool reverse = false)
        {
            List<Transform> elements = new List<Transform>();

            int i = 0, max = field.childCount;
            if (reverse) { i = field.childCount - 1; max = -1; }

            for (; i != max;)
            {
                for (int j = 0; j < field.GetChild(i).childCount; j++)
                {
                    elements.Add(field.GetChild(i).GetChild(j));
                }

                i = reverse ? i - 1 : i + 1;
            }

            return elements;
        }

        public static List<Transform> getFieldElements(this Transform field, bool reverse = false)
        {
            List<Transform> elements = new List<Transform>();

            int i = 0, max = field.childCount;
            if (reverse) { i = field.childCount - 1; max = -1; }

            while (i != max)
            {
                elements.Add(field.GetChild(i));

                i = (reverse ? i - 1 : i + 1);
            }

            return elements;
        }
    }

}
