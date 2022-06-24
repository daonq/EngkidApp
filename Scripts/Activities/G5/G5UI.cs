using UnityEngine;
using UnityEngine.UI;
using UniRx;
using common;
using System.Linq;
/*hhh*/
namespace G5
{
    [System.Serializable]
    public class G5UI
    {
        public Text txtcontent;
        public Transform target;
        public Transform[] DragsDisable;
        public Transform[] Drags;
        public Transform page;
        public G5Btn btnDone;
        public G5Btn btnPrint;
        public Button btnEdit;
        public Button btnBook;
        public Image pic;
        public Transform Drop;
        public Transform editor;
        public void Start()
        {
            Show(true);
        }
        public void Begin()
        {
            Show(false);
            btnDone.IsActive = false;
            btnDone.SetStatus(0);
            btnPrint.IsActive = false;
            btnPrint.SetStatus(0);
        }
        public void Show(bool isShow)
        {
            editor.gameObject.SetActive(isShow);
            btnEdit.gameObject.SetActive(isShow);
            pic.gameObject.SetActive(isShow);
            txtcontent.gameObject.SetActive(!isShow);
            target.gameObject.SetActive(!isShow);
            Drags.ToObservable().Subscribe(x => x.gameObject.SetActive(!isShow));
            DragsDisable.ToObservable().Subscribe(x => x.gameObject.SetActive(!isShow));
            page.gameObject.SetActive(!isShow);
            btnDone.gameObject.SetActive(!isShow);
            btnPrint.gameObject.SetActive(!isShow);
        }
        public void SetContent(string content)
        {
            txtcontent.text = content;
        }
        public void SetPage(string pagecount)
        {
            page.GetChild(1).GetComponent<Text>().text = pagecount;
        }
        public void SetContentDrag(string[] contentDrag)
        {
            int i = 0;
            Drags.ToObservable().Subscribe(x => {
                DragsDisable[i].GetChild(1).GetComponent<Text>().text = contentDrag[i];
                x.GetChild(1).GetComponent<Text>().text = contentDrag[i++]; 
            });
        }
    }
}