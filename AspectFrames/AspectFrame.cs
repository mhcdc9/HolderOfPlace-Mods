using ADV;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AspectFrames
{
    public class AspectFrame : MonoBehaviour
    {
        public Card card;
        public SpriteRenderer spriteRenderer;
        bool iconOnly;

        public List<GameObject> icons = new List<GameObject>();

        public void Set(Card card)
        {
            this.card = card;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AspectAdded(Card aspect, Sprite customSprite)
        {
            if (card.GetAspectCount() <= icons.Count)
            {
                foreach (GameObject i in icons)
                {
                    if (i != null)
                    {
                        Destroy(i);
                    }
                }
                icons.Clear();
            }
            
            Sprite sprite = aspect?.transform?.Find("AnimBase/NewAliveBase/Base")?.GetComponent<SpriteRenderer>()?.sprite;
            if (customSprite != null && customSprite.rect.height <= 350)
            {
                sprite = customSprite;
            }
            iconOnly = (AspectFramesMod.instance.ignoreCustomFrames || icons.Count >= 1 || customSprite == null || customSprite.rect.height <= 350);
            GameObject icon = AddIcon(iconOnly);
            icon.GetComponent<SpriteRenderer>().sprite = sprite;
            spriteRenderer.sprite = iconOnly ? null : customSprite;
            UpdateIcons();
        }

        public float spacing = 1f;

        public void UpdateIcons()
        {
            StopAllCoroutines();
            for(int i=0; i<icons.Count; i++)
            {
                icons[i].SetActive(iconOnly);
                Transform t = icons[i].transform;
                t.localPosition = new Vector3(1.4f, 2.1f - spacing*i, -0.1f + 0.001f * i);
            }
        }

        public float speed = 1.5f;

        public float rotation = -1;
        public float offset = 90f;
        public void StartCircle()
        {
            StartCoroutine(Circle());
        }
        public IEnumerator Circle()
        {
            List<float> angles = new List<float>();
            for(int i=0; i<icons.Count; i++)
            {
                angles.Add((2*Mathf.PI*i) / (icons.Count));
            }
            while(true)
            {
                for(int i=0; i<icons.Count; i++)
                {
                    icons[i].transform.localPosition = new Vector3(1.5f * Mathf.Cos(angles[i]), 1.95f, -0.1f * Mathf.Sin(angles[i]));
                    icons[i].transform.rotation = Quaternion.Euler(new Vector3(0, offset + rotation * angles[i] * 180 / Mathf.PI, 0));
                }
                yield return null;
                for(int i=0; i<angles.Count; i++)
                {
                    angles[i] += Time.deltaTime * speed;
                    angles[i] %= 2*Mathf.PI;
                }
            }
        }

        public GameObject AddIcon(bool setActive = true)
        {
            GameObject icon = new GameObject("AspectIcon", typeof(SpriteRenderer));
            icon.SetActive(setActive);
            icon.transform.SetParent(transform, false);
            icons.Add(icon);
            icon.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            return icon;
        }
    }
}
