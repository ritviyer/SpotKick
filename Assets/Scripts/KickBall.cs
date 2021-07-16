using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBall : MonoBehaviour
{
    Rigidbody rb;
    Vector3 firstpos, lastpos;
    Vector3 force;
    float distance;
    Vector3 startpos;
    Vector3 cursorStartPos;
    Quaternion startRotation;
    public Vector3 WindSpeed;

    float dragdistance;

    public bool returned = true;
    bool kickEnabled = false;
    public bool drag = false;
    public bool inAir = false;

    bool touchOnBall = false;
    [SerializeField] GameObject players;
    Animator animator;

    private LineRenderer line;
    [SerializeField] GameObject cursor;


    int attempts;
    int currentLevel;

    void Start()
    {
        dragdistance = transform.localScale.x * 20f;
        startpos = transform.position;
        cursorStartPos = cursor.transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }
    private void OnEnable()
    {
        EventManager.onStartGame += EnableKick;
        EventManager.onRefreshGame += EnableKick;
        EventManager.onLevelComplete += LevelUp;
    }
    private void OnDisable()
    {
        EventManager.onStartGame -= EnableKick;
        EventManager.onRefreshGame -= EnableKick;
        EventManager.onLevelComplete -= LevelUp;
    }
    void EnableKick()
    {
        for(int i = 0; i <players.transform.childCount; i++)
        {
            if (players.transform.GetChild(i).gameObject.activeSelf)
            {
                animator = players.transform.GetChild(i).GetComponent<Animator>();
                break;
            }
        }
        attempts = 0;
        currentLevel = GlobalVariables.currentLevel;
        kickEnabled = true;
        StopAllCoroutines();
        StartCoroutine(Returnball(0f));
        int level = PlayerPrefs.GetInt("level", 1);
        if (level == 1 || level == 2)
        {
            cursor.SetActive(true);
            cursor.transform.position = cursorStartPos;
            LeanTween.move(cursor, cursor.transform.position + Vector3.back*2f, 1f).setLoopType(LeanTweenType.linear);
        }
    }
    void LevelUp()
    {
        int stars = PlayerPrefs.GetInt("level" + currentLevel.ToString() + "stars", 0);
        int currStars;
        if (attempts == 1)
            currStars = 3;
        else if (attempts <= 3)
            currStars = 2;
        else
            currStars = 1;

        TinySauce.OnGameFinished(true, currStars, levelNumber: currentLevel.ToString());

        if (currStars>stars)
            PlayerPrefs.SetInt("level" + currentLevel.ToString() + "stars", currStars);

        kickEnabled = false;
        StopAllCoroutines();
        animator.SetBool("levelComplete", true);
    }
    void Update()
    {
        if (returned && kickEnabled)
        {
            Player();
            //PlayerOld();
        }
    }
    void Player()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && (hit.transform.gameObject.name == "CueBall" || InRadius(hit.point)))
            {
                touchOnBall = true;
                if (cursor.activeSelf)
                {
                    LeanTween.cancel(cursor);
                    cursor.SetActive(false);
                }
            }
        }
        if (Input.GetMouseButton(0) && touchOnBall)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity))
            {
                Vector3 lineWantedPosition = new Vector3(mouseHit.point.x, mouseHit.point.y, mouseHit.point.z);
                line.positionCount = 2;
                line.SetPosition(0, transform.position);
                Vector3 eP = GetAimPosition(transform.position, lineWantedPosition);
                line.SetPosition(1, eP);

                if ((transform.rotation.eulerAngles.y >= 0f && transform.rotation.eulerAngles.y <= 95f) || (transform.rotation.eulerAngles.y <= 360f && transform.rotation.eulerAngles.y >= 265f))
                {
                    line.enabled = true;
                }
                else
                    line.enabled = false;
            }
        }
        if (Input.GetMouseButtonUp(0) && touchOnBall)
        {
            touchOnBall = false;

            if (distance>1f && line.enabled)
            {
                animator.gameObject.transform.position = new Vector3(0f, 1f, -9.25f);
                animator.SetBool("toKick", true);
                line.enabled = false;
            }
        }
    }
    bool InRadius(Vector3 touchPoint)
    {
        if (touchPoint.x >= transform.position.x - 1.5f && touchPoint.x <= transform.position.x + 1.5f && touchPoint.z >= transform.position.z - 1.5f && touchPoint.z <= transform.position.z + 1.5f)
            return true;
        return false;
    }
    Vector3 GetAimPosition(Vector3 startPoint ,Vector3 endPoint)
    {
        //endPoint.y = startPoint.y;
        //float distance = Mathf.Clamp(Vector3.Distance(startPoint, endPoint), 0, 2.25f) * -1.5f;
        //return Vector3.MoveTowards(startPoint,endPoint, distance);

        endPoint.y = startPoint.y;
        Vector3 direction = endPoint - startPoint;
        float angle = Vector3.Angle(direction, Vector3.back);
        if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, angle * -1f, 0f);

        distance = Mathf.Clamp(Vector3.Distance(startPoint, endPoint),0f,10f);
        return Vector3.MoveTowards(startPoint, startPoint + transform.forward*10f, distance);
    }
    Vector3 CalculateForce(Vector3 start, Vector3 end)
    {
        //Vector3 distance = end - start;
        //distance.z = distance.y * -1f;
        //distance.y = 0f;
        //distance.x = -1f * distance.x;

        //distance = (distance * 4f);
        //distance.x = Mathf.Clamp(distance.x, -250f, 250f);
        //distance.z = Mathf.Clamp(distance.z, -250f, 250f);
        //return distance;

        Vector3 distance = start-end;
        distance.z = distance.y;
        distance.y = 0f;

        distance.x = Mathf.Clamp(distance.x, -250f, 250f);
        distance.z = Mathf.Clamp(distance.z, -250f, 250f);
        distance = distance / 50f;
        return distance;
    }
    void FixedUpdate()
    {
        if (inAir)
        {
            rb.AddForce(WindSpeed);
        }
    }
    void SetWind()
    {
        WindSpeed = new Vector3(Random.Range(-5, 5), 0, 0);
    }

    public void AddForceToBall()
    {
        attempts += 1;
        FindObjectOfType<SoundManager>().PlayKickMusic();
        rb.isKinematic = false;
        rb.AddForce(transform.forward*(distance/1.5f), ForceMode.Impulse);
        animator.SetBool("toKick", false);
        //inAir = true;
        returned = false;
        StartCoroutine(Returnball(3f));
    }
    IEnumerator Returnball(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startpos;
        transform.rotation = startRotation;
        rb.isKinematic = true;
        animator.gameObject.transform.position = new Vector3(-2f, 1f, -9.25f);
        animator.SetBool("levelComplete", false);
        SetWind();
        //Debug.Log(WindSpeed);
        inAir = false;
        returned = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Net")
        {
            //Lost
        }
    }
}
