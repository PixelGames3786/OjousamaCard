using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NovelManager : MonoBehaviour
{
    [SerializeField] GameObject dialogue = null;
    [SerializeField] GameObject choices = null;
    [SerializeField] Button readMore = null;
    [SerializeField] TextMeshProUGUI massage = null;
    [SerializeField] Text yBranchMassage = null;
    [SerializeField] Text nBranchMassage = null;
    [SerializeField] OjousamaNovel TextDatas = null;

	public RectTransform LeftTalker, RightTalker;

	private string TargetText, AddText;

	private int CurrentSentenceID = -1,NowCharaNum = 0, NowLineNum = 1;

	public float NeedAddTime;
	private float AddTime;

	public bool CanNext=true,Showing;

	private void OnEnable()
	{
		CurrentSentenceID = SaveLoadManager.instance.NextNovel;

		//最初の1行目を表示します。
		ReadmoreMessage();
	}

	private void Update()
	{
        if ((Input.GetMouseButtonDown(0)||Input.GetKey(KeyCode.Space))&&CanNext)
        {
			CanNext = false;

			ReadmoreMessage();
        }

		if (Showing)
		{
			AddTime += Time.deltaTime;

			if (AddTime >= NeedAddTime)
			{
				AddTime = 0;

				AddText += TargetText[NowCharaNum];

				massage.text = AddText;

				NowCharaNum++;

				//もし全部表示し終わったなら
				if (NowCharaNum >= TargetText.Length)
				{
					AddText = "";

					NowCharaNum = 0;

					Showing = false;
					CanNext = true;
				}
			}
		}
	}

	//文章の続きを表示します。
	public void ReadmoreMessage()
	{

		//文章番号をもとに文を検索します。
		Sentence result = SerchSentence(CurrentSentenceID);

		//シーンチェンジ
		SceneChange(result);

		//喋り手のウィンドウを出す
		TalkerWindow(result);

		//メッセージがこれ以上ない場合はダイアログUIを非アクティブにする。
		EndOfTalk(result);
		//得られたメッセージを表示します。
		ShowingMassage(result);
		ShowingMassageIsBranch(result);
		Connect(result);

		//文章番号をひとつだけ次に進めます。
		CurrentSentenceID++;
	}

	//メッセージを表示させるだけです。
	void ShowingMassage(Sentence sentence)
	{
		TargetText = sentence.message;
		massage.text = "";

		Showing = true;
		CanNext = false;

		//massage.text = sentence.message;
	}

	Sentence SerchSentence(int Id)
	{
		//シナリオのメッセージ配列の中から文章をIDで検索して取得します。
		Sentence result = TextDatas.sentences.First(
			(Sentence line) => { return line.id == Id; }
		);
		return result;
	}

	//選択肢のメッセージを表示させるだけです。
	//選択肢のUIはボタンとテキストで構成されています。
	void ShowingMassageIsBranch(Sentence sentence)
	{
		if (sentence.branch)
		{
			SwichReadmoreInteractable();
			SwichCoicesActivate();
			yBranchMassage.text = sentence.yesMessage;
			nBranchMassage.text = sentence.noMessage;
		}
	}

	//肯定を選択したとき、ボタンコンポーネントのEventから呼び出されます。
	public void ChooseYes()
	{
		//選択肢を押下したときReadmoreボタンを有効にします。
		SwichReadmoreInteractable();
		//選択肢を非表示にします。
		SwichCoicesActivate();

		Sentence result = SerchSentence(CurrentSentenceID);
		Skip(result.choseYes);
	}

	//否定を選択したとき、ボタンコンポーネントのEventから呼び出されます。
	public void ChooseNo()
	{
		//選択肢を押下したときReadmoreボタンを有効にします。
		SwichReadmoreInteractable();
		//選択肢を非表示にします。
		SwichCoicesActivate();

		Sentence result = SerchSentence(CurrentSentenceID);
		Skip(result.choseNo);
	}

	//文章を読み飛ばします。
	//選択の結果、シナリオが分岐したのち収束するときなどに使用します。
	void Connect(Sentence sentence)
	{
		if (sentence.doConnect)
		{
			Skip(sentence.skipId);
		}
	}

	//別の文章に読み飛ばします。
	void Skip(int id)
	{
		Sentence result = SerchSentence(id);
		CurrentSentenceID = result.id;
		ShowingMassage(result);
	}

	//最後の文章になったらUIを非表示にします。
	//会話が終わったのにもかかわらず、UIが表示されていたらバグです。
	//だから非表示する必要があります。
	void EndOfTalk(Sentence sentence)
	{
		if (sentence.endOfTalk)
		{
			//このゼロは現在読んでいる文章をリセットするための数値です。
			CurrentSentenceID = 0;
			SwichDialougeActivate();
		}
	}



	//Readmoreボタンを押下の可不可を切り替えます。
	//選択肢があるとき、選択肢以外のボタンを押せなくするために使用します。
	void SwichReadmoreInteractable()
	{
		if (readMore.interactable)
		{
			readMore.interactable = false;
		}
		else
		{
			readMore.interactable = true;
		}
	}

	//アクティブ状態を切り替えます。
	void SwichActiveState(GameObject target)
	{
		target.SetActive(!target.activeSelf);

	}

	//選択肢のアクティブ状態を切り替えます。
	void SwichCoicesActivate()
	{
		if (choices.activeSelf)
		{
			choices.SetActive(false);
		}
		else
		{
			choices.SetActive(true);
		}
	}

	//対話UIのアクティブ状態を切り替えます。
	void SwichDialougeActivate()
	{
		if (dialogue.activeSelf)
		{
			dialogue.SetActive(false);
		}
		else
		{
			dialogue.SetActive(true);
		}
	}

	public void TalkerWindow(Sentence sentence)
    {
        if (sentence.Talker=="")
        {
			return;
        }

		string[] Splited = sentence.Talker.Split(',');

        switch (Splited[1])
        {
			case "Left":

				LeftTalker.DOAnchorPosX(-960,0.3f);

				LeftTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[0];

				break;

			case "Right":

				RightTalker.DOAnchorPosX(960, 0.3f);

				RightTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[0];

				break;
        }
    }


	//シーンチェンジします。
	void SceneChange(Sentence sentence)
    {
        if (sentence.sceneChange!="")
        {
            if (sentence.nextBattle!="")
            {
				SaveLoadManager.instance.NextBattle = sentence.nextBattle;
            }

			SceneManager.LoadScene(sentence.sceneChange);
        }
    }

	public void OnApplicationFocus(bool Focus)
    {

    }
}
