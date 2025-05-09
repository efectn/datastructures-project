import feedparser
import json
import re

def clean_html(raw_html):
    raw_html = re.sub(r'<a [^>]*>(.*?)<\/a>', r'\1', raw_html)
    raw_html = re.sub(r'<[^>]+>', '', raw_html)
    return raw_html.strip()

def rss_to_json(rss_urls):
    all_entries = []

    for url in rss_urls:
        feed = feedparser.parse(url)
        for entry in feed.entries:
            title = clean_html(entry.get("title", ""))
            if len(title) > 50:
                title = title[:50]
            item = {
                "title": title,
                "url": entry.get("link", ""),
                "description": clean_html(entry.get("description", ""))
            }
            all_entries.append(item)

    return json.dumps(all_entries, indent=2, ensure_ascii=False)

rss_links = [
    "https://www.aa.com.tr/tr/rss/default?cat=guncel",
    "https://www.haberturk.com/rss",
    "https://arkeofili.com/feed/",
    "https://feeds.bbci.co.uk/turkce/rss.xml",
    "https://www.donanimhaber.com/rss/tum/",
    "https://www.sozcu.com.tr/feeds-haberler",
    "https://www.mynet.com/haber/rss/sondakika",
    "https://www.turkiyehaberajansi.com/rss.xml",
    "https://www.hurriyet.com.tr/rss/gundem",
    "https://www.hurriyet.com.tr/rss/ekonomi",
    "https://www.hurriyet.com.tr/rss/teknoloji",
    "https://www.milliyet.com.tr/rss/rssnew/dunyarss.xml",
    "https://www.trthaber.com/sondakika.rss",
    "https://bigpara.hurriyet.com.tr/rss/",
    "https://www.ntv.com.tr/otomobil.rss",
    "https://www.ntv.com.tr/sanat.rss",
    "https://www.ntv.com.tr/dunya.rss",
    "https://www.ntv.com.tr/turkiye.rss",
    "https://haberglobal.com.tr/rss",
    "https://popsci.com.tr/feed/",
    "https://www.bilimoloji.com/feed/",
    "https://www.chip.com.tr/rss",
]

json_output = rss_to_json(rss_links)

with open("src/Resources/documents.json", "w", encoding="utf-8") as json_file:
    json_file.write(json_output)
