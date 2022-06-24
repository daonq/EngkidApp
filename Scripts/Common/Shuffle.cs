using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UniRx;
namespace common{
public class Shuffle : MonoBehaviour {
	public static int[] change(int[] a)
	{
		int[] result = new int[a.Length];
		for (int i = a.Length-1; i > 0; i--)
		{
			int rnd = Random.Range(0,i);
			int temp = a[i];
			a[i] = a[rnd];
			a[rnd] = temp;
		}
		for (int i = 0; i < a.Length; i++)
		{
			result [i] = a [i];
		}
		return result;
	}
    public static int[] createList(int N)
    {
        int[] list = new int[N];
        for (int i = 0; i < N; i++)
        {
            list[i] = i;
        }
        int[] newRandowm = change(list);
        return newRandowm;
    }
    public static int[] choinRD(bool isRandom,int N)
    {
         return isRandom?createList(N):createNone(N);
    }
    public static int[] createNone(int N)
     {
            int[] list = new int[N];
            for (int i = 0; i < N; i++)
            {
                list[i] = i;
            }
            return list;
     }
     public static List<int> choinOneFromArry(string wordname, string[] words)
     {
            List<int> choin = new List<int>();
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] != wordname)
                {
                    choin.Add(i);
                }
            }
        return choin;
     }
     public static bool randomBoolean()
     {
        return (Random.value >= 0.5)?true:false;
    }
    public static int choinOne(int abc)
    {
       int [] choinX = createList(abc);
       int choin = choinX[Random.Range(0,2)];
       return choin;
    }
    public static  bool[] DisBool(bool[] isBools)
     {
        bool[] nguocs = new bool[isBools.Length];
        for (int i = 0; i < isBools.Length; i++)
        {
                nguocs[i] = !isBools[i];
        }
            return nguocs;
     }
    public static int findIdbyContent(string wordname, string[] words)
     {
            int choin=0;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == wordname)
                {
                    choin = i;
                }
            }
            return choin;
     }
     public static int[] makeNumberChoin(int N)
     {
            int[] choin= new int[N];
            for (int i = 0; i <N; i++)
            {
                choin[i] = Random.Range(1, 100);
                string checkchucDonVi = "" + choin[i];
                if (checkchucDonVi.Length == 2)
                {
                    if ((int) checkchucDonVi[1] == (int) checkchucDonVi[0])
                    {
                        choin[i] = Random.Range(89, 98);
                    }
                }
            }
           return choin;
     }
    public static bool[] makeBoolChoin(int N)
    {
        bool[] choin = new bool[N];
        for (int i = 0; i < N; i++)
        {
            choin[i] = randomBoolean();
        }
        return choin;
    }
    private string makeSpace(int StrSpace)
    {
        string str = "";
        for (int i = 0; i < StrSpace; i++)
        {
            str += " ___ ";
        }
        return str;
    }
    public static string[] ShuffleString(string[] a)
    {
        string[] result = new string[a.Length];
        for (int i = a.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);
            string temp = a[i];
            a[i] = a[rnd];
            a[rnd] = temp;
        }
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i];
        }
        return result;

    }
    public static int[] findIdbyInt(int N, int[] ints)
    {
        int[] choin=new int[ints.Length-1];
        List<int> other = new List<int>();
        for (int i = 0; i<ints.Length; i++)
        {
            if (ints[i] != N)
            {
                other.Add(ints[i]);
            }
        }
        for (int j = 0; j < other.Count; j++)
        {
            choin[j] = other[j];
        }
        return choin;
    }
    public static int[] ShuffleGame(int[] a)
    {
        for (int i = a.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);
            int temp = a[i];
            a[i] = a[rnd];
            a[rnd] = temp;
        }
        return a;
    }
    public static RectTransform[] shuffleRect(RectTransform[] games)
    {
        RectTransform[] gamesOthers= new RectTransform[games.Length];
        int[] lists = createList(games.Length);
        int[] choinLists = ShuffleGame(lists);
        for (int i = 0; i < games.Length; i++)
        {
            gamesOthers[i] = games[choinLists[i]];
        }
        return gamesOthers;
    }
    public static Image[] shuffleImage(Image[] games)
    {
        Image[] gamesOthers = new Image[games.Length];
        int[] lists = createList(games.Length);
        int[] choinLists = ShuffleGame(lists);
        for (int i = 0; i < games.Length; i++)
        {
            gamesOthers[i] = games[choinLists[i]];
        }
        return gamesOthers;
    }
    public static GameObject[] shuffleGameObject(GameObject[] games)
    {
        GameObject[] gamesOthers = new GameObject[games.Length];
        int[] lists = createList(games.Length);
        int[] choinLists = ShuffleGame(lists);
        for (int i = 0; i < games.Length; i++)
        {
            gamesOthers[i] = games[choinLists[i]];
        }
        return gamesOthers;
    }
    public static int[] choinXinArray(int N, int X, int[] a)
    {
        int[] contentchoin = ShuffleGame(findIdbyInt(X, a));
        List<int> other = new List<int>();
        for(int i = 0; i < N-1; i++)
        {
            other.Add(contentchoin[i]);
        }
        other.Add(X);
        return ShuffleGame(other.ToArray());
    }
    public static List<GameObject> GetAllChilds(GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }
    public static GameObject[] FindThreeItem(GameObject gameCurrent,GameObject[] gameItems)
    {
        GameObject choinYes = gameItems.ToArray().Where(a => a.name == gameCurrent.name).ToArray()[0];
        GameObject[] choinNo = gameItems.ToArray().Where(a => a.name != gameCurrent.name).ToArray();
        GameObject[] choinNoRD = shuffleGameObject(choinNo);
        GameObject[] choins = new GameObject[3] { choinYes, choinNoRD[0], choinNoRD[1] };
        return shuffleGameObject(choins);
    }
    public static string[] paths(string link)
    {
        string[] paths = link.Split(new string[] { "/" }, System.StringSplitOptions.None);
        string filename = paths[paths.Length - 1];
        int end = link.LastIndexOf("/" + filename);
        return new string[2] { paths[paths.Length - 1],link.Substring(0,end)+"/"};
    }
    }
}