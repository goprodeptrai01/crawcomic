using System.Net;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using HtmlAgilityPack;
using ReadMangaTest.Data;
using ReadMangaTest.Models;
using ScrapySharp.Network;

namespace CrawComic;

public class WebScrapping
{
    private readonly DataContext _context;
    public WebScrapping(DataContext context)
    {
        Console.WriteLine("welcome to scraping");
        _context = context;
    }

    private HtmlNodeCollection GetHtmlNodesByScrapySharp(string url)
    {
        try
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage htmlPage = browser.NavigateToPage(new Uri(url));
            return htmlPage.Html.SelectNodes(".");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private HtmlNode GetHtmlNodeByScrapySharp(string url)
    {
        try
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage htmlPage = browser.NavigateToPage(new Uri(url));
            return htmlPage.Html;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    // public List<Genre> ScrapeCategory()
    public List<Genre> ScrapeGenre()
    {
        // URL of the web page you want to scrape data from
        var url = "https://kunmanga.com/manga-genre/action/";
        var htmlPage = GetHtmlNodeByScrapySharp(url);

        var genreNodes = htmlPage.SelectNodes(".//li[@class='col-6 col-sm-4 col-md-2']");
        // Console.WriteLine(GenreNodes);
        var genres = new List<Genre>();
        if (genreNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return null;
        }


        foreach (var genreNode in genreNodes)
        {
            var genre = new Genre();
            var nameGenre = genreNode.SelectSingleNode(".//a").InnerText.Trim().Split('\n').ToArray()[0];
            genre.Name = nameGenre;
            var urlGenre = genreNode.SelectSingleNode(".//a").Attributes["href"].Value;
            genre.Url = urlGenre;
            genre.Description = "";

            genres.Add(genre);
        }

        // throw new Exception("Debug");
        return genres;
        // return;
        // _context.Genres.AddRange(Genres);
        // _context.SaveChanges();
        Console.WriteLine("Data has been saved to the database successfully");
    }

    [Obsolete("Obsolete")]
    public async void ScrapeComicAndArtist()
    {
        var genreList = ScrapeGenre();
        var comics = new List<Comic>();
        var artists = new List<Artist>();
        var comicGenres = new List<ComicGenre>();
        var chapters = new List<Chapter>();
        var pages = new List<Page>();
        var nullArtist = new Artist()
        {
            Name = "IsUpdating",
            Description = "Is Updating!",
            Url = "Is Updating!"
        };
        artists.Add(nullArtist);
        // URL of the web page you want to scrape data from
        var url = "https://kunmanga.com/";

        var htmlPages = new List<HtmlNode>();

        int countUrl = 1;

        while (true)
        {
            string urlPage = url;
            if (countUrl > 1)
            {
                urlPage += "page/" + countUrl;
            }
            
            if (countUrl > 1)
            {
                break;
            }
            Console.WriteLine("Adding PageHtml: "+countUrl);
            Console.WriteLine(urlPage);
            
            var htmlPage = GetHtmlNodeByScrapySharp(urlPage);
            
            if (htmlPage == null)
            {
                break;
            }
            htmlPages.Add(htmlPage);
            countUrl++;
        }

        if (htmlPages == null)
        {
            Console.WriteLine("No data found on the web page");
            return;
        }

        Console.WriteLine("NumbOfPageHtml: " + htmlPages.Count);

        int countPage = 1;
        foreach (var htmlPage in htmlPages)
        {
            Console.WriteLine("Page: " + countPage);
            countPage++;
            var rowComicNodes = htmlPage.SelectNodes("//div[@class='page-listing-item']");

            Console.WriteLine(1);
            if (rowComicNodes == null)
            {
                Console.WriteLine("No data found on the web page");
                return;
            }

            Console.WriteLine(2);
            Console.WriteLine($"Found {rowComicNodes.Count} items on the web page");

            // Loop through the selected HTML nodes and extract the data
            Console.WriteLine(3);
            
            int count = 4;

            foreach (var rowComicNode in rowComicNodes)
            {
                var comicNodes = rowComicNode.SelectNodes("//div[@class='col-6 col-md-3 badge-pos-2']");
                foreach (var comicNode in comicNodes)
                {
                    if (count > 5)
                    {
                        break;
                    }
                    Console.WriteLine(count);
                    count++;
                    string detailUrl = comicNode.SelectSingleNode(".//div//div//a").Attributes["href"].Value;

                    var comicHtmlPage = GetHtmlNodeByScrapySharp(detailUrl);

                    if (comicHtmlPage == null)
                    {
                        throw new Exception("Page not found!");
                    }

                    var nameComic = comicNode
                        .SelectSingleNode(".//div[@class='post-title font-title']//h3[@class='h5']//a")
                        .InnerHtml.Trim();
                    Console.WriteLine(nameComic);

                    var comicDescriptionNode = comicHtmlPage
                        ?.SelectSingleNode(".//div[@class='c-page-content style-1']")
                        .SelectSingleNode(".//div[@class='description-summary']");

                    var nameArtist = comicHtmlPage?.SelectSingleNode("//div[@class='author-content']//a");

                    var nameGenres = comicHtmlPage?.SelectNodes(".//div[@class='genres-content']//a");

                    var listChapterNode = comicHtmlPage?.SelectNodes("//li[@class='wp-manga-chapter    ']");

                    Artist artist;
                    if (nameArtist == null)
                    {
                        artist = nullArtist;
                    }
                    else
                    {
                        Console.WriteLine("Adding artist: " + count);
                        artist = new Artist()
                        {
                            Name = nameArtist.InnerHtml.Trim(),
                            Url = nameArtist.Attributes["href"].Value,
                            Description = "",
                            Comics = new List<Comic>()
                        };
                    }

                    Console.WriteLine("Adding comic: " + count);
                    Console.WriteLine(comicNode.SelectSingleNode(".//div[@class='item-thumb  c-image-hover']//a//img")
                        .Attributes["src"]
                        .Value);
                    var urlWallpaper = await DownloadImageToString(comicNode.SelectSingleNode(".//div[@class='item-thumb  c-image-hover']//a//img")
                        .Attributes["src"]
                        .Value, nameComic, "",nameComic+".jpg");
                    var comic = new Comic
                    {
                        Name = nameComic,
                        Description = comicDescriptionNode != null
                            ? comicDescriptionNode.SelectSingleNode(".//div//div[@class='limit-html']") != null
                                ? comicDescriptionNode.SelectSingleNode(".//div//div[@class='limit-html']")
                                    .InnerHtml
                                    .Trim()
                                : comicDescriptionNode.SelectSingleNode(".//div//div[2]") != null
                                    ? comicDescriptionNode.SelectSingleNode(".//div//div[2]")
                                        .InnerHtml.Trim()
                                    : comicDescriptionNode.SelectSingleNode(".//div//div") != null
                                        ? comicDescriptionNode.SelectSingleNode(".//div//div")
                                            .InnerHtml.Trim()
                                        : comicDescriptionNode.SelectSingleNode(".//div//p[2]") != null
                                            ? comicDescriptionNode.SelectSingleNode(".//div//p[2]")
                                                .InnerHtml.Trim()
                                            : comicDescriptionNode.SelectSingleNode(".//div//p")
                                                .InnerHtml.Trim()
                            : "",
                        Url = comicNode.SelectSingleNode(".//div[@class='post-title font-title']//h3[@class='h5']//a")
                            .Attributes["href"]
                            .Value,
                        
                        Wallpaper = urlWallpaper,
                        Chapters = new List<Chapter>()
                    };
                    if (listChapterNode == null)
                    {
                        Console.WriteLine("chapter not found");
                        continue;
                    }

                    foreach (var chapterNode in listChapterNode)
                    {
                        var chapter = new Chapter()
                        {
                            Name = chapterNode.SelectSingleNode(".//a").InnerHtml.Trim(),
                            Url = chapterNode.SelectSingleNode(".//a").Attributes["href"].Value,
                            Comic = comic,
                            Pages = new List<Page>()
                        };
                        Console.WriteLine("Adding chapter: " + chapter.Name);


                        var chapterHtmlPage = GetHtmlNodeByScrapySharp(chapter.Url);

                        var pageNodes = chapterHtmlPage.SelectNodes("//div[@class='page-break no-gaps']");
                        if (pageNodes == null)
                        {
                            Console.WriteLine("pages not found");
                            continue;
                        }

                        foreach (var pageNode in pageNodes)
                        {
                            var pageName = pageNode.SelectSingleNode(".//img").Attributes["id"].Value + "_" + (pageNodes.Count - 1);
                            var urlPage = await DownloadImageToString(pageNode.SelectSingleNode(".//img").Attributes["src"].Value.Trim(), nameComic, chapter.Name, pageName);
                            var page = new Page()
                            {
                                Name = pageName,
                                Url = urlPage,
                                Chapter = chapter
                            };
                            Console.WriteLine("Adding page: " + page.Name);
                            pages.Add(page);
                        }
                        chapters.Add(chapter);
                    }


                    if (nameGenres == null)
                    {
                        Console.WriteLine("Genre not found");
                        continue;
                    }

                    foreach (var nameGenre in nameGenres)
                    {
                        var genreName = nameGenre.InnerHtml.Trim();
                        foreach (var genre in genreList)
                        {
                            if (genre.Name.Equals(genreName))
                            {
                                var comicGenre = new ComicGenre()
                                {
                                    Comic = comic,
                                    Genre = genre,
                                };
                                comicGenres.Add(comicGenre);
                            }
                        }
                    }


                    // Console.WriteLine(comic.Name);
                    // Console.WriteLine(comic.Artist.Name);
                    artists.Add(artist);
                    comics.Add(comic);
                    Console.WriteLine("Done " + count);
                }
            }
        }


        artists = artists.Distinct().ToList();

        // Save the extracted data to the database
        _context.Artists.AddRange(artists);
        _context.Comics.AddRange(comics);
        _context.Genres.AddRange(genreList);
        _context.Chapters.AddRange(chapters);
        _context.Pages.AddRange(pages);
        _context.ComicGenres.AddRange(comicGenres);
        _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");
        throw new Exception("Success!");
    }
    [Obsolete("Obsolete")]
    public async Task<string> DownloadImageToString(string url, string nameComic, string chapter, string namePage)
    {

        try
        {
            Console.WriteLine(url);
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "Mozilla/6.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            byte[] imageBytes = client.DownloadData(url);
            var urlImage = await UploadPageToFile(imageBytes, nameComic, chapter, namePage);
            Console.WriteLine(urlImage);
            return urlImage;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null!;
        }
       
    }

    public async Task<string> UploadPageToFile(byte[] imageBytes, string nameComic, string chapter, string namePage)
    {
        // var firebaseApp = FirebaseApp.Create(new AppOptions()
        // {
        //     Credential = GoogleCredential.FromFile("D:/GO/Download/crawcomic-firebase-adminsdk-4gkut-68d84f9e7a.json"),
        //     ProjectId = "crawcomic"
        // });
        var storage = new FirebaseStorage("crawcomic.appspot.com");
        var stream = new MemoryStream(imageBytes);
        FirebaseStorageTask? task = null;
        if (chapter == "")
        {
            task = storage.Child(nameComic).Child(namePage).PutAsync(stream);
        }
        else
        {
            task = storage.Child(nameComic).Child(chapter).Child(namePage+".jpg").PutAsync(stream);
        }
        var downloadUrl = await task;
        return downloadUrl;
    }
}
