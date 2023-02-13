// using ReadMangaTest.Models;
//
// namespace ReadMangaTest.Data;
//
// public class Seed
// {
//     private readonly DataContext _context;
//
//     public Seed(DataContext context)
//     {
//         _context = context;
//     }
//
//     public void SeedDataContext()
//     {
//         if (!_context.ComicCategories.Any())
//         {
//             var comicCategories = new List<ComicCategory>()
//             {
//                 new ComicCategory()
//                 {
//                     Comic = new Comic()
//                     {
//                         Name = "Comic 1",
//                         Description = "DesComic 1",
//                         Wallpaper = "WalComic 1",
//                         Artist = new Artist()
//                         {
//                             Name = "Artist 1",
//                             Description = "DesArtist 1",
//                         },
//                         Chapters = new List<Chapter>()
//                         {
//                             new Chapter()
//                             {
//                                 Name = "Chapter 1",
//                                 Content = "Content 1 Comic 1",
//                             },
//                             new Chapter()
//                             {
//                                 Name = "Chapter 2",
//                                 Content = "Content 2 Comic 1",
//                             }
//                         }
//                     },
//                     Category = new Category()
//                     {
//                         Name = "Category 1",
//                         Description = "DesCategory 1"
//                     }
//                 },
//                 new ComicCategory()
//                 {
//                     Comic = new Comic()
//                     {
//                         Name = "Comic 2",
//                         Description = "DesComic 2",
//                         Wallpaper = "WalComic 2",
//                         Artist = new Artist()
//                         {
//                             Name = "Artist 2",
//                             Description = "DesArtist 2",
//                         },
//                         Chapters = new List<Chapter>()
//                         {
//                             new Chapter()
//                             {
//                                 Name = "Chapter 1",
//                                 Content = "Content 1 Comic 2",
//                             },
//                             new Chapter()
//                             {
//                                 Name = "Chapter 2",
//                                 Content = "Content 2 Comic 2",
//                             }
//                         }
//                     },
//                     Category = new Category()
//                     {
//                         Name = "Category 2",
//                         Description = "DesCategory 2"
//                     }
//                 }
//             };
//             // _context.ComicCategories.AddRange(comicCategories);
//             // _context.SaveChanges();
//         }
//     }
// }