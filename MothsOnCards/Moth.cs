using ADV;
using ModdingCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MothsOnCards
{
    public class Moth : MonoBehaviour
    {
        public static Moth prefab;
        public static float minFlutterStart = 3f;
        public static float maxFlutterStart = 10f;

        public Wing left;
        public Wing right;
        public float currentAngle;

        public static void CreatePrefab(string path)
        {
            GameObject obj = new GameObject("MothPrefab", typeof(Moth), typeof(SpriteRenderer));
            DontDestroyOnLoad(obj);
            prefab = obj.GetComponent<Moth>();
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            renderer.sprite = BootstrapMain.GetSprite(path + "/Images/moth_body.png");

            GameObject leftObj = new GameObject("LeftWing", typeof(Wing), typeof(SpriteRenderer));
            leftObj.transform.SetParent(obj.transform);
            Texture2D tex = BootstrapMain.GetTex(path + "/Images/moth_wing.png");
            leftObj.GetComponent<SpriteRenderer>().sprite = BootstrapMain.GetSprite(tex, new Vector2(0, 0.5f));
            leftObj.transform.localScale = new Vector3(-1, 1, 1);

            GameObject rightObj = new GameObject("LeftWing", typeof(Wing), typeof(SpriteRenderer));
            rightObj.transform.SetParent(obj.transform);
            rightObj.GetComponent<SpriteRenderer>().sprite = BootstrapMain.GetSprite(tex, new Vector2(0, 0.5f));

            prefab.left = leftObj.GetComponent<Wing>();
            prefab.right = rightObj.GetComponent<Wing>();
            prefab.gameObject.SetActive(false);
        }

        public static void Create(Card card, Color c, int amount)
        {
            List<Vector3> positions = new List<Vector3>();
            for(int i=0; i<amount; i++)
            {
                Moth moth = GameObject.Instantiate(prefab, card.transform.Find("AnimBase"));
                moth.transform.localScale = new Vector3(Rand(1.75f,2.5f), Rand(1.75f,2.5f), 1);
                moth.transform.localRotation = Quaternion.Euler(0, 0, Rand(0, 360));
                positions.Add(RandomVector(-5, 5, 25, positions));
                moth.transform.localPosition = positions[i];
                Color color = (c == Color.black) ? new Color(1 - Rand() * Rand(), 1 - Rand() * Rand(), 1 - Rand() * Rand()) : c;
                moth.ChangeColor(color);
                moth.currentAngle = Rand(0, 30);
                moth.RotateWings(moth.currentAngle);
                moth.gameObject.SetActive(true);
            }
        }

        public void Start()
        {
            StartCoroutine(RandomFlutter());
        }

        public IEnumerator RandomFlutter()
        {
            while(true)
            {
                yield return new WaitForSeconds(Rand(minFlutterStart, maxFlutterStart));
                float oldAngle = currentAngle;
                float newAngle = 70f;
                float targetTime = 0.5f;
                float currentTime = 0f;
                while (currentTime < targetTime)
                {
                    currentTime += Time.deltaTime;
                    currentAngle = Interp(oldAngle, newAngle, currentTime / targetTime);
                    RotateWings(currentAngle);
                    yield return null;
                }
                oldAngle = currentAngle;
                newAngle = Rand(0, 15);
                currentTime = 0f;
                while (currentTime < targetTime)
                {
                    currentTime += Time.deltaTime;
                    currentAngle = Interp(oldAngle, newAngle, currentTime / targetTime);
                    RotateWings(currentAngle);
                    yield return null;
                }
            }
        }

        public static float Interp(float oldValue, float newValue, float t) => t * newValue + (1 - t) * oldValue;

        public static Vector3 RandomVector(float min, float max, float tolerance, List<Vector3> awayFrom)
        {
            Vector3 v = Vector3.zero;
            bool flag;
            for (int i = 0; i < 100; i++)
            {
                flag = true;
                v = new Vector3(Rand(min,max), Rand(min,max), -1);
                foreach(Vector3 w in awayFrom)
                {
                    if ((v - w).sqrMagnitude < tolerance * (1 - 0.01*i))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            return v;
        }
        public static float Rand(float min = 0, float max=1) => UnityEngine.Random.Range(min, max);

        public void RotateWings(float angle)
        {
            left.transform.localRotation = Quaternion.Euler(0, angle, 0);
            right.transform.localRotation = Quaternion.Euler(0, -angle, 0);
        }

        public void ChangeColor(Color color)
        {
            left.ChangeColor(color);
            right.ChangeColor(color);
        }

        public class Wing : MonoBehaviour
        {
            public void ChangeColor(Color c)
            {
                GetComponent<SpriteRenderer>().color = c;
            }
        }
    }
}
