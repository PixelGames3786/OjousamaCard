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

	public Image Back01, Back02,LeftTatie,RightTatie;

	private AudioSource Audio;

	private List<Sprite> Taties;
	private List<string> TatieNames;

	private string TargetText, AddText;

	private int CurrentSentenceID = -1,NowCharaNum = 0, NowLineNum = 1;

	public float NeedAddTime;
	private float AddTime;

	public bool CanNext=true,Showing,WhichBack=true;



	void Start()
	{
		CurrentSentenceID = SaveLoadManager.instance.NextNovel;

		//立ち絵読み込み
		Taties = Resources.LoadAll<Sprite>("Novel/Taties").ToList();
		TatieNames = Taties.Select(Tatie => Tatie.name).ToList();

		Audio = GetComponent<AudioSource>();

		//最初の1行目を表示します。
		ReadmoreMessage();
	}

	private void Update()
	{
        if ((Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Space))&&CanNext)
        {
			CanNext = false;

			ReadmoreMessage();
        }
		//一括表示
		else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && Showing)
        {
			massage.text = TargetText;

			AddText = "";

			NowCharaNum = 0;

			Showing = false;
			CanNext = true;
        }

		if (Showing)
		{
			AddTime += Time.deltaTime;

			if (AddTime >= NeedAddTime)
			{
				string RichText = "";

				AddTime = 0;

                if (TargetText[NowCharaNum]=='<')
                {
                    while (true)
                    {
						RichText += TargetText[NowCharaNum];

						if (TargetText[NowCharaNum] == '>')
						{
							break;
						}

						NowCharaNum++;
                    }

					AddText += RichText;
                }
                else
                {
					AddText += TargetText[NowCharaNum];
				}

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

		AboutTatie(result);

		BackGroundChange(result);

		BGM(result);

		SE(result);

		//特殊な演出がないかチェック
		StartCoroutine("CheckSpecial",result);

		//メッセージがこれ以上ない場合はダイアログUIを非アクティブにする。
		EndOfTalk(result);
        //得られたメッセージを表示します。

        if (result.message!=""&&CanNext)
        {
			ShowingMassage(result);
		}

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

		switch (Splited[0])
        {
			case "Rise":

                {
					switch (Splited[2])
					{
						case "Left":

							LeftTalker.DOAnchorPosX(-960, 0.3f);

							LeftTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[1];

							break;

						case "Right":

							RightTalker.DOAnchorPosX(960, 0.3f);

							RightTalker.GetChild(0).GetComponent<TextMeshProUGUI>().text = Splited[1];

							break;
					}
				}

				break;

			case "Close":

                {
					switch (Splited[1])
					{
						case "Left":

							LeftTalker.DOAnchorPosX(-1460, 0.3f);

							break;

						case "Right":

							RightTalker.DOAnchorPosX(1460, 0.3f);

							break;

						case "Both":

							LeftTalker.DOAnchorPosX(-1460, 0.3f);
							RightTalker.DOAnchorPosX(1460, 0.3f);

							break;
					}
				}

				break;
        }

        
    }

	public void SE(Sentence sentence) 
	{
        if (sentence.SE=="")
        {
			return;
        }

		string[] content = sentence.SE.Split(',');

		switch (content[0])
		{
			case "Play":

				{
					BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));
				}

				break;

			case "PlayLoop":

				BGMManager.instance.SetLoop("SE",true);
				BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));

				break;

			case "PlayStopLoop":

				BGMManager.instance.SetLoop("SE", false);
				BGMManager.instance.SEPlay(content[1], float.Parse(content[2]));

				break;

			case "Stop":

				BGMManager.instance.SEStop(float.Parse(content[1]));

				break;
		}
	}

	public void BGM(Sentence sentence)
    {
        if (sentence.BGM=="")
        {
			return;
        }

		string[] content = sentence.BGM.Split(',');

		switch (content[0])
        {
			case "Play" :

                {
					BGMManager.instance.BGMPlay(content[1],float.Parse(content[2]));
                }

				break;

			case "PlayLoop":

				{
					BGMManager.instance.BGMPlay(content[1], float.Parse(content[2]));
					BGMManager.instance.SetLoop("BGM", true);
				}

				break;

			case "PlayStopLoop":

                {
					BGMManager.instance.BGMPlay(content[1], float.Parse(content[2]));
					BGMManager.instance.SetLoop("BGM", false);
				}

				break;

			case "Stop":

				{
					BGMManager.instance.BGMStop(float.Parse(content[1]));
				}

				break;
        }
    }

	public void AboutTatie(Sentence sentence)
    {
        if (sentence.Tatie=="")
        {
			return;
        }

		string[] content = sentence.Tatie.Split(',');
		Image Target = null;


		if (content[2] == "Left")
		{
			Target = LeftTatie;
		}
		else
		{
			Target = RightTatie;
		}

		//タイプ別
		switch (content[0])
        {
			case "FadeIn" :
				int Num = TatieNames.IndexOf(content[1]);

				Target.sprite = Taties[Num];

				Target.DOFade(1f,0.5f);

				break;

			case "FadeOut":

				if (content[2] == "Left")
				{
					Target = LeftTatie;
				}
				else if(content[2]=="Right")
				{
					Target = RightTatie;

				}else if (content[2]=="Both")
                {
					LeftTatie.DOFade(0f,0.5f);
					RightTatie.DOFade(0f,0.5f);

					break;
                }

				Target.DOFade(0f, 0.5f);

				break;

			case "Change":

				Num = TatieNames.IndexOf(content[1]);

				Target.sprite = Taties[Num];

				break;
        }
    }

	public void BackGroundChange(Sentence sentence)
    {
        if (sentence.BackGround=="")
        {
			return;
        }

		string[] content = sentence.BackGround.Split(',');

		float Time = float.Parse(content[1]);

		Sprite ChangeImage = Resources.Load<Sprite>("Novel/BackGrounds/"+content[0]);

		//01が起動している場合02にかえる
		if (WhichBack)
        {
			Back02.sprite = ChangeImage;

			Back01.DOFade(0f, Time);

			Back02.DOFade(1f,Time);

        }
		//02が起動している場合01にかえる
        else
        {
			Back01.sprite = ChangeImage;

			Back02.DOFade(0f, Time);

			Back01.DOFade(1f, Time);
		}

		WhichBack = !WhichBack;
    }

	public IEnumerator CheckSpecial(Sentence sentence)
    {


        if (sentence.Special.Contains("SceneFadeIn"))
        {
			CanNext = false;

			string[] content = sentence.Special.Split(',');

			GameObject.Find("Fader").GetComponent<Image>().DOFade(0f, float.Parse(content[1]));

			yield return new WaitForSeconds(float.Parse(content[1]));
        }
		else if (sentence.Special.Contains("SceneFadeOut"))
        {
			CanNext = false;

			string[] content = sentence.Special.Split(',');

			GameObject.Find("Fader").GetComponent<Image>().DOFade(1f, float.Parse(content[1])).OnComplete(()=> { massage.text = ""; });

			yield return new WaitForSeconds(float.Parse(content[1]));
		}
		else if (sentence.Special.Contains("GameEnd"))
        {
			CanNext = false;

			GameObject.Find("Fader").GetComponent<Image>().DOFade(1f, 5f).OnComplete(() => { massage.text = ""; });

			yield break;
		}

		if (sentence.NeedClick)
		{
			CanNext = true;
		}
		else
		{
			ReadmoreMessage();
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

			SceneController.instance.StartSceneLoad(sentence.sceneChange);
        }
    }

	public void OnApplicationFocus(bool Focus)
    {

    }
}
