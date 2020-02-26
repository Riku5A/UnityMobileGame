using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Material[] ballMaterials;
    public GameObject bounusColor;
    public GameObject ShakeButton;
    public GameObject ResultUI;
    public GameObject wall;
    public GameObject scoreUI;
    public GameObject timeUptext;
    private Text scoreText;
    private int bounusID;
    private int score = 0;
    private int point = 100;
    private float limitTime = 60;
    private GameObject firstBall;
    private GameObject lastBall;
    private string currentName;
    private Color32 currentColor;
    private bool st;
    public GameObject stButton;
    public GameObject timeGUI;
    List<GameObject> removeBallList = new List<GameObject>();
    List<GameObject> removeLineList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DropBall(50));
        bounusID = Random.Range(0,4);
        bounusColor.GetComponent<Renderer>().material = ballMaterials[bounusID];
    }

    // Update is called once per frame
    void Update()
    {
        if(st == true)
        {
            if(Input.GetMouseButtonDown(0) && firstBall == null)
            {
                OnDragStart();
            }else if(Input.GetMouseButtonUp(0))
            {
                OnDragEnd();
            }else if(firstBall != null)
            {
                OnDraging();
            }
            limitTime -= Time.deltaTime;
            if(limitTime <= 0)
            {
                limitTime = 0;
                st = false;
                StartCoroutine(EndGame());
            }
        }
        timeGUI.GetComponent<Text>().text = ((int)limitTime).ToString();
    }

    private void OnDragStart()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            if(hit.collider.tag == "Ball")
            {
                GameObject hitObj = hit.collider.gameObject;
                firstBall = hitObj;
                lastBall = hitObj;
                currentName = hitObj.name;
                removeBallList = new List<GameObject>();
                PushToList(hitObj);
            }
        }
    }

    private void OnDraging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            if(hit.collider.tag == "Ball")
            {
                GameObject hitObj = hit.collider.gameObject;
                if(hitObj.name == currentName && lastBall != hitObj)
                {
                    float distance = Vector3.Distance(hitObj.transform.position,lastBall.transform.position);
                    if(distance < 1.0f)
                    {
                        CreateLine(lastBall.transform.position,hitObj.transform.position);
                        lastBall.name = "ball";
                        lastBall = hitObj;
                        PushToList(hitObj);
                    }
                }
            }
        }
    }

    private void OnDragEnd()
    {
        int remove_cnt = removeBallList.Count;
        if(remove_cnt >= 3)
        {
            for(int i = 0; i < remove_cnt; i++)
            {
                Destroy(removeBallList[i]);
            }
            if(bounusID == int.Parse(currentName.Substring(4,1)))
            {
                score += point * remove_cnt * 2;
            }else{
                score += point * remove_cnt;
            }
            bounusID = Random.Range(0,4);
            bounusColor.GetComponent<Renderer>().material = ballMaterials[bounusID];
            StartCoroutine(DropBall(remove_cnt));
            removeBallList.Clear();
        }else{
            for(int i = 0; i < remove_cnt; i++)
            {
                removeBallList[i].name = currentName;
                MeshRenderer mr = removeBallList[i].GetComponent<MeshRenderer>();
                mr.material.color = currentColor;
            }
        }
        int remove_cnt2 = removeLineList.Count;
        for(int j = 0; j < remove_cnt2; j++)
        {
            Destroy(removeLineList[j]);
        }
        firstBall = null;
        lastBall = null;
    }

    IEnumerator DropBall(int count) //ボールを落とす
    {
        for(int i = 0; i < count; i++)
        {
            //ランダムなX:-2~2の位置
            Vector3 pos = new Vector3(Random.Range(-2.0f,2.0f),0f,1f);
            GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity) as GameObject;
            int materialID = Random.Range(0,4);
            ball.name = "ball" + materialID;
            ball.GetComponent<Renderer>().material = ballMaterials[materialID];
            yield return new WaitForSeconds(0.0f);
        }
    }

    void PushToList(GameObject obj)
    {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        currentColor = mr.material.color;
        mr.material.color = mr.material.color - new Color32(150,150,150,0);
        removeBallList.Add(obj);
    }

    void PushToLineList(GameObject obj)
    {
        removeLineList.Add(obj);
    }

    void CreateLine(Vector3 start,Vector3 end)
    {
        GameObject newObj = new GameObject();
        PushToLineList(newObj);
        newObj.transform.parent = transform;
        LineRenderer newLine = new LineRenderer();
        newLine = newObj.AddComponent<LineRenderer>();
        newLine.SetPosition(0,start + new Vector3(0f,0f,-0.3f));
        newLine.SetPosition(1,end + new Vector3(0f,0f,-0.3f));
        newLine.SetColors(Color.red,Color.red);
        newLine.SetWidth(0.25f,0.25f);
    }

    public void OnStart()
    {
        st = true;
        stButton.SetActive(false);
        ShakeButton.SetActive(true);
    }

    public void Shake()
    {
        if(st == true)
        {
            StartCoroutine(Shaking());
        }
    }

    IEnumerator EndGame()
    {
        float startTime = Time.time;
        float endTime = startTime + 2f;
        timeUptext.SetActive(true);
        yield return new WaitForSeconds(1f);
        timeUptext.SetActive(false);
        ResultUI.SetActive(true);
        scoreText = scoreUI.GetComponent<Text>(); 
        do
        {
            //現在の時間の割合
            float timeRate = (Time.time - startTime) / 2f;
            float updateValue = (float)((score * timeRate));
            scoreText.text = "Score: " + updateValue.ToString("f0");
            yield return null;
        } while(Time.time < endTime);
        scoreText.text = "Score: " + score.ToString();
    }

    IEnumerator Shaking()
    {
        wall.transform.localScale += new Vector3(0,1,0);
        yield return new WaitForSeconds(0.05f);
        wall.transform.localScale -= new Vector3(0,1,0);
    }
}
