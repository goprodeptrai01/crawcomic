using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Driver;
using ReadMangaTest.Data;
using ReadMangaTest.Models;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace ReadMangaTest.DataScrapping;

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

    // public List<Category> ScrapeCategory()
    public List<BsonDocument> ScrapeCategory()
    {
        // URL of the web page you want to scrape data from
        var url = "https://kunmanga.com/manga-genre/action/";
        var htmlPage = GetHtmlNodeByScrapySharp(url);

        var categoryNodes = htmlPage.SelectNodes(".//li[@class='col-6 col-sm-4 col-md-2']");
        // Console.WriteLine(categoryNodes);
        var categories = new List<BsonDocument>();
        if (categoryNodes == null)
        {
            Console.WriteLine("No data found on the web page");
            return null;
        }


        foreach (var categoryNode in categoryNodes)
        {
            var category = new Category();
            var nameCategory = categoryNode.SelectSingleNode(".//a").InnerText.Trim().Split('\n').ToArray()[0];
            category.Name = nameCategory;
            var urlCategory = categoryNode.SelectSingleNode(".//a").Attributes["href"].Value;
            category.Url = urlCategory;
            category.Description = "";

            categories.Add(new BsonDocument
            {
                { "Name" , category.Name },
                { "Url" , category.Url },
                { "Description" , category.Description },
            });
        }

        // throw new Exception("Debug");
        return categories;
        // return;
        // _context.Categories.AddRange(categories);
        // _context.SaveChanges();
        Console.WriteLine("Data has been saved to the database successfully");
    }

    public async void ScrapeComicAndArtist()
    {
        var connectionString = "mongodb+srv://datacraw:superta01@cluster0.bodptox.mongodb.net/?retryWrites=true&w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("comic");
        // var collection = database.GetCollection<BsonDocument>("people1");
        var categoryCollection = database.GetCollection<BsonDocument>("category");
        var categoryList = ScrapeCategory();
        var comicCollection = database.GetCollection<BsonDocument>("Comic");
        var comics = new List<BsonDocument>();
        var artistCollection = database.GetCollection<BsonDocument>("Artist");
        var artists = new List<BsonDocument>();
        var comicCategoryCollection = database.GetCollection<BsonDocument>("ComicCategory");
        var comicCategories = new List<BsonDocument>();
        var chapterCollection = database.GetCollection<BsonDocument>("Chapter");
        var chapters = new List<BsonDocument>();
        var pageCollection = database.GetCollection<BsonDocument>("Page");
        var pages = new List<BsonDocument>();
        var nullArtist = new Artist()
        {
            Name = "IsUpdating",
            Description = "Is Updating!",
            Url = "Is Updating!"
        };
        artists.Add(nullArtist.ToBsonDocument());
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

                    var nameCategories = comicHtmlPage?.SelectNodes(".//div[@class='genres-content']//a");

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
                        Wallpaper = comicNode.SelectSingleNode(".//div[@class='item-thumb  c-image-hover']//a//img")
                            .Attributes["src"]
                            .Value,
                        Artist = artist,
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
                            var page = new Page()
                            {
                                Name = pageNode.SelectSingleNode(".//img").Attributes["id"].Value + "/" +
                                       pageNodes.Count,
                                Url = pageNode.SelectSingleNode(".//img").Attributes["src"].Value.Trim(),
                                Chapter = chapter
                            };
                            Console.WriteLine("Adding page: " + page.Name);
                            pages.Add(page.ToBsonDocument());
                        }
                        chapters.Add(chapter.ToBsonDocument());
                    }


                    if (nameCategories == null)
                    {
                        Console.WriteLine("category not found");
                        continue;
                    }

                    foreach (var nameCategory in nameCategories)
                    {
                        var categoryName = nameCategory.InnerHtml.Trim();
                        foreach (var category in categoryList)
                        {
                            if (category.GetElement("Name").ToString().Equals(categoryName))
                            {
                                var comicCategory = new ComicCategory()
                                {
                                    Comic = comic,
                                    Category = new Category()
                                    {
                                        Name = category.GetElement("Name").ToString(),
                                        Url = category.GetElement("Url").ToString(),
                                        Description = category.GetElement("Description").ToString(),
                                    }
                                };
                                comic.ComicCategories.Add(comicCategory);
                                comicCategories.Add(comicCategory.ToBsonDocument());
                            }
                        }
                    }


                    // Console.WriteLine(comic.Name);
                    // Console.WriteLine(comic.Artist.Name);
                    artist.Comics?.Add(comic);
                    artists.Add(artist.ToBsonDocument());
                    comics.Add(comic.ToBsonDocument());
                    Console.WriteLine("Done " + count);
                }
            }
        }


        artists = artists.Distinct().ToList();

        // Save the extracted data to the database
        await categoryCollection.InsertManyAsync(categoryList);
        await comicCollection.InsertManyAsync(comics);
        await artistCollection.InsertManyAsync(artists);
        await chapterCollection.InsertManyAsync(chapters);
        await pageCollection.InsertManyAsync(pages);
        await comicCategoryCollection.InsertManyAsync(comicCategories);
        // _context.Artists.AddRange(artists);
        // _context.Comics.AddRange(comics);
        // _context.Categories.AddRange(categoryList);
        // _context.Chapters.AddRange(chapters);
        // _context.Pages.AddRange(pages);
        // _context.ComicCategories.AddRange(comicCategories);
        // _context.SaveChanges();

        Console.WriteLine("Data has been saved to the database successfully");
        throw new Exception("Success!");
    }
}