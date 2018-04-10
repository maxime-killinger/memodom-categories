using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using CsvHelper;
using LumenWorks.Framework.IO.Csv;


namespace memodom
{
    class Program
    {
        public class Categories
        {
            public string categorie { get; set; }
            public string subCategorie { get; set; }
        }

        static void Main(string[] args)
        {
            List<Categories> categories = new List<Categories>();
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter the path to the CSV file");
                System.Console.WriteLine("Usage:\t./memodom ./path/to/file.csv");
                return;
            }
            categories = ReadCsv(args[0]);
            categories = RemoveDuplicate(categories);
            WriteSqlCategories(categories.GroupBy(x => x.categorie).Select(x => x.First()).ToList());
            WriteSqlSubCategories(categories);
        }

        static void WriteSqlSubCategories(List<Categories> categories) 
        {
            foreach (Categories categorie in categories)
            {
                if (categorie.categorie.Count() > 1 && categorie.subCategorie.Count() > 1)
                    Console.WriteLine("INSERT INTO `memodom_test`.`subcategories` (`id`, `title`, `category_id`) VALUES(NULL, '" + categorie.subCategorie + "', (SELECT id FROM categories WHERE title = '" + categorie.categorie + "')); ");
            }
        }

        static void WriteSqlCategories(List<Categories> categories)
        {
            foreach (Categories categorie in categories)
            {
                if (categorie.categorie.Count() > 1)
                    Console.WriteLine("INSERT INTO `memodom_test`.`categories` (`id`, `title`) VALUES(NULL, '" + categorie.categorie + "');");
            }
        }

        static List<Categories> RemoveDuplicate(List<Categories> categories)
        {
            List<Categories> categoriesClean = new List<Categories>();
            int i = 0;

            while (i < categories.Count())
            {
                int j = 0;
                bool check = false;

                while (j < categoriesClean.Count())
                {
                    if ((String.Compare(categoriesClean[j].categorie, categories[i].categorie) == 0
                          && String.Compare(categoriesClean[j].subCategorie, categories[i].subCategorie) == 0))
                    {
                        check = true;
                        break;
                    }
                    j++;
                }
                if (!check)
                {
                    Categories tmpCat = new Categories();

                    tmpCat.categorie = categories[i].categorie;
                    tmpCat.subCategorie = categories[i].subCategorie;
                    categoriesClean.Add(tmpCat);
                }
                i++;
            }
            return (categoriesClean);
        }

        static List<Categories> ReadCsv(string name)
        {
            List<Categories> categoriesList = new List<Categories>();

            using (CsvReader csv = new CsvReader(new StreamReader(name), true, ';'))
            {
                while (csv.ReadNextRecord())
                {
                    Categories categories = new Categories();

                    categories.categorie = csv[0];
                    categories.subCategorie = csv[1];
                    categoriesList.Add(categories);
                }
            }
            return (categoriesList);
        }
    }
};